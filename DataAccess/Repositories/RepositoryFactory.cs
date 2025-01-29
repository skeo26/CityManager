using CityManagementApp.BL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly Dictionary<string, BaseRepository<object>> _repositories;
        private string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        public RepositoryFactory(string connectionString)
        {
            _repositories = new Dictionary<string, BaseRepository<object>>
            {
                { "city", new RepositoryAdapter<City>(new CityRepository(connectionString)) },
                { "district", new RepositoryAdapter<District>(new DistrictRepository(connectionString)) },
                { "street", new RepositoryAdapter<Street>(new StreetRepository(connectionString)) },
                { "house", new RepositoryAdapter<House>(new HouseRepository(connectionString)) },
                { "resident", new RepositoryAdapter<Resident>(new ResidentRepository(connectionString)) },
                { "registration", new RepositoryAdapter<Registration>(new RegistrationRepository(connectionString)) },
            };
        }

        public object CreateNewItemForTable(string tableName)
        {
            switch (tableName)
            {
                case "city":
                    return new City();
                case "district":
                    return new District();
                case "house":
                    return new House();
                case "registration":
                    return new Registration();
                case "street":
                    return new Street();
                case "resident":
                    return new Resident();
                default:
                    return null;
            }
        }

        public BaseRepository<object> GetRepository(string tableName)
        {
            _repositories.TryGetValue(tableName, out var repository);
            return repository;
        }

        public List<string> GetTableNames()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var tableNames = new List<string>();
            using (var command = new NpgsqlCommand(
                "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableNames.Add(reader.GetString(0));
                    }
                }
            }
            return tableNames;
        }
    }
}
