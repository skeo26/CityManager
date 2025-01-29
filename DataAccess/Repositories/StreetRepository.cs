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
    public class StreetRepository : BaseRepository<Street>
    {

        public StreetRepository(string connectionString) : base(connectionString) { }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public override async Task AddAsync(Street street)
        {
            if (!await IsForeignKeyValidAsync("District", street.id_district))
            {
                throw new ArgumentException($"Жилец с ID {street.id_district} не существует.");
            }

            string sql = "INSERT INTO Street (Id, nameStreet, sizeStreet, id_district) VALUES (@Id, @nameStreet, @sizeStreet, @id_district);";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", street.Id);
                    command.Parameters.AddWithValue("@nameStreet", street.nameStreet);
                    command.Parameters.AddWithValue("@sizeStreet", street.sizeStreet);
                    command.Parameters.AddWithValue("@id_district", street.id_district);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            string sql = "DELETE FROM Street WHERE Id = @Id;";
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

        public override async Task<List<Street>> GetAllAsync()
        {
            var streets = new List<Street>();
            using (NpgsqlConnection connection = CreateConnection())
            {
                await connection.OpenAsync();
                NpgsqlCommand sqlQuery = new NpgsqlCommand("SELECT Id, nameStreet, sizeStreet, id_district FROM Street;", connection);
                using (NpgsqlDataReader reader = await sqlQuery.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        Street street = new Street(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2), reader.GetInt32(3));
                        streets.Add(street);
                    }
                }
            }
            return streets;
        }

        public override async Task UpdateAsync(Street street)
        {
            if (!await IsForeignKeyValidAsync("District", street.id_district))
            {
                throw new ArgumentException($"Жилец с ID {street.id_district} не существует.");
            }

            const string sql = "UPDATE Street SET nameStreet = @nameStreet, sizeStreet = @sizeStreet, id_district = @id_district WHERE Id = @Id;";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", street.Id);
                    command.Parameters.AddWithValue("@nameStreet", street.nameStreet);
                    command.Parameters.AddWithValue("@sizeStreet", street.sizeStreet);
                    command.Parameters.AddWithValue("@id_district", street.id_district);
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
