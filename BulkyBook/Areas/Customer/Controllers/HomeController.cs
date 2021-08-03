using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BulkyBook.Utility;
using System.Security.Claims;

namespace BulkyBook.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            //get session:

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (user != null)
            {
                var cnt = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == user.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.ShoopingCart, cnt);
            }
            return View(productList);
        }

        [HttpGet]
        public IActionResult Details(int productId)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == productId, includeProperties: "Category,CoverType");

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart shoppingCart)
        {

            if (ModelState.IsValid)
            {
                //save : 
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = user.Value;

                var shoppingDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ApplicationUserId == shoppingCart.ApplicationUserId
                                                                                  && x.ProductId == shoppingCart.ProductId);
                if (shoppingDb != null)
                {
                    //Increase Count: 
                    shoppingDb.Count += shoppingCart.Count;
                }
                else
                {
                    //Create : 
                    _unitOfWork.ShoppingCart.Create(shoppingCart);
                }

                //Session : 
                var cnt = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ShoopingCart, cnt);
                _unitOfWork.Save();
            }
            else
            {
                var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == shoppingCart.ProductId, includeProperties: "Category,CoverType");

                 shoppingCart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };
                return View(shoppingCart);
            }
            return RedirectToAction(nameof(Index));

        }
        //public IActionResult AddToCart(int productId)
        //{
        //    List<int> sessionList = new List<int>();
        //    if (String.IsNullOrEmpty(HttpContext.Session.GetString(SD.ShoopingCart)))
        //    {
        //        sessionList.Add(productId);
        //        HttpContext.Session.SetObj(SD.ShoopingCart, sessionList);
        //    }
        //    else
        //    {
        //        sessionList = HttpContext.Session.GetObj<List<int>>(SD.ShoopingCart);
        //        if (!sessionList.Contains(productId))
        //        {
        //            sessionList.Add(productId);
        //            HttpContext.Session.SetObj(SD.ShoopingCart, sessionList);
        //        }
        //    }
        //    return RedirectToAction(nameof(Index));

        //}
    }
}
