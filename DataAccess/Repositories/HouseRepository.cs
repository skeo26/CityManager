using CityManagementApp.BL;
using CityManagementApp.DataAccess.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess
{
    public class HouseRepository : BaseRepository<House>
    {

        public HouseRepository(string connectionString) : base(connectionString) { }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public override async Task AddAsync(House house)
        {
            if (!await IsForeignKeyValidAsync("street", house.id_street))
            {
                throw new ArgumentException($"Улица с ID {house.id_street} не существует.");
            }

            string sql = "INSERT INTO House (Id, numberHouse, floorHouse, id_street) VALUES (@Id, @numberHouse, @floorHouse, @id_street);";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", house.Id);
                    command.Parameters.AddWithValue("@numberHouse", house.numberHouse);
                    command.Parameters.AddWithValue("@floorHouse", house.floorHouse);
                    command.Parameters.AddWithValue("@id_street", house.id_street);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            string sql = "DELETE FROM House WHERE Id = @Id;";
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

        public override async Task<List<House>> GetAllAsync()
        {
            var houses = new List<House>();
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                NpgsqlCommand sqlQuery = new NpgsqlCommand("SELECT Id, numberHouse, floorHouse, id_street FROM House;", connection);
                using (NpgsqlDataReader reader = await sqlQuery.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        House house = new House(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
                        houses.Add(house);
                    }
                }
            }
            return houses;
        }

        public override async Task UpdateAsync(House house)
        {
            if (!await IsForeignKeyValidAsync("street", house.id_street))
            {
                throw new ArgumentException($"Улица с ID {house.id_street} не существует.");
            }


            const string sql = "UPDATE House SET numberHouse = @numberHouse, floorHouse = @floorHouse, id_street = @id_street WHERE Id = @Id;";
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", house.Id);
                    command.Parameters.AddWithValue("@numberHouse", house.numberHouse);
                    command.Parameters.AddWithValue("@floorHouse", house.floorHouse);
                    command.Parameters.AddWithValue("@id_street", house.id_street);
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
                using (var command = new Npgsql.NpgsqlCommand(query, connection))
                {
                    maxId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return maxId + 1;
        }
    }
}
