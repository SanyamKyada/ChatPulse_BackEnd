using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CP.Data.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly CPDatabaseContext _context;
        private readonly DbSet<T> entities;
        public GenericRepository(CPDatabaseContext context)
        {
            _context = context;
            entities = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public IEnumerable<T> GetFilteredData(Func<T, bool> filter)
        {
            return entities.Where(filter);
        }


        public T GetById(int id)
        {
            return entities.Find(id);
        }

        public void Delete(T t)
        {
            entities.Remove(t);
        }

        public void Insert(T t)
        {
            entities.Add(t);
        }

        public void Save()
        {
            _context.SaveChangesAsync();
        }

        public void Update(T t)
        {
            entities.Update(t);
        }

        public virtual IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
    }
}
