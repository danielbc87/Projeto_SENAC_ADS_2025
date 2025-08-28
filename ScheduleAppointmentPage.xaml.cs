using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Linq; // <- necessário para .Where(...)

namespace Health_Organizer
{
    public partial class ScheduleAppointmentPage : ContentPage
    {
        private Appointment _currentAppointment;

        // Construtor modificado para aceitar uma consulta existente (opcional)
        public ScheduleAppointmentPage(Appointment appointment = null)
        {
            InitializeComponent();
            _currentAppointment = appointment;

            if (_currentAppointment != null)
            {
                docNameEntry.Text = _currentAppointment.Doctor;
                dateEntry.Text = _currentAppointment.Date;
                timeEntry.Text = _currentAppointment.Time;

                // (opcional) sincroniza o DatePicker se já vier data preenchida
                if (DateTime.TryParseExact(
                        dateEntry.Text,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var dt))
                {
                    // se você tem o datePicker no XAML:
                    datePicker.Date = dt;
                }
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            // Entrega no formato que o resto do fluxo já usa:
            dateEntry.Text = e.NewDate.ToString("dd/MM/yyyy");
        }

        private void OnDateEntryTapped(object sender, EventArgs e)
        {
            // Abre o calendário ao tocar no Entry “falso”
            datePicker.Focus();
        }

        private void OnHorarioTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;

            // pega só dígitos da nova entrada (nunca use entry.Text aqui)
            string digits = new string((e.NewTextValue ?? string.Empty).Where(char.IsDigit).ToArray());

            if (digits.Length >= 3)
                digits = digits.Insert(2, ":");

            if (digits.Length > 5)
                digits = digits.Substring(0, 5);

            if (entry.Text != digits)
                entry.Text = digits;
        }

        // ⚠️ Se você estiver usando DatePicker para a data,
        // NÃO ligue este handler no XAML (deixa aqui apenas como fallback).
        private async void OnDataTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;

            string text = new string((e.NewTextValue ?? string.Empty).Where(char.IsDigit).ToArray());

            if (text.Length >= 3)
                text = text.Insert(2, "/");

            if (text.Length >= 6)
                text = text.Insert(5, "/");

            if (text.Length > 10)
                text = text.Substring(0, 10);

            if (entry.Text != text)
                entry.Text = text;

            // Valida apenas quando completar 10 caracteres
            if (text.Length == 10)
            {
                if (DateTime.TryParseExact(text, "dd/MM/yyyy",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out DateTime selectedDate))
                {
                    if (selectedDate < DateTime.Today)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Erro", "A data da consulta não pode ser no passado.", "OK");
                        entry.Text = "";
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Erro", "Data inválida.", "OK");
                    entry.Text = "";
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Campos obrigatórios
            if (string.IsNullOrWhiteSpace(docNameEntry.Text) ||
                string.IsNullOrWhiteSpace(dateEntry.Text) ||
                string.IsNullOrWhiteSpace(timeEntry.Text))
            {
                await DisplayAlert("Erro", "Todos os campos devem ser preenchidos.", "OK");

                docNameEntry.Text = string.IsNullOrWhiteSpace(docNameEntry.Text) ? "" : docNameEntry.Text;
                dateEntry.Text = string.IsNullOrWhiteSpace(dateEntry.Text) ? "" : dateEntry.Text;
                timeEntry.Text = string.IsNullOrWhiteSpace(timeEntry.Text) ? "" : timeEntry.Text;
                return;
            }

            // Data: use TryParseExact para garantir dd/MM/yyyy
            if (!DateTime.TryParseExact(
                    dateEntry.Text,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime selectedDate))
            {
                await DisplayAlert("Erro", "Formato de data inválido. Use DD/MM/AAAA.", "OK");
                dateEntry.Text = "";
                return;
            }

            // Data não pode ser no passado
            if (selectedDate < DateTime.Today)
            {
                await DisplayAlert("Erro", "A data da consulta não pode ser no passado.", "OK");
                dateEntry.Text = "";
                return;
            }

            // Hora: use TryParseExact para hh:mm (24h)
            if (!TimeSpan.TryParseExact(
                    timeEntry.Text,
                    "hh\\:mm",
                    CultureInfo.InvariantCulture,
                    out TimeSpan selectedTime))
            {
                await DisplayAlert("Erro", "Formato de horário inválido. Use HH:MM.", "OK");
                timeEntry.Text = "";
                return;
            }

            // Criar ou atualizar a consulta
            if (_currentAppointment == null)
            {
                var appointment = new Appointment
                {
                    Doctor = docNameEntry.Text,
                    Date = selectedDate.ToString("dd/MM/yyyy"),
                    Time = selectedTime.ToString(@"hh\:mm")
                };
                await App.Database.SaveAppointmentAsync(appointment);
            }
            else
            {
                _currentAppointment.Doctor = docNameEntry.Text;
                _currentAppointment.Date = selectedDate.ToString("dd/MM/yyyy");
                _currentAppointment.Time = selectedTime.ToString(@"hh\:mm");
                await App.Database.UpdateAppointmentAsync(_currentAppointment);
            }

            await DisplayAlert("Sucesso", "Consulta salva com sucesso!", "OK");
            await Navigation.PopAsync();
        }
    }
}
