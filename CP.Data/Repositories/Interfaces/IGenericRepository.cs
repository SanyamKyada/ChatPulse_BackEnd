using System.Linq.Expressions;

namespace CP.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetFilteredData(Func<T, bool> filter);
        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        T GetById(int id);
        void Insert(T t);
        void Update(T t);
        void Delete(T t);
        Task Save();
    }
}
