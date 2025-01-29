using CityManagementApp.BL;
using CityManagementApp.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CityManagementApp.DataAccess.Validate
{
    public class ResidentValidator : IValidator
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private const int standartOfNumberPassport = 6;
        private const int standartOfNumberSeries = 4;

        public ResidentValidator(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<string>> ValidateAsync(object entity)
        {
            var errors = new List<string>();
            var idProperty = entity.GetType().GetProperty("Id");
            var entityId = idProperty?.GetValue(entity);
            var fullName = entity.GetType().GetProperty("fullName")?.GetValue(entity)?.ToString();
            var birthday = entity.GetType().GetProperty("birthday")?.GetValue(entity)?.ToString();
            var passportNumber = entity.GetType().GetProperty("passportNumber")?.GetValue(entity)?.ToString();
            var passportSeries = entity.GetType().GetProperty("passportSeries")?.GetValue(entity)?.ToString();
            var houseId = entity.GetType().GetProperty("id_house")?.GetValue(entity);

            ValidateFullName(fullName, errors);
            await ValidateUniqueFullNameAsync(fullName, errors);
            ValidateBirthday(birthday, errors);
            ValidatePassportNumber(passportNumber, errors);
            ValidatePassportSeries(passportSeries, errors);
            await ValidateHouseExistsAsync(houseId, errors);
            await ValidateResidentInHouseAsync(fullName, houseId, errors, entityId);

            return errors;
        }

        private void ValidateFullName(string fullName, List<string> errors)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                errors.Add("ФИО не может быть пустым");
                return;
            }

            if (!Regex.IsMatch(fullName, @"^[а-яА-ЯёЁ\s]+$"))
            {
                errors.Add("ФИО должно содержать только русские буквы и пробелы.");
            }
        }

        private async Task ValidateUniqueFullNameAsync(string fullName, List<string> errors)
        {
            if (await _repositoryFactory.GetRepository("resident").ExistsAsync("resident", "fullName", fullName))
            {
                errors.Add($"Человек с ФИО '{fullName}' уже существует.");
            }
        }

        private void ValidateBirthday(string birthday, List<string> errors)
        {
            if (string.IsNullOrEmpty(birthday))
            {
                errors.Add("Дата рождения не может быть пустой");
                return;
            }

            if (!DateTime.TryParse(birthday, out _))
            {
                errors.Add("Дата рождения некорректна");
            }
        }

        private void ValidatePassportNumber(string passportNumber, List<string> errors)
        {
            if (string.IsNullOrEmpty(passportNumber))
            {
                errors.Add("Номер паспорта не может быть пустым");
                return;
            }

            if (!int.TryParse(passportNumber, out _))
            {
                errors.Add("Номер паспорта некорректен!");
                return;
            }

            if (passportNumber.Length != standartOfNumberPassport)
            {
                errors.Add("Номер паспорта должен быть шестизначным числом");
            }
        }

        private void ValidatePassportSeries(string passportSeries, List<string> errors)
        {
            if (string.IsNullOrEmpty(passportSeries))
            {
                errors.Add("Серия паспорта не может быть пустой");
                return;
            }

            if (!int.TryParse(passportSeries, out _))
            {
                errors.Add("Серия паспорта некорректна!");
                return;
            }

            if (passportSeries.Length != standartOfNumberSeries)
            {
                errors.Add("Серия паспорта должна быть шестизначным числом");
            }
        }

        private async Task ValidateHouseExistsAsync(object houseId, List<string> errors)
        {
            if (houseId == null || !await _repositoryFactory.GetRepository("house").ExistsAsync("house", "Id", houseId))
            {
                errors.Add($"Дом с Id '{houseId}' не существует.");
            }
        }

        private async Task ValidateResidentInHouseAsync(string fullName, object houseId, List<string> errors, object currentEntityId)
        {
            if (await ResidentWithSameHouseExistsAsync(fullName, houseId, currentEntityId))
            {
                errors.Add($"Жилец с ФИО '{fullName}' уже существует в указанном доме (Id дома: {houseId}).");
            }
        }

        private async Task<bool> ResidentWithSameHouseExistsAsync(string fullName, object houseId, object currentEntityId)
        {
            var repository = _repositoryFactory.GetRepository("resident");

            var query = "SELECT COUNT(1) FROM Resident WHERE fullName = @fullName AND id_house = @houseId";

            if (currentEntityId != null)
            {
                query += " AND Id != @currentEntityId";
            }

            var parameters = new Dictionary<string, object>
            {
                { "@fullName", fullName },
                { "@houseId", houseId }
            };

            if (currentEntityId != null)
            {
                parameters.Add("@currentEntityId", currentEntityId);
            }

            return await repository.ExistsWithQueryAsync(query, parameters);
        }
    }
}
