using Time_Meetings_JW.Entities;

namespace Time_Meetings_JW.Services
{
    class Timer
    {
        private IDispatcherTimer timer;
        private Part currentPart;
        
        public Timer(Part part)
        {
            currentPart = part;
        }

        public void StartTimer()
        {
            timer = Application.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                if (currentPart != null)
                {
                    currentPart.TimeUsed++;
                    currentPart.TimeUsedDesc--;
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
