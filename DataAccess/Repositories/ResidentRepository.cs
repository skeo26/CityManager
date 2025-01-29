using CityManagementApp.BL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess.Repositories
{
    public class ResidentRepository : BaseRepository<Resident>
    {

        public ResidentRepository(string connectionString) : base(connectionString) { }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public override async Task AddAsync(Resident resident)
        {
            if (!await IsForeignKeyValidAsync("house", resident.id_house))
            {
                throw new ArgumentException($"Жилец с ID {resident.id_house} не существует.");
            }

            string sql = "INSERT INTO Resident (Id, fullName, birthday, passportNumber, passportSeries, id_house)" +
                " VALUES (@Id ,@fullName, @birthday, @passportNumber, @passportSeries, @id_house);";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", resident.Id);
                    command.Parameters.AddWithValue("@fullName", resident.fullName);
                    command.Parameters.AddWithValue("@birthday", DateTime.Parse(resident.birthday));
                    command.Parameters.AddWithValue("@passportNumber", resident.passportNumber);
                    command.Parameters.AddWithValue("@passportSeries", resident.passportSeries);
                    command.Parameters.AddWithValue("@id_house", resident.id_house);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            string sql = "DELETE FROM Resident WHERE Id = @Id;";
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

        public override async Task<List<Resident>> GetAllAsync()
        {
            var residents = new List<Resident>();
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                NpgsqlCommand sqlQuery = new NpgsqlCommand("SELECT Id, fullName, birthday, passportNumber, passportSeries, id_house FROM Resident;", connection);
                using (NpgsqlDataReader reader = await sqlQuery.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        Resident resident = new Resident(reader.GetInt32(0), reader.GetString(1), reader.GetDateTime(2).ToString().Substring(0, 10),
                            reader.GetString(3), reader.GetString(4), reader.GetInt32(5));
                        residents.Add(resident);
                    }
                }
            }
            return residents;
        }

        public override async Task UpdateAsync(Resident resident)
        {
            if (!await IsForeignKeyValidAsync("house", resident.id_house))
            {
                throw new ArgumentException($"Жилец с ID {resident.id_house} не существует.");
            }

            const string sql = "UPDATE Resident SET fullName = @fullName, birthday = @birthday, " +
                "passportNumber = @passportNumber, passportSeries = @passportSeries, id_house = @id_house WHERE Id = @Id;";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", resident.Id);
                    command.Parameters.AddWithValue("@fullName", resident.fullName);
                    command.Parameters.AddWithValue("@birthday", DateTime.Parse(resident.birthday.Substring(0, 10)));
                    command.Parameters.AddWithValue("@passportNumber", resident.passportNumber);
                    command.Parameters.AddWithValue("@passportSeries", resident.passportSeries);
                    command.Parameters.AddWithValue("@id_house", resident.id_house);
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
