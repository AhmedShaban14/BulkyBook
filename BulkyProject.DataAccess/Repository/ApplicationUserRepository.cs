using BulkyBook.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Lock(string userId)
        {
            var user = _db.ApplicationUser.Find(userId);
            user.LockoutEnd = DateTime.Now.AddHours(4);
        }

        public void UnLock(string userId)
        {
            var user = _db.ApplicationUser.Find(userId);
            user.LockoutEnd = DateTime.Now;
        }
    }
}
