using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess.Validate
{
    public interface IValidator
    {
        Task<List<string>> ValidateAsync(object entity);
    }
}
