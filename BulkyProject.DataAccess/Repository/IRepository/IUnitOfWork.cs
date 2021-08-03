using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
   public interface IUnitOfWork : IDisposable
    {
        public ICategoryRepository Category { get; }
        public ICoverTypeRepository CoverType{ get; }
        public IProductRepository Product{ get; }
        public ICompanyRepository Company{ get; }
        public IApplicationUserRepository ApplicationUser{ get; }
        public IShoppingCartRepository ShoppingCart{ get; }
        public IOrderHeaderRepository OrderHeader{ get; }
        public IOrderDetailsRepository OrderDetails{ get; }
        void Save();
    }
}
