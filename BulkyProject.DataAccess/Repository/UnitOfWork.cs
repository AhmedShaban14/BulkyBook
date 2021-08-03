using BulkyBook.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;


        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(db);
            CoverType = new CoverTypeRepository(db);
            Product = new ProductRepository(db);
            Company= new CompanyRepository(db);
            ApplicationUser= new ApplicationUserRepository(db);
            ShoppingCart= new ShoppingCartRepository(db);
            OrderHeader= new OrderHeaderRepository(db);
            OrderDetails= new OrderDetailsRepository(db);
        }
        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType{ get; private set; }
        public IProductRepository Product{ get; private set; }
        public ICompanyRepository Company{ get; private set; }
        public IApplicationUserRepository ApplicationUser{ get; private set; }
        public IShoppingCartRepository ShoppingCart{ get; private set; }
        public IOrderHeaderRepository OrderHeader{ get; private set; }
        public IOrderDetailsRepository OrderDetails{ get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
