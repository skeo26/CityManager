using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess.Repositories
{
    public interface IRepositoryFactory
    {
        BaseRepository<object> GetRepository(string tableName);
        object CreateNewItemForTable(string tableName);
        List<string> GetTableNames();
    }
}
