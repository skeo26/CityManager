using CityManagementApp.BL;
using CityManagementApp.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess.Validate
{
    public class DistrictValidator : IValidator
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public DistrictValidator(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<string>> ValidateAsync(object entity)
        {
            var errors = new List<string>();
            var idProperty = entity.GetType().GetProperty("Id");
            var entityId = idProperty?.GetValue(entity);
            var nameDistrict = entity.GetType().GetProperty("nameDistrict")?.GetValue(entity)?.ToString();
            var idCity = entity.GetType().GetProperty("id_city")?.GetValue(entity);

            ValidateName(nameDistrict, errors);
            await ValidateCityExistsAsync(idCity, errors);
            await ValidateUniqueDistrictInCityAsync(nameDistrict, idCity, errors, entityId);

            return errors;
        }

        private void ValidateName(string nameDistrict, List<string> errors)
        {
            if (string.IsNullOrEmpty(nameDistrict))
            {
                errors.Add("Название района не может быть пустым");
                return;
            }

            if (!Regex.IsMatch(nameDistrict, @"^[а-яА-ЯёЁ\s]+$"))
            {
                errors.Add("Название района должно содержать только русские буквы.");
            }
        }

        private async Task ValidateCityExistsAsync(object idCity, List<string> errors)
        {
            if (idCity == null || !await _repositoryFactory.GetRepository("city").ExistsAsync("city", "Id", idCity))
            {
                errors.Add($"Город с Id '{idCity}' не существует.");
            }
        }

        private async Task ValidateUniqueDistrictInCityAsync(string nameDistrict, object idCity, List<string> errors, object entityId)
        {
            if (await DistrictWithSameCityExistsAsync(nameDistrict, idCity, entityId))
            {
                errors.Add($"Район с названием '{nameDistrict}' уже существует в указанном городе (Id города: {idCity}).");
            }
        }

        private async Task<bool> DistrictWithSameCityExistsAsync(string nameDistrict, object idCity, object currentEntityId)
        {
            var repository = _repositoryFactory.GetRepository("district");

            var query = "SELECT COUNT(1) FROM District WHERE nameDistrict = @nameDistrict AND id_city = @idCity";
            if (currentEntityId != null)
            {
                query += " AND Id != @currentEntityId";
            }

            var parameters = new Dictionary<string, object>
            {
                { "@nameDistrict", nameDistrict },
                { "@idCity", idCity }
            };

            if (currentEntityId != null)
            {
                parameters.Add("@currentEntityId", currentEntityId);
            }

            return await repository.ExistsWithQueryAsync(query, parameters);
        }
    }
}

