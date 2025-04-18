﻿using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.Net;
using Time_Meetings_JW.Entities;

namespace Time_Meetings_JW.Services
{
    public class Meeting
    {
        private ObservableCollection<Part> Parts = new();
        private string link_master = "https://wol.jw.org";
        private int actual_part = 1;
        private string actual_week = "";
        private enum Colors {
            Zero = 0xFFFFFF,
            First = 0x3C7F8B,
            Second = 0xD68F00,
            Third = 0x942926
        }

        public async Task GetContentMeetingMidweek(Page page)
        {
            if (!await VerifyInternetConnection(page))
                return;

            string link = await GetLinkActualMeeting();
            HtmlDocument doc = await GetHtmlMeeting(link);
            SetActualWeek(doc);

            if (actual_week == Preferences.Get("labelWeek", "") && Preferences.Get("parts_midweek", "") != "")
                return;

            SetSectionsAtMeeting(doc);
        }

        public async Task GetContentMeetingWeekend(Page page)
        {
            if (!await VerifyInternetConnection(page))
                return;

            string link = await GetLinkActualMeeting();
            HtmlDocument doc = await GetHtmlMeeting(link);
            SetActualWeek(doc);

            if (actual_week == Preferences.Get("labelWeek", "") && Preferences.Get("parts_weekend", "") != "")
                return;

            SetPart(100, "Discurso Público", 30, Colors.Zero);
            SetPart(101, "Estudo de A Sentinela", 60, Colors.Zero);
        }

        public void GetContentMeetingMemmorial()
        {
            SetPart(102, "Discurso da Celebração", 45, Colors.Zero);
        }

        public void SetActualWeek(HtmlDocument doc)
        {
            actual_week = doc.DocumentNode.SelectSingleNode("//*[@id='p1']").InnerText;
        }

        public string GetWeek()
        {
            return actual_week;
        }

        private async Task<string> GetLinkActualMeeting()
        {
            string link = "";
            using (var client = new HttpClient())
            {
                try
                {
                    string htmlContent = await client.GetStringAsync($"{link_master}/pt/");

                    HtmlDocument doc = new();
                    doc.LoadHtml(htmlContent);

                    var li_meetings = doc.DocumentNode.SelectSingleNode("//*[@id='menuToday']");
                    if (li_meetings != null)
                    {
                        var href_actual_meeting = li_meetings.SelectSingleNode(".//a");

                        if (href_actual_meeting != null && href_actual_meeting.Attributes["href"] != null)
                        {
                            link = href_actual_meeting.Attributes["href"].Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao buscar HTML: {ex.Message}");
                }
            }

            return link;
        }

        private async Task<HtmlDocument> GetHtmlMeeting(string link)
        {
            HtmlDocument doc = new();
            using (var client = new HttpClient())
            {
                try
                {
                    string htmlContent = await client.GetStringAsync($"{link_master}{link}");

                    doc = new();
                    doc.LoadHtml(htmlContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao buscar HTML: {ex.Message}");
                }
            }

            return doc;
        }

        private void SetSectionsAtMeeting(HtmlDocument doc)
        {
            actual_part = 1;
            SetPart(-1, "Comentários Iniciais", 1, Colors.Zero);
            SetTreasures(doc);
            SetDoYourBestInMinistry(doc);
            SetOurChristianLife(doc);
            SetPart(99, "Comentários Finais", 3, Colors.Zero);
        }

        private void SetPart(int number, string name, int time, Colors? color)
        {
            time = time * 60;
            Parts.Add(new Part
            {
                Name = name + (time > 0 ? $" ({time / 60} min)" : ""),
                Time = time,
                TimeUsedDesc = time,
                Number = number,
                Color = $"#{(int)(color ?? Colors.Zero):X6}"
            });
        }

        private int getTime(string timeUnformatted)
        {
            return Convert.ToInt32(timeUnformatted.Split('(')[1].Split(' ')[0]);
        }

        private void SetPartByClass(string className, HtmlDocument doc, Colors color)
        {
            var list_parts = doc.DocumentNode.SelectNodes($"//*[contains(@class, '{className}')]");

            int idx = 0;
            foreach (var item in list_parts)
            {
                string title = item.InnerText;
                if (idx++ == 0)
                {
                    SetPart(0, title, 0, color);
                    continue;
                }

                int next_id = Convert.ToInt32(item.Attributes["Id"].Value.Split("p")[1]) + 1;
                string time = doc.DocumentNode.SelectSingleNode($"//*[@id='p{next_id}']").InnerText;
                SetPart(actual_part++, title, getTime(time), color);
            }
        }

        private void SetTreasures(HtmlDocument doc)
        {
            SetPartByClass("du-color--teal-700", doc, Colors.First);
        }

        private void SetDoYourBestInMinistry(HtmlDocument doc)
        {
            SetPartByClass("du-color--gold-700", doc, Colors.Second);
        }

        private void SetOurChristianLife(HtmlDocument doc)
        {
            SetPartByClass("du-color--maroon-600", doc, Colors.Third);
        }

        public ObservableCollection<Part> GetParts() {
            return Parts;
        }

        public async Task<bool> VerifyInternetConnection(Page page)
        {
            var entry = await Dns.GetHostEntryAsync("www.google.com");
            if (entry.AddressList.Length == 0)
            {
                page.DisplayAlert("ERRO", "Sem conexão com a internet para baixar reunião semanal", "Fechar");
                return false;
            }
            return true;
        }
    }
}
