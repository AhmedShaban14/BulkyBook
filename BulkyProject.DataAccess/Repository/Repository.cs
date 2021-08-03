using BulkyBook.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> table;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            table = _db.Set<T>();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string includeProperties = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = table;
            //Divide to 3 steps : 1-filter , 2- include , 3- orderBy 

            //1-Filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            //2-includeProperites
            if (includeProperties != null)
            {
                foreach (var include in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include);
                }
            }

            //orderBy:
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = table;
            //Divide to 2 steps : 1-filter , 2- include  

            //1-Filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            //2-includeProperites
            if (includeProperties != null)
            {
                foreach (var include in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include);
                }
            }

            return query.FirstOrDefault();
        }



        public T Get(int id)
        {
            return table.Find(id);
        }

        public void Create(T entity)
        {
            table.Add(entity);
        }

        public void Delete(int id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }

        public void Delete(T entity)
        {
            table.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            table.RemoveRange(entity);
        }
    }
}
