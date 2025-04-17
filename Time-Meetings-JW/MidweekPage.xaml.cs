using System.Collections.ObjectModel;
using Time_Meetings_JW.Entities;
using Time_Meetings_JW.Services;
using Time_Meetings_JW.Enums;
using Time_Meetings_JW.Utils;

namespace Time_Meetings_JW
{
    public partial class MidweekPage : ContentPage
    {
        public ObservableCollection<Part> Parts { get; set; } = new();
        public Services.Timer currentTimer { get; set; } = null;

        public MidweekPage()
        {
            InitializeComponent();
            BindingContext = this;
            _ = GetPartsWeek();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await GetPartsWeek();
        }

        private async Task GetPartsWeek()
        {
            try
            {
                ManageParts.SetPartsByStorage(labelWeek, Parts, EMeetingType.Midweek);

                Meeting meeting = new();
                await meeting.GetContentMeetingMidweek();

                ManageParts.ComparePartsStorageWithActual(labelWeek, meeting, Parts, EMeetingType.Midweek);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar partes: {ex.Message}", "OK");
                Console.WriteLine($"Erro completo: {ex}");
            }
        }

        private async void ToggleTimerClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int number)
            {
                currentTimer = await ManageParts.ToggleTimer(this, Parts, number, currentTimer);
            }
        }
    }
}
