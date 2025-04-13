using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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


        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

    }
}
