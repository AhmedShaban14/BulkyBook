using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Models.ViewModel
{
   public class ShoppingCartVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
    }
}
