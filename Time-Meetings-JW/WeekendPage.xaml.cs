﻿using System.Collections.ObjectModel;
using Time_Meetings_JW.Entities;
using Time_Meetings_JW.Services;
using Time_Meetings_JW.Enums;
using Time_Meetings_JW.Utils;

namespace Time_Meetings_JW
{
    public partial class WeekendPage : ContentPage
    {
        public ObservableCollection<Part> Parts { get; set; } = new();
        public Services.Timer currentTimer { get; set; } = null;

        public WeekendPage()
        {
            InitializeComponent();
            BindingContext = this;
            _ = GetPartsWeekend();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Preferences.Get("parts_weekend", "") == "")
            {
                _ = GetPartsWeekend();
            }
        }

        private async Task GetPartsWeekend()
        {
            try
            {
                ManageParts.SetPartsByStorage(labelWeek, Parts, EMeetingType.Weekend);

                Meeting meeting = new();
                await meeting.GetContentMeetingWeekend();

                ManageParts.ComparePartsStorageWithActual(labelWeek, meeting, Parts, EMeetingType.Weekend);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar partes: {ex.Message}", "OK");
                Console.WriteLine($"Erro completo: {ex}");
            }
        }

        private void ToggleTimerClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int number)
            {
                ManageParts.ToggleTimer(Parts, number, currentTimer);
            }
        }
    }
}
