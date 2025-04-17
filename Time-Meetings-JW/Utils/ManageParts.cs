using System.Collections.ObjectModel;
using System.Text.Json;
using Time_Meetings_JW.Entities;
using Time_Meetings_JW.Enums;
using Time_Meetings_JW.Services;

namespace Time_Meetings_JW.Utils
{
    public class ManageParts
    {
        public static Services.Timer _currentTimer;

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
            if (label != null)
            {
                string labelWeek = Preferences.Get("labelWeek", "");
                label.Text = labelWeek;
            }

            string key = meetingType switch
            {
                EMeetingType.Midweek => "parts_midweek",
                EMeetingType.Weekend => "parts_weekend",
                EMeetingType.Memmorial => "memmorial",
                _ => ""
            };


            ObservableCollection<Part> partsDeserialized = GetPartsDeserialized(key);
            
            if (partsDeserialized != null)
            {
                SetPartsByArray(Parts, partsDeserialized);
            }
            else
            {
                Parts.Clear();
            }
        }

        public static void ComparePartsStorageWithActual(Label label, Meeting meeting, ObservableCollection<Part> Parts, EMeetingType meetingType)
        {
            if (label != null)
            {
                string dateMeeting = label.Text;
                if (meeting.GetWeek() != "" && meeting.GetWeek() != dateMeeting)
                {
                    dateMeeting = meeting.GetWeek();
                    Preferences.Set("labelWeek", dateMeeting);

                    SetPartsByArray(Parts, meeting.GetParts());
                    SaveParts(Parts);
                }
                else if (meeting.GetParts().Count > 0)
                {
                    {
                        SetPartsByArray(Parts, meeting.GetParts());
                    }

                    label.Text = dateMeeting ?? meeting.GetWeek();
                }
            }
            else
            {
                SetPartsByArray(Parts, meeting.GetParts());
            }
        }

        public static void SaveParts(ObservableCollection<Part> Parts)
        {
            if (Parts[0].Number == 100)
                Preferences.Set("parts_weekend", JsonSerializer.Serialize(Parts));
            else if (Parts[0].Number <= 101)
                Preferences.Set("parts_midweek", JsonSerializer.Serialize(Parts));
            else if (Parts[0].Number == 102)
                Preferences.Set("memmorial", JsonSerializer.Serialize(Parts));
        }

        public static async Task<Services.Timer> ToggleTimer(Page page, ObservableCollection<Part> Parts, int number, Services.Timer currentTimer)
        {
            _currentTimer = currentTimer;
            SetEnableAllParts(Parts);
            int index = GetIndexPartByNumber(Parts, number);
            var part = Parts[index];

            bool toStart = true;

            if (part.Status == EStatus.Finished || part.Status == EStatus.Delayed)
            {
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
                return _currentTimer;

            part.Status = part.Status switch
            {
                EStatus.NotStarted => EStatus.Started,
                EStatus.Started => EStatus.Finished,
                EStatus.OutTime => EStatus.Delayed,
                _ => EStatus.NotStarted
            };

            if (_currentTimer == null)
                _currentTimer = new Services.Timer(part);

            if (part.Status == EStatus.Started)
            {
                SetDisableOthersParts(Parts, index);
                part.TimeUsedDesc = (part.Time ?? 0) - part.TimeUsed;
                _currentTimer.StopTimer();
                _currentTimer.StartTimer(Parts);
            }
            else if (_currentTimer != null && part.Status == EStatus.Finished || part.Status == EStatus.Delayed)
            {
                _currentTimer.StopTimer();
                _currentTimer = null;
                part.TimeUsedDesc = part.TimeUsed;
            }

            return _currentTimer;
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

        #region [Visita do Superintendente de Circuito]
        public static void SetPartsVisit()
        {
            SetPartsVisitMidweek();
            SetPartsVisitWeekend();
        }

        private static void SetPartsVisitMidweek()
        {
            ObservableCollection<Part> partsDeserialized = GetPartsDeserialized("parts_midweek");
            if (partsDeserialized != null)
            {
                partsDeserialized[partsDeserialized.Count - 2].Name = "Discurso de Serviço - Visita";
                SaveParts(partsDeserialized);
            }
        }

        private static void SetPartsVisitWeekend()
        {
            ObservableCollection<Part> partsDeserialized = GetPartsDeserialized("parts_weekend");
            if (partsDeserialized != null)
            {
                partsDeserialized[partsDeserialized.Count - 1].Time = 30 * 60;
                partsDeserialized.Add(new Part
                {
                    Name = "Discurso Final - Visita",
                    Time = 30 * 60,
                    Status = EStatus.NotStarted
                });
                SaveParts(partsDeserialized);
            }
        }

        public static void RemovePartsVisit()
        {
            RemovePartsVisitMidweek();
            RemovePartsVisitWeekend();
        }

        private static void RemovePartsVisitMidweek()
        {
            ObservableCollection<Part> partsDeserialized = GetPartsDeserialized("parts_midweek");
            
            if (partsDeserialized != null)
            {
                partsDeserialized[partsDeserialized.Count - 2].Name = "Estudo bíblico de congregação (30 min)";
                SaveParts(partsDeserialized);
            }
        }

        private static void RemovePartsVisitWeekend()
        {
            ObservableCollection<Part> partsDeserialized = GetPartsDeserialized("parts_weekend");

            if (partsDeserialized != null)
            {
                partsDeserialized.RemoveAt(partsDeserialized.Count - 1);
                partsDeserialized[partsDeserialized.Count - 1].Time = 60 * 60;
                SaveParts(partsDeserialized);
            }
        }

        #endregion

        private static ObservableCollection<Part> GetPartsDeserialized(string nameStore)
        {
            string partsSerialized = Preferences.Get(nameStore, "");

            if (!string.IsNullOrEmpty(partsSerialized))
            {
                ObservableCollection<Part> partsDeserialized = JsonSerializer.Deserialize<ObservableCollection<Part>>(partsSerialized);
                return partsDeserialized;
            }

            return null;
        }
    }
}
