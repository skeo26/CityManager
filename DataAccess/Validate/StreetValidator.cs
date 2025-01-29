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
    public class StreetValidator : IValidator
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private const double smallestSizeStreet = 3;
        private const double largestSizeStreet = 2000;

        public StreetValidator(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<string>> ValidateAsync(object entity)
        {
            var errors = new List<string>();
            var idProperty = entity.GetType().GetProperty("Id");
            var entityId = idProperty?.GetValue(entity);
            var nameStreet = entity.GetType().GetProperty("nameStreet")?.GetValue(entity)?.ToString();
            var sizeStreet = entity.GetType().GetProperty("sizeStreet")?.GetValue(entity)?.ToString();
            var idDistrict = entity.GetType().GetProperty("id_district")?.GetValue(entity);

            ValidateNameStreet(nameStreet, errors);
            await ValidateDistrictExistsAsync(idDistrict, errors);
            await ValidateUniqueStreetInDistrictAsync(nameStreet, idDistrict, errors, entityId);
            ValidateSizeStreet(sizeStreet, errors);

            return errors;
        }

        private void ValidateNameStreet(string nameStreet, List<string> errors)
        {
            if (string.IsNullOrEmpty(nameStreet))
            {
                errors.Add("Название улицы не может быть пустым");
                return;
            }

            if (!Regex.IsMatch(nameStreet, @"^[а-яА-ЯёЁ\s]+$"))
            {
                errors.Add("Название улицы должно содержать только русские буквы.");
            }
        }

        private async Task ValidateDistrictExistsAsync(object idDistrict, List<string> errors)
        {
            if (idDistrict == null || !await _repositoryFactory.GetRepository("district").ExistsAsync("district", "Id", idDistrict))
            {
                errors.Add($"Район с Id '{idDistrict}' не существует.");
            }
        }

        private async Task ValidateUniqueStreetInDistrictAsync(string nameStreet, object idDistrict, List<string> errors, object currentEntityId)
        {
            if (await StreetWithSameDistrictExistsAsync(nameStreet, idDistrict, currentEntityId))
            {
                errors.Add($"Улица с названием '{nameStreet}' уже существует в указанном районе (Id района: {idDistrict}).");
            }
        }

        private void ValidateSizeStreet(string sizeStreet, List<string> errors)
        {
            if (sizeStreet.Contains('.'))
            {
                errors.Add("Введите длину улицы через ','");
                return;
            }

            if (!double.TryParse(sizeStreet, out double size))
            {
                errors.Add("Длина улицы некорректна!");
                return;
            }

            if (size < smallestSizeStreet || size > largestSizeStreet)
            {
                errors.Add("Слишком большая или маленькая улица!");
            }
        }

        private async Task<bool> StreetWithSameDistrictExistsAsync(string nameStreet, object idDistrict, object currentEntityId)
        {
            var repository = _repositoryFactory.GetRepository("street");

            var query = "SELECT COUNT(1) FROM Street WHERE nameStreet = @nameStreet AND id_district = @idDistrict";

            if (currentEntityId != null)
            {
                query += " AND Id != @currentEntityId";
            }

            var parameters = new Dictionary<string, object>
            {
                { "@nameStreet", nameStreet },
                { "@idDistrict", idDistrict }
            };

            if (currentEntityId != null)
            {
                parameters.Add("@currentEntityId", currentEntityId);
            }

            return await repository.ExistsWithQueryAsync(query, parameters);
        }
    }
}

