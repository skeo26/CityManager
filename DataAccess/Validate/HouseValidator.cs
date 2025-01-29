using CityManagementApp.BL;
using CityManagementApp.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CityManagementApp.DataAccess.Validate
{
    public class HouseValidator : IValidator
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private const int minNumberHouse = 1;
        private const int maxNumberHouse = 10000;

        private const int minFloorHouse = 1;
        private const int maxFloorHouse = 170;

        public HouseValidator(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<string>> ValidateAsync(object entity)
        {
            var errors = new List<string>();
            var idProperty = entity.GetType().GetProperty("Id");
            var entityId = idProperty?.GetValue(entity);
            var numberHouse = entity.GetType().GetProperty("numberHouse")?.GetValue(entity)?.ToString();
            var floorHouse = entity.GetType().GetProperty("floorHouse")?.GetValue(entity)?.ToString();
            var idStreet = entity.GetType().GetProperty("id_street")?.GetValue(entity);

            ValidateNumberHouse(numberHouse, errors);
            ValidateFloorHouse(floorHouse, errors);
            await ValidateStreetExistsAsync(idStreet, errors);
            await ValidateUniqueHouseOnStreetAsync(numberHouse, idStreet, errors, entityId);

            return errors;
        }
         
        private void ValidateNumberHouse(string numberHouse, List<string> errors)
        {
            if (!int.TryParse(numberHouse, out int number))
            {
                errors.Add("Номер дома некорректен!");
                return;
            }

            if (number < minNumberHouse || number > maxNumberHouse)
            {
                errors.Add("Номер дома выходит из диапазона допустимых");
            }
        }

        private void ValidateFloorHouse(string floorHouse, List<string> errors)
        {
            if (!int.TryParse(floorHouse, out int floor))
            {
                errors.Add("Этаж дома некорректен!");
                return;
            }

            if (floor < minFloorHouse || floor > maxFloorHouse)
            {
                errors.Add("Этаж дома выходит из диапазона допустимых");
            }
        }

        private async Task ValidateStreetExistsAsync(object idStreet, List<string> errors)
        {
            if (idStreet == null || !await _repositoryFactory.GetRepository("street").ExistsAsync("street", "Id", idStreet))
            {
                errors.Add($"Улица с Id '{idStreet}' не существует.");
            }
        }

        private async Task ValidateUniqueHouseOnStreetAsync(string numberHouse, object idStreet, List<string> errors, object entityId)
        {
            if (int.TryParse(numberHouse, out int number))
            {
                if (await HouseWithSameStreetExistsAsync(number, idStreet, entityId))
                {
                    errors.Add($"Дом с номером '{numberHouse}' уже существует на указанной улице (Id улицы: {idStreet}).");
                }
            }
        }

        private async Task<bool> HouseWithSameStreetExistsAsync(int numberHouse, object idStreet, object currentEntityId)
        {
            var repository = _repositoryFactory.GetRepository("house");

            var query = "SELECT COUNT(1) FROM House WHERE numberHouse = @numberHouse AND id_street = @idStreet";

            if (currentEntityId != null)
            {
                query += " AND Id != @currentEntityId";
            }

            var parameters = new Dictionary<string, object>
            {
                { "@numberHouse", numberHouse },
                { "@idStreet", idStreet }
            };

            if (currentEntityId != null)
            {
                parameters.Add("@currentEntityId", currentEntityId);
            }

            return await repository.ExistsWithQueryAsync(query, parameters);
        }
    }
}
