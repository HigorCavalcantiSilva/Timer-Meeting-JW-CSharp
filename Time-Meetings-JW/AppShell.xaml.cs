using System.ComponentModel;
using Microsoft.Maui.Controls;

namespace Time_Meetings_JW
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        private bool _showMemmorial;

        public bool showMemmorial
        {
            get => _showMemmorial;
            set
            {
                if (_showMemmorial != value)
                {
                    _showMemmorial = value;
                    OnPropertyChanged(nameof(showMemmorial));
                }
            }
        }

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("MidweekPage", typeof(MidweekPage));
            Routing.RegisterRoute("WeekendPage", typeof(WeekendPage));
            Routing.RegisterRoute("ConfigurationsPage", typeof(ConfigurationsPage));
            Routing.RegisterRoute("MemmorialPage", typeof(MemmorialPage));

            BindingContext = this;

            this.PropertyChanged += AppShell_PropertyChanged;

            isMemmorial();
        }

        private void AppShell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FlyoutIsPresented) && FlyoutIsPresented)
            {
                isMemmorial();
            }
        }

        public void isMemmorial()
        {
            showMemmorial = Preferences.Get("is_memmorial", false);
        }
    }
}
