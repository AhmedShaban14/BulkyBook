using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Models.ViewModel
{
   public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<CoverType> CoverTypes{ get; set; }
    }
}
