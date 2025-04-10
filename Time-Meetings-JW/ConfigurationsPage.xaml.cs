namespace Time_Meetings_JW
{
    public partial class ConfigurationsPage : ContentPage
    {
        public ConfigurationsPage()
        {
            InitializeComponent();
            BindingContext = this;
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
                        Preferences.Remove("parts_midweek");
                }
                else if (number == 2)
                {
                    confirm = DisplayAlert("Atenção", "Deseja limpar informações da REUNIÃO DO FIM DE SEMANA?", "Sim", "Não");
                    if (await confirm)
                        Preferences.Remove("parts_weekend");
                }
                else if (number == 3)
                {
                    confirm = DisplayAlert("Atenção", "Deseja limpar informações de TODAS AS REUNIÕES?", "Sim", "Não");
                    if (await confirm)
                        Preferences.Clear();
                }
            }
        }
    }
}
