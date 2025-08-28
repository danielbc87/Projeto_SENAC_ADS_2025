using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace Health_Organizer
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _database;

        public Database()

        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "health_organizer.db3");


            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Medication>().Wait();
            _database.CreateTableAsync<Appointment>().Wait();
        }

        // CRUD - Medicamentos
        public Task<int> SaveMedicationAsync(Medication medication) => _database.InsertAsync(medication);
        public Task<List<Medication>> GetMedicationsAsync() => _database.Table<Medication>().ToListAsync();

        // CRUD - Consultas
        public Task<int> SaveAppointmentAsync(Appointment appointment) => _database.InsertAsync(appointment);
        public Task<List<Appointment>> GetAppointmentsAsync() => _database.Table<Appointment>().ToListAsync();

        // Método para excluir um medicamento
        public Task<int> DeleteMedicationAsync(Medication medication)
        {
            return _database.DeleteAsync(medication);
        }

        // Método para excluir uma consulta
        public Task<int> DeleteAppointmentAsync(Appointment appointment)
        {
            return _database.DeleteAsync(appointment);
        }
        public Task<int> UpdateMedicationAsync(Medication medication)
        {
            return _database.UpdateAsync(medication);
        }
        public Task<int> UpdateAppointmentAsync(Appointment appointment)
        {
            return _database.UpdateAsync(appointment);
        }

    }

    // Modelo para Medicamentos
    public class Medication
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Primeiro_Horário { get; set; }
        public string Frequencia { get; set; }
        public string Dias { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Proximo { get; set; }
        public DateTime? Final
        {
            get
            {
                if (Inicio == null || string.IsNullOrWhiteSpace(Frequencia) || string.IsNullOrWhiteSpace(Dias))
                    return null;

                var partes = Frequencia.Split(' ');
                if (partes.Length != 2)
                    return null;

                if (!int.TryParse(partes[0], out int quantidadeFrequencia))
                    return null;

                if (!int.TryParse(Dias, out int quantidadeDias))
                    return null;

                string unidade = partes[1].ToLower();

                if (unidade.Contains("dia"))
                {
                    // Se a frequência é "a cada X dias", então a duração é: Início + (X * quantidadeDias)
                    return Inicio.Value.AddDays(quantidadeFrequencia * quantidadeDias);
                }
                else if (unidade.Contains("hora"))
                {
                    // Se a frequência é "a cada X horas", então:
                    // número total de horas = quantidadeDias * 24 / frequência
                    // → número de doses = quantidadeDias * (24 / frequência)
                    // → tempo total = número de doses * frequência (em horas)
                    double dosesPorDia = 24.0 / quantidadeFrequencia;
                    int totalDoses = (int)(quantidadeDias * dosesPorDia);
                    int totalHoras = totalDoses * quantidadeFrequencia;

                    return Inicio.Value.AddHours(totalHoras);
                }
                else
                {
                    return null;
                }
            }
        }

        public string Resumo
        {
            get
            {
                string dataInicio = Inicio?.ToString("dd/MM/yyyy") ?? "sem data";
                string dataFinal = Final?.ToString("dd/MM/yyyy HH:mm") ?? "sem previsão";

                return $"Medicamento: {Name} \n a cada {Frequencia}, por {Dias} dias \n início em {dataInicio} \n termina em {dataFinal}";
            }
        }
    }








}

    // Modelo para Consultas
    public class Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Doctor { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }

