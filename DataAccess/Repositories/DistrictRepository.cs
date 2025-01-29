using CityManagementApp.BL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CityManagementApp.DataAccess.Repositories
{
    public class DistrictRepository : BaseRepository<District>
    {

        public DistrictRepository(string connectionString) : base(connectionString) { }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public override async Task AddAsync(District district)
        {
            if (!await IsForeignKeyValidAsync("City", district.id_city))
            {
                throw new ArgumentException($"Город с ID {district.id_city} не существует.");
            }

            string sql = "INSERT INTO District (Id, nameDistrict, id_city) VALUES (@Id, @nameDistrict, @id_city);";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", district.Id);
                    command.Parameters.AddWithValue("@nameDistrict", district.nameDistrict);
                    command.Parameters.AddWithValue("@id_city", district.id_city);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            string sql = "DELETE FROM District WHERE Id = @Id;";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override async Task<List<District>> GetAllAsync()
        {
            var districts = new List<District>();
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                NpgsqlCommand sqlQuery = new NpgsqlCommand("SELECT Id, nameDistrict, id_city FROM District;", connection);
                using (NpgsqlDataReader reader = await sqlQuery.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        District district = new District(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2));
                        districts.Add(district);
                    }
                }
            }
            return districts;
        }

        public override async Task UpdateAsync(District district)
        {
            if (!await IsForeignKeyValidAsync("City", district.id_city))
            {
                throw new ArgumentException($"Город с ID {district.id_city} не существует.");
            }

            const string sql = "UPDATE District SET nameDistrict = @nameDistrict, id_city = @id_city WHERE Id = @Id;";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", district.Id);
                    command.Parameters.AddWithValue("@nameDistrict", district.nameDistrict);
                    command.Parameters.AddWithValue("@id_city", district.id_city);
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
