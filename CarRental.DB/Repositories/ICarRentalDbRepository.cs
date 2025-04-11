using System.Linq.Expressions;

namespace CarRental.DB.Repositories
{
    public interface ICarRentalDbRepository<T> where T : class
    {

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
