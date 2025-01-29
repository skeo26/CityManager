using CityManagementApp.BL;
using CityManagementApp.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess.Validate
{
    public class RegistrationValidator : IValidator
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public RegistrationValidator(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<string>> ValidateAsync(object entity)
        {
            var errors = new List<string>();

            var idProperty = entity.GetType().GetProperty("Id");
            var entityId = idProperty?.GetValue(entity);
            var dateRegistration = entity.GetType().GetProperty("dateRegistration")?.GetValue(entity)?.ToString();
            var dataDeregistration = entity.GetType().GetProperty("dataDeregistration")?.GetValue(entity)?.ToString();
            var idResident = entity.GetType().GetProperty("id_resident")?.GetValue(entity);
            var idHouse = entity.GetType().GetProperty("id_house")?.GetValue(entity);

            ValidateDateRegistration(dateRegistration, errors);
            ValidateDateDeregistration(dateRegistration, dataDeregistration, errors);
            await ValidateResidentExistsAsync(idResident, errors);
            await ValidateHouseExistsAsync(idHouse, errors);
            await ValidateResidentAlreadyRegisteredAsync(idResident, idHouse, dateRegistration, errors, entityId);

            return errors;
        }


        private void ValidateDateRegistration(string dateRegistration, List<string> errors)
        {
            if (string.IsNullOrEmpty(dateRegistration))
            {
                errors.Add("Дата регистрации не может быть пустой");
                return;
            }

            if (!DateTime.TryParse(dateRegistration, out _))
            {
                errors.Add("Дата регистрации некорректна");
            }
        }

        private void ValidateDateDeregistration(string dateRegistration, string dataDeregistration, List<string> errors)
        {
            if (string.IsNullOrEmpty(dataDeregistration))
            {
                return;
            }

            if (!DateTime.TryParse(dataDeregistration, out var parsedDataDeregistration))
            {
                errors.Add("Дата дерегистрации некорректна");
                return;
            }

            if (DateTime.TryParse(dateRegistration, out var parsedDateRegistration) && parsedDataDeregistration <= parsedDateRegistration)
            {
                errors.Add("Дата дерегистрации должна быть позже даты регистрации.");
            }
        }

        private async Task ValidateResidentExistsAsync(object idResident, List<string> errors)
        {
            if (idResident == null || !await _repositoryFactory.GetRepository("resident").ExistsAsync("resident", "Id", idResident))
            {
                errors.Add($"Житель с Id '{idResident}' не существует.");
            }
        }

        private async Task ValidateHouseExistsAsync(object idHouse, List<string> errors)
        {
            if (idHouse == null || !await _repositoryFactory.GetRepository("house").ExistsAsync("house", "Id", idHouse))
            {
                errors.Add($"Дом с Id '{idHouse}' не существует.");
            }
        }

        private async Task ValidateResidentAlreadyRegisteredAsync(object idResident, object idHouse, string dateRegistration, List<string> errors, object currentEntityId)
        {
            if (idResident != null && idHouse != null && DateTime.TryParse(dateRegistration, out var parsedDateRegistration))
            {
                if (await ResidentAlreadyRegisteredAsync(idResident, idHouse, parsedDateRegistration, currentEntityId))
                {
                    errors.Add($"Житель с Id '{idResident}' уже зарегистрирован в другом доме.");
                }
            }
        }

        private async Task<bool> ResidentAlreadyRegisteredAsync(object idResident, object idHouse, DateTime dateRegistration, object currentEntityId)
        {
            var repository = _repositoryFactory.GetRepository("registration");

            var query = @"
                SELECT 1 
                FROM Registration 
                WHERE id_resident = @idResident 
                  AND id_house != @idHouse 
                  AND (@dateRegistration BETWEEN dateRegistration AND COALESCE(dataDeregistration, '9999-12-31'))
            ";

            if (currentEntityId != null)
            {
                query += " AND Id != @currentEntityId";
            }

            var parameters = new Dictionary<string, object>
            {
                { "@idResident", idResident },
                { "@idHouse", idHouse },
                { "@dateRegistration", dateRegistration }
            };

            if (currentEntityId != null)
            {
                parameters.Add("@currentEntityId", currentEntityId);
            }

            return await repository.ExistsWithQueryAsync(query, parameters);
        }
    }
}
