using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Time_Meetings_JW
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Part> Parts { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            _ = GetPartsWeek();
        }

        private async Task GetPartsWeek()
        {
            try
            {
                Meeting meeting = new();
                await meeting.GetContentMeeting();
                Parts.Clear();
                foreach(Part part in meeting.GetParts())
                {
                    Parts.Add(part);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar partes: {ex.Message}", "OK");
                Console.WriteLine($"Erro completo: {ex}");
            }
        }
    }
}
