using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Health_Organizer
{
    public partial class ListRecordsPage : ContentPage
    {
        public ListRecordsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            // Buscar medicamentos e consultas no banco de dados
            List<Medication> medications = await App.Database.GetMedicationsAsync();
            List<Appointment> appointments = await App.Database.GetAppointmentsAsync();

            // Atualizar as listas na interface
            MedicationsListView.ItemsSource = medications;
            AppointmentsListView.ItemsSource = appointments;
        }

        // M�todo para excluir um medicamento
        private async void OnDeleteMedicationClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var medication = button?.CommandParameter as Medication;

            if (medication != null)
            {
                bool confirm = await DisplayAlert("Confirma��o", $"Deseja excluir {medication.Name}?", "Sim", "N�o");
                if (confirm)
                {
                    await App.Database.DeleteMedicationAsync(medication);
                    LoadData(); // Atualiza a lista
                }
            }
        }

        // M�todo para excluir uma consulta
        private async void OnDeleteAppointmentClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var appointment = button?.CommandParameter as Appointment;

            if (appointment != null)
            {
                bool confirm = await DisplayAlert("Confirma��o", $"Deseja excluir a consulta com {appointment.Doctor}?", "Sim", "N�o");
                if (confirm)
                {
                    await App.Database.DeleteAppointmentAsync(appointment);
                    LoadData(); // Atualiza a lista
                }
            }
        }

        // M�todo para editar um medicamento (redireciona para a tela de edi��o)
        private async void OnEditMedicationClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var medication = button?.CommandParameter as Medication;

            if (medication != null)
            {
                await Navigation.PushAsync(new AddMedicationPage(medication));
            }
        }

        // M�todo para editar uma consulta (redireciona para a tela de edi��o)
        private async void OnEditAppointmentClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var appointment = button?.CommandParameter as Appointment;

            if (appointment != null)
            {
                await Navigation.PushAsync(new ScheduleAppointmentPage(appointment));
            }
        }
    }
}

