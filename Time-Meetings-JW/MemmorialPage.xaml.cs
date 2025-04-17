using System.Collections.ObjectModel;
using Time_Meetings_JW.Entities;
using Time_Meetings_JW.Services;
using Time_Meetings_JW.Enums;
using Time_Meetings_JW.Utils;

namespace Time_Meetings_JW
{
    public partial class MemmorialPage : ContentPage
    {
        public ObservableCollection<Part> Parts { get; set; } = new();
        public Services.Timer currentTimer { get; set; } = null;

        public MemmorialPage()
        {
            InitializeComponent();
            BindingContext = this;
            _ = GetPartsMemmorial();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await GetPartsMemmorial();
        }

        private async Task GetPartsMemmorial()
        {
            try
            {
                ManageParts.SetPartsByStorage(null, Parts, EMeetingType.Memmorial);

                Meeting meeting = new();
                meeting.GetContentMeetingMemmorial();

                ManageParts.ComparePartsStorageWithActual(null, meeting, Parts, EMeetingType.Memmorial);
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
