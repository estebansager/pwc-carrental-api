using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.DB.Repositories
{
    public class CarRentalDbRepository<T> : ICarRentalDbRepository<T> where T : class
    {

        protected readonly DbContext _context;

        public readonly DbSet<T> _dbSet;

        public CarRentalDbRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

    }
}
