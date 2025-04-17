using System.Collections.ObjectModel;
using Time_Meetings_JW.Entities;
using Time_Meetings_JW.Enums;
using Time_Meetings_JW.Utils;

namespace Time_Meetings_JW.Services
{
    public class Timer
    {
        private IDispatcherTimer timer;
        private Part currentPart;
        
        public Timer(Part part)
        {
            currentPart = part;
        }

        public void StartTimer(ObservableCollection<Part> Parts)
        {
            timer = Application.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                if (currentPart != null)
                {
                    currentPart.TimeUsed++;
                    currentPart.TimeUsedDesc--;

                    if (currentPart.TimeUsedDesc < 0)
                        currentPart.Status = EStatus.OutTime;

                    if (currentPart.TimeUsed > currentPart.Time - 30 && currentPart.TimeUsedDesc <= 30 && currentPart.TimeUsedDesc >= 0)
                        currentPart.Status = EStatus.Ending;

                    ManageParts.SaveParts(Parts);
                }
            };
            timer.Start();
        }
        public void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }
    }
}
