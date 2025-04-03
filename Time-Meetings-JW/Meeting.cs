using HtmlAgilityPack;
using System.Collections.ObjectModel;

namespace Time_Meetings_JW
{
    public class Meeting
    {
        private ObservableCollection<Part> Parts = new();
        private string link_master = "https://wol.jw.org";
        private int actual_part = 1;

        public async Task GetContentMeeting()
        {
            string link = await GetLinkActualMeeting();
            HtmlDocument doc = await GetHtmlMeeting(link);
            SetSectionsAtMeeting(doc);
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
            SetPart(-1, "Comentários Iniciais", 1);
            SetTreasures(doc);
            SetDoYourBestInMinistry(doc);
            SetOurChristianLife(doc);
            SetPart(99, "Comentários Finais", 3);
        }

        private void SetPart(int number, string name, int time)
        {
            Parts.Add(new Part
            {
                Name = name,
                Time = time,
                Number = number
            });
        }

        private int getTime(string timeUnformatted)
        {
            return Convert.ToInt32(timeUnformatted.Split('(')[1].Split(' ')[0]);
        }

        private void SetPartByClass(string className, HtmlDocument doc)
        {
            var list_parts = doc.DocumentNode.SelectNodes($"//*[contains(@class, '{className}')]");

            int idx = 0;
            foreach (var item in list_parts)
            {
                string title = item.InnerText;
                if (idx++ == 0)
                {
                    SetPart(0, title, 0);
                    continue;
                }

                int next_id = Convert.ToInt32(item.Attributes["Id"].Value.Split("p")[1]) + 1;
                string time = doc.DocumentNode.SelectSingleNode($"//*[@id='p{next_id}']").InnerText;
                SetPart(actual_part++, title, getTime(time));
            }
        }

        private void SetTreasures(HtmlDocument doc)
        {
            SetPartByClass("du-color--teal-700", doc);
        }

        private void SetDoYourBestInMinistry(HtmlDocument doc)
        {
            SetPartByClass("du-color--gold-700", doc);
        }

        private void SetOurChristianLife(HtmlDocument doc)
        {
            SetPartByClass("du-color--maroon-600", doc);
        }

        public ObservableCollection<Part> GetParts() {
            return Parts;
        }
    }
}
