namespace Time_Meetings_JW
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("MidweekPage", typeof(MidweekPage));
            Routing.RegisterRoute("WeekendPage", typeof(WeekendPage));
            Routing.RegisterRoute("ConfigurationsPage", typeof(ConfigurationsPage));
        }
    }
}
