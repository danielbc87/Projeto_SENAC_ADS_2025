using Microsoft.Maui.Controls;

namespace Health_Organizer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        private async void OnAddMedicationClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddMedicationPage());
        }



        private async void OnScheduleAppointmentClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ScheduleAppointmentPage());
        }
        private async void OnViewRecordsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListRecordsPage());
        }
    }
}