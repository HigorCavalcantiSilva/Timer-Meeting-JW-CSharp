using System.ComponentModel;
using Time_Meetings_JW.Enums;

namespace Time_Meetings_JW.Entities
{
    public class Part : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public int? Number { get; set; }
        public string? Name { get; set; }
        public int? Time { get; set; }
        public string? Color { get; set; }
        
        private int timeUsedDesc = 0;
        public int TimeUsedDesc
        {
            get => timeUsedDesc;
            set
            {
                if (timeUsedDesc != value)
                {
                    timeUsedDesc = value;
                    OnPropertyChanged(nameof(TimeUsedDesc));
                    OnPropertyChanged(nameof(FormattedTimeUsed));
                }
            }
        }
        public int TimeUsed { get; set; } = 0;

        private bool enabled = true;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        private EStatus status = EStatus.NotStarted;
        public EStatus Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(ColorButton));
                }
            }
        }

        public string? BackgroundTitleParts =>
            isTitle ? Color : Colors.Transparent.ToString();
        public string? ColorText =>
            isTitle ? Colors.White.ToHex() : Color;
        public FontAttributes FontWeight =>
            isTitle ? FontAttributes.Bold : FontAttributes.None;
        public bool isTitle =>
            Time == 0;
        public string ColorButton =>
            Status switch
            {
                EStatus.NotStarted => Colors.Gray.ToHex(),
                EStatus.Started => Colors.Green.ToHex(),
                EStatus.Finished => Colors.Blue.ToHex(),
                EStatus.Delayed => Colors.Red.ToHex(),
                _ => Colors.Gray.ToHex()
            };
        public string? FormattedTimeUsed =>
            TimeSpan.FromSeconds(TimeUsedDesc).ToString(@"mm\:ss");
    }
}
