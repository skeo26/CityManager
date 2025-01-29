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
    public class CityValidator : IValidator
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private const int smallestPopulation = 800;
        private const int largestPopulation = 26000000;

        public CityValidator(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<string>> ValidateAsync(object entity)
        {
            var errors = new List<string>();

            var nameCity = entity.GetType().GetProperty("nameCity")?.GetValue(entity)?.ToString();
            var population = entity.GetType().GetProperty("population")?.GetValue(entity)?.ToString();

            ValidateName(nameCity, errors);
            await ValidateUniqueNameAsync(nameCity, errors);
            ValidatePopulation(population, errors);

            return errors;
        }

        private static void ValidateName(string nameCity, List<string> errors)
        {
            if (string.IsNullOrEmpty(nameCity))
            {
                errors.Add("Название города не может быть пустым");
                return;
            }

            if (!Regex.IsMatch(nameCity, @"^[а-яА-ЯёЁ\s]+$"))
            {
                errors.Add("Название города должно содержать только русские буквы.");
            }
        }

        private async Task ValidateUniqueNameAsync(string nameCity, List<string> errors)
        {
            if (await _repositoryFactory.GetRepository("city").ExistsAsync("city", "nameCity", nameCity))
            {
                errors.Add($"Город с названием '{nameCity}' уже существует.");
            }
        }

        private static void ValidatePopulation(string population, List<string> errors)
        {
            if (!int.TryParse(population, out int populationNumber))
            {
                errors.Add("Население некорректно!");
                return;
            }

            if (populationNumber < smallestPopulation || populationNumber > largestPopulation)
            {
                errors.Add("Слишком большое или маленькое население!");
            }
        }
    }

}
