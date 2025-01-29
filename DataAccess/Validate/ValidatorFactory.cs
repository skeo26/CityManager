using CityManagementApp.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess.Validate
{
    public class ValidatorFactory
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ValidatorFactory(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public IValidator GetValidator(string tableName)
        {
            return tableName switch
            {
                "city" => new CityValidator(_repositoryFactory),
                "district" => new DistrictValidator(_repositoryFactory),
                "street" => new StreetValidator(_repositoryFactory),
                "house" => new HouseValidator(_repositoryFactory),
                "resident" => new ResidentValidator(_repositoryFactory),
                "registration" => new RegistrationValidator(_repositoryFactory),
                _ => throw new NotSupportedException($"Валидатор для таблицы {tableName} не найден.")
            }; ;
        }
    }
}
