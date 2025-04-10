using System.Collections.ObjectModel;
using System.Text.Json;
using Time_Meetings_JW.Entities;
using Time_Meetings_JW.Enums;
using Time_Meetings_JW.Services;

namespace Time_Meetings_JW.Utils
{
    public class ManageParts
    {
        private static void SetPartsByArray(ObservableCollection<Part> Parts, ObservableCollection<Part> partsGetted)
        {
            Parts.Clear();
            foreach (Part part in partsGetted)
            {
                part.Enabled = true;
                part.TimeUsedDesc = (part.Time ?? 0) - part.TimeUsed;
                if (part.Status == EStatus.Started && part.TimeUsed > 0)
                    part.Status = EStatus.Finished;
                else if (part.TimeUsed == 0)
                    part.Status = EStatus.NotStarted;

                Parts.Add(part);
            }

            SaveParts(Parts);
        }

        public static void SetPartsByStorage(Label label, ObservableCollection<Part> Parts, EMeetingType meetingType)
        {
            string labelWeek = Preferences.Get("labelWeek", "");
            label.Text = labelWeek;

            string partsSerialized = meetingType == EMeetingType.Midweek ? Preferences.Get("parts_midweek", "") : Preferences.Get("parts_weekend", "");
            if (!string.IsNullOrEmpty(partsSerialized))
            {
                ObservableCollection<Part> partsDeserialized = JsonSerializer.Deserialize<ObservableCollection<Part>>(partsSerialized);
                SetPartsByArray(Parts, partsDeserialized);
            }
            else
            {
                Parts.Clear();
            }
        }

        public static void ComparePartsStorageWithActual(Label label, Meeting meeting, ObservableCollection<Part> Parts, EMeetingType meetingType)
        {
            string dateMeeting = label.Text;
            if (meeting.GetWeek() != "" && meeting.GetWeek() != dateMeeting)
            {
                dateMeeting = meeting.GetWeek();
                Preferences.Set("labelWeek", dateMeeting);

                SetPartsByArray(Parts, meeting.GetParts());
                SaveParts(Parts);
            }
            else if (Parts.Count == 0)
            {
                SetPartsByArray(Parts, meeting.GetParts());
            }

            label.Text = dateMeeting ?? meeting.GetWeek();
        }

        public static void SaveParts(ObservableCollection<Part> Parts)
        {
            if (Parts[0].Number == 100)
                Preferences.Set("parts_weekend", JsonSerializer.Serialize(Parts));
            else
                Preferences.Set("parts_midweek", JsonSerializer.Serialize(Parts));
        }

        public static async void ToggleTimer(ObservableCollection<Part> Parts, int number, Services.Timer currentTimer)
        {
            SetEnableAllParts(Parts);
            int index = GetIndexPartByNumber(Parts, number);
            var part = Parts[index];

            bool toStart = true;

            if (part.Status == EStatus.Finished || part.Status == EStatus.Delayed)
            {
                Page page = new Page();
                Task<bool> restart = page.DisplayAlert("Atenção", "Deseja CONTINUAR ou ZERAR esta marcação?", "Continuar", "Zerar");

                part.Status = EStatus.NotStarted;
                if (!await restart)
                {
                    part.TimeUsed = 0;
                    part.TimeUsedDesc = part.Time ?? 0;
                    toStart = false;
                }
            }

            if (!toStart)
                return;

            part.Status = part.Status switch
            {
                EStatus.NotStarted => EStatus.Started,
                EStatus.Started => EStatus.Finished,
                EStatus.OutTime => EStatus.Delayed,
                _ => EStatus.NotStarted
            };

            if (currentTimer == null)
                currentTimer = new Services.Timer(part);

            if (part.Status == EStatus.Started)
            {
                SetDisableOthersParts(Parts, index);
                part.TimeUsedDesc = (part.Time ?? 0) - part.TimeUsed;
                currentTimer.StartTimer(Parts);
            }
            else if (currentTimer != null && part.Status == EStatus.Finished || part.Status == EStatus.Delayed)
            {
                currentTimer.StopTimer();
                currentTimer = null;
                part.TimeUsedDesc = part.TimeUsed;
            }
        }

        private static int GetIndexPartByNumber(ObservableCollection<Part> Parts, int number)
        {
            return Parts.ToList().FindIndex(p => p.Number == number);
        }

        private static void SetEnableAllParts(ObservableCollection<Part> Parts)
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                Parts[i].Enabled = true;
            }
        }

        private static void SetDisableOthersParts(ObservableCollection<Part> Parts, int index)
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                if (i != index)
                {
                    Parts[i].Enabled = false;
                }
            }
        }
    }
}
