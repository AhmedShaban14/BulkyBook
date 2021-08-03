using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {

        T GetFirstOrDefault(
         Expression<Func<T, bool>> filter = null,
         string includeProperties = null
         );

        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null
            );
     
        T Get(int id);
        void Create(T entity);
        void Delete(int id);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entity);


    }
}
