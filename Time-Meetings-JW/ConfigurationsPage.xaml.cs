using Time_Meetings_JW.Utils;

namespace Time_Meetings_JW
{
    public partial class ConfigurationsPage : ContentPage
    {
        public ConfigurationsPage()
        {
            InitializeComponent();
            BindingContext = this;

            switchMemmorial.IsToggled = Preferences.Get("is_memmorial", false);
            switchVisitCircuit.IsToggled = Preferences.Get("is_visit_circuit", false);
        }

        private void OnSwitchMemmorialToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("is_memmorial", e.Value);
            if (e.Value == false)
                Preferences.Remove("memmorial");
        }

        private void OnSwitchVisitCircuitToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("is_visit_circuit", e.Value);

            if (e.Value == true)
            {
                ManageParts.SetPartsVisit();
            }
            else
            {
                ManageParts.RemovePartsVisit();
            }
        }

        private async void DeleteInfosClicked(object sender, EventArgs e)
        {
            if (sender is Button button && int.TryParse(button.CommandParameter?.ToString(), out int number))
            {
                Task<bool> confirm;
                if (number == 1)
                {
                    confirm = DisplayAlert("Atenção", "Deseja limpar informações da REUNIÃO DO MEIO DE SEMANA?", "Sim", "Não");
                    if(await confirm)
                    {
                        StopTimerActual();
                        Preferences.Remove("parts_midweek");
                    }
                }
                else if (number == 2)
                {
                    confirm = DisplayAlert("Atenção", "Deseja limpar informações da REUNIÃO DO FIM DE SEMANA?", "Sim", "Não");
                    if (await confirm)
                    {
                        StopTimerActual();
                        Preferences.Remove("parts_weekend");
                    }
                }
                else if (number == 3)
                {
                    confirm = DisplayAlert("Atenção", "Deseja limpar informações de TODAS AS REUNIÕES?", "Sim", "Não");
                    if (await confirm)
                    {
                        StopTimerActual();
                        switchMemmorial.IsToggled = false;
                        switchVisitCircuit.IsToggled = false;
                        Preferences.Clear();
                    }
                }
            }
        }

        private void StopTimerActual()
        {
            if (ManageParts._currentTimer != null)
                ManageParts._currentTimer.StopTimer();
        }
    }
}
