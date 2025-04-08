using System.Collections.ObjectModel;
using Time_Meetings_JW.Entities;
using Time_Meetings_JW.Services;
using Time_Meetings_JW.Enums;

namespace Time_Meetings_JW
{
    public partial class MidweekPage : ContentPage
    {
        public ObservableCollection<Part> Parts { get; set; } = new();
        private Services.Timer currentTimer { get; set; } = null;

        public MidweekPage()
        {
            InitializeComponent();
            BindingContext = this;
            // Validar aqui se tem internet, se não houver, mostrar uma mensagem avisando o usuário
            _ = GetPartsWeek();
        }

        private async Task GetPartsWeek()
        {
            try
            {
                Meeting meeting = new();
                await meeting.GetContentMeeting();
                labelWeek.Text = meeting.GetWeek();
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

        private async void ToggleTimerClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int number)
            {
                SetEnableAllParts();
                int index = GetIndexPartByNumber(number);
                var part = Parts[index];

                bool toStart = true;

                if(part.Status == EStatus.Finished || part.Status == EStatus.Delayed)
                {
                    Task<bool> restart = DisplayAlert("Atenção", "Deseja ZERAR esta marcação?", "Sim", "Não");

                    if (!await restart)
                        return;

                    part.Status = EStatus.NotStarted;
                    part.TimeUsed = 0;
                    part.TimeUsedDesc = part.Time ?? 0;
                    toStart = false;
                }

                if (!toStart)
                    return;

                part.Status = part.Status switch
                {
                    EStatus.NotStarted => EStatus.Started,
                    EStatus.Started => part.TimeUsed > part.Time ? EStatus.Delayed : EStatus.Finished,
                    _ => EStatus.NotStarted
                };

                if (currentTimer == null)
                    currentTimer = new Services.Timer(part);

                if (part.Status == EStatus.Started)
                {
                    SetDisableOthersParts(index);
                    currentTimer.StartTimer();
                }
                else if (currentTimer != null && part.Status == EStatus.Finished || part.Status == EStatus.Delayed)
                {
                    currentTimer.StopTimer();
                    currentTimer = null;
                    part.TimeUsedDesc = part.TimeUsed;
                }
            }

            // Implementar o "localStorage" aqui para salvar o array de tempos marcados
            // Caso exista algo no "localStorage", buscar dele para renderizar a variável Parts
            // Caso contrário, buscar do wol.jw.org
        }

        private int GetIndexPartByNumber(int number)
        {
            return Parts.ToList().FindIndex(p => p.Number == number);
        }

        private void SetEnableAllParts()
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                Parts[i].Enabled = true;
            }
        }

        private void SetDisableOthersParts(int index)
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                if (i != index)
                {
                    Parts[i].Enabled = false;
                }
            }
        }
    }
}
