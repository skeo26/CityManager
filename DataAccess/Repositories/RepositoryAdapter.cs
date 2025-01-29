using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.DataAccess.Repositories
{
    public class RepositoryAdapter<T> : BaseRepository<object>
    {
        private readonly BaseRepository<T> _innerRepository;

        public RepositoryAdapter(BaseRepository<T> innerRepository) : base(innerRepository.ConnectionString)
        {
            _innerRepository = innerRepository;
        }

        public override async Task<List<object>> GetAllAsync()
        {
            var result = await _innerRepository.GetAllAsync();
            return result.Cast<object>().ToList();
        }

        public override Task AddAsync(object entity)
        {
            return _innerRepository.AddAsync((T)entity);
        }

        public override Task UpdateAsync(object entity)
        {
            return _innerRepository.UpdateAsync((T)entity);
        }

        public override Task DeleteAsync(int id)
        {
            return _innerRepository.DeleteAsync(id);
        }

        public override Task<int> GetNextIdAsync(string name)
        {
            return _innerRepository.GetNextIdAsync(name);
        }
    }
}
