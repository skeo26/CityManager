using CityManagementApp.BL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CityManagementApp.DataAccess.Repositories
{
    public class RegistrationRepository : BaseRepository<Registration>
    {

        public RegistrationRepository(string connectionString) : base(connectionString) { }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public override async Task AddAsync(Registration registration)
        {
            if (!await IsForeignKeyValidAsync("resident", registration.id_resident))
            {
                throw new ArgumentException($"Жилец с ID {registration.id_resident} не существует.");
            }
            if (!await IsForeignKeyValidAsync("house", registration.id_house))
            {
                throw new ArgumentException($"Дом с ID {registration.id_house} не существует.");
            }

            string sql = "INSERT INTO Registration (Id, dateRegistration, dataDeregistration, id_resident, id_house) VALUES (@Id, @dateRegistration, @dataDeregistration, @id_resident, @id_house);";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", registration.Id);
                    command.Parameters.AddWithValue("@dateRegistration", DateTime.Parse(registration.dateRegistration));
                    command.Parameters.AddWithValue("@dataDeregistration", string.IsNullOrEmpty(registration.dataDeregistration) ? (object)DBNull.Value : DateTime.Parse(registration.dataDeregistration));
                    command.Parameters.AddWithValue("@id_resident", registration.id_resident);
                    command.Parameters.AddWithValue("@id_house", registration.id_house);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            string sql = "DELETE FROM Registration WHERE Id = @Id;";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override async Task<List<Registration>> GetAllAsync()
        {
            var registrations = new List<Registration>();
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                NpgsqlCommand sqlQuery = new NpgsqlCommand("SELECT Id, dateRegistration, dataDeregistration, id_resident, id_house FROM Registration;", connection);
                using (NpgsqlDataReader reader = await sqlQuery.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var idRegistration = reader.GetInt32(0);
                        var dateRegistration = reader.IsDBNull(1) ? null : reader.GetDateTime(1).ToString();
                        var dateDeregistration = reader.IsDBNull(2) ? null : reader.GetDateTime(2).ToString();
                        var idResident = reader.GetInt32(3);
                        var idHouse = reader.GetInt32(4);

                        Registration registration = new Registration(idRegistration, dateRegistration?.Substring(0, 10), dateDeregistration?.Substring(0, 10), idResident, idHouse);
                        registrations.Add(registration);
                    }
                }
            }
            return registrations;
        }

        public override async Task UpdateAsync(Registration registration)
        {
            if (!await IsForeignKeyValidAsync("resident", registration.id_resident))
            {
                throw new ArgumentException($"Жилец с ID {registration.id_resident} не существует.");
            }
            if (!await IsForeignKeyValidAsync("house", registration.id_house))
            {
                throw new ArgumentException($"Дом с ID {registration.id_house} не существует.");
            }

            const string sql = "UPDATE Registration SET dateRegistration = @dateRegistration, dataDeregistration = @dataDeregistration, id_resident = @id_resident, id_house = @id_house WHERE Id = @Id;";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", registration.Id);
                    command.Parameters.AddWithValue("@dateRegistration", DateTime.Parse(registration.dateRegistration.Substring(0, 10)));
                    command.Parameters.AddWithValue("@dataDeregistration",
                             string.IsNullOrEmpty(registration.dataDeregistration) ?
                                DBNull.Value : DateTime.Parse(registration.dataDeregistration.Substring(0, 10)));
                    command.Parameters.AddWithValue("@id_resident", registration.id_resident);
                    command.Parameters.AddWithValue("@id_house", registration.id_house);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override async Task<int> GetNextIdAsync(string name)
        {
            int maxId = 0;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = $"SELECT COALESCE(MAX(Id), 0) FROM \"{name.ToLower()}\";";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    maxId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return maxId + 1;
        }
    }
}
