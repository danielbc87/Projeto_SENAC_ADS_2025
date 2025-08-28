using Microsoft.Maui.Controls;
using System;

namespace Health_Organizer
{
    public partial class AddMedicationPage : ContentPage
    {
        private Medication _currentMedication;

        public AddMedicationPage(Medication medication = null)
        {
            InitializeComponent();
            frequenciaPicker.SelectedIndex = 1;
            _currentMedication = medication;

            if (_currentMedication != null)
            {
                medNameEntry.Text = _currentMedication.Name;
                medPrimeiroHorarioEntry.Text = _currentMedication.Primeiro_Horário;
                Qtd.Text = _currentMedication.Dias;

                // Preencher o Picker com o valor salvo
                if (!string.IsNullOrEmpty(_currentMedication.Frequencia))
                    frequenciaPicker.SelectedItem = _currentMedication.Frequencia;
            }

        }

        private void OnHorarioTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;

            // remove qualquer caractere que não seja número
            string text = new string(entry.Text?.Where(char.IsDigit).ToArray());

            if (text.Length >= 3)
            {
                // insere ":" após os dois primeiros dígitos
                text = text.Insert(2, ":");
            }

            // limita no formato HH:MM
            if (text.Length > 5)
                text = text.Substring(0, 5);

            // atualiza o texto só se realmente mudou, para evitar loop
            if (entry.Text != text)
                entry.Text = text;
        }


       




        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validação: verificar se os campos estão preenchidos
            if (string.IsNullOrWhiteSpace(medNameEntry.Text) ||
                string.IsNullOrWhiteSpace(medPrimeiroHorarioEntry.Text))
            {
                await DisplayAlert("Erro", "Todos os campos devem ser preenchidos.", "OK");

                // Limpar campos com erro
                medNameEntry.Text = string.IsNullOrWhiteSpace(medNameEntry.Text) ? "" : medNameEntry.Text;
                medPrimeiroHorarioEntry.Text = string.IsNullOrWhiteSpace(medPrimeiroHorarioEntry.Text) ? "" : medPrimeiroHorarioEntry.Text;

                return;
            }

            // Validação: verificar se o horário está no formato HH:MM
            if (!TimeSpan.TryParse(medPrimeiroHorarioEntry.Text, out TimeSpan selectedTime))
            {
                await DisplayAlert("Erro", "Formato de horário inválido. Use HH:MM.", "OK");

                // Limpar campo incorreto
                medPrimeiroHorarioEntry.Text = "";
                return;
            }





            // Criar ou atualizar o medicamento
            if (_currentMedication == null)
            {
                var medication = new Medication
                {
                    Name = medNameEntry.Text,
                    Primeiro_Horário = selectedTime.ToString(@"hh\:mm"),
                    Dias = Qtd.Text,
                    Inicio = DateTime.Today,
                    Frequencia = $"{medFrequenciaEntry.Text} {frequenciaPicker.SelectedItem?.ToString()}",
                    
                };


                await App.Database.SaveMedicationAsync(medication);
            }
            else
            {
                _currentMedication.Name = medNameEntry.Text;
                _currentMedication.Primeiro_Horário = selectedTime.ToString(@"hh\:mm");
                _currentMedication.Dias = Qtd.Text;
                _currentMedication.Frequencia = $"{medFrequenciaEntry.Text} {frequenciaPicker.SelectedItem?.ToString()}";

                await App.Database.UpdateMedicationAsync(_currentMedication);
            }


            await DisplayAlert("Sucesso", "Medicamento salvo com sucesso!", "OK");
            await Navigation.PopAsync();
        }

    }

}

