namespace Time_Meetings_JW
{
    public class Time
    {
        public static string GetTimeString(int timeInt)
        {
            string minutes = (timeInt / 60).ToString().PadLeft(2);
            string seconds = (timeInt % 60).ToString().PadLeft(2);
            return $"{minutes}:{seconds}";
        }

        public static int GetTimeInt(string timeString)
        {
            List<string> timeSplitted = [.. timeString.Split(':')];
            int minutes = Convert.ToInt32(timeSplitted[0]) * 60;
            int seconds = Convert.ToInt32(timeSplitted[1]);

            return minutes + seconds;
        }
    }
}
