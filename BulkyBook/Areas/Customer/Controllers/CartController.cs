using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //user must logged to see his shopping Carts : 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM scvm = new ShoppingCartVM
            {
                OrderHeader = new Models.OrderHeader(),
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == user.Value, includeProperties: "Product")
            };
            scvm.OrderHeader.OrderTotal = 0;
            foreach (var shoppingCart in scvm.ShoppingCarts)
            {

                scvm.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
            }

            return View(scvm);
        }
        public IActionResult Summary()
        {
            //user must logged to see his shopping Carts : 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM scvm = new ShoppingCartVM
            {
                OrderHeader = new Models.OrderHeader(),
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == user.Value, includeProperties: "Product")
            };
            scvm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == user.Value);
            scvm.OrderHeader.Name = scvm.OrderHeader.ApplicationUser.Name;
            scvm.OrderHeader.PhoneNumber = scvm.OrderHeader.ApplicationUser.PhoneNumber;
            scvm.OrderHeader.City = scvm.OrderHeader.ApplicationUser.City;
            scvm.OrderHeader.State = scvm.OrderHeader.ApplicationUser.State;
            scvm.OrderHeader.PostalCode = scvm.OrderHeader.ApplicationUser.PostalCode;
            scvm.OrderHeader.OrderDate= DateTime.Now;

            scvm.OrderHeader.OrderTotal = 0;
            foreach (var shoppingCart in scvm.ShoppingCarts)
            {
                scvm.OrderHeader.OrderTotal += (shoppingCart.Product.Price * shoppingCart.Count);
            }
            return View(scvm);
        }

        [HttpPost]
        public IActionResult Summary(ShoppingCartVM scvm)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                
                scvm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == user.Value);
                scvm.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == user.Value,includeProperties:"Product");

                scvm.OrderHeader.ApplicationUserId = user.Value;
                scvm.OrderHeader.OrderDate = DateTime.Now;
                scvm.OrderHeader.OrderStatus =SD.OrderStatusPending;
                _unitOfWork.OrderHeader.Create(scvm.OrderHeader);
                _unitOfWork.Save();
                

                foreach (var item in scvm.ShoppingCarts)
                {
                    var orderDetails = new OrderDetails
                    {
                        OrderId = scvm.OrderHeader.Id,
                        ProductId=item.ProductId,
                        Count = item.Count,
                        Price = item.Product.Price
                    };
                    scvm.OrderHeader.OrderTotal += orderDetails.Count * orderDetails.Price;
                    _unitOfWork.OrderDetails.Create(orderDetails);
                }
                _unitOfWork.ShoppingCart.DeleteRange(scvm.ShoppingCarts);
                HttpContext.Session.SetInt32(SD.ShoopingCart, 0);
                _unitOfWork.Save();
            }
            else
            {
                return View(scvm);
            }
        
            return RedirectToAction("ConfirmOrder",new { orderHeaderId=scvm.OrderHeader.Id});
        }

        public IActionResult ConfirmOrder(int orderHeaderId)
        {
            return View(orderHeaderId);
        }
        public IActionResult Plus(int shoppingCartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == shoppingCartId, includeProperties: "Product");
            shoppingCart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int shoppingCartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == shoppingCartId, includeProperties: "Product");
            if (shoppingCart.Count == 1)
            {
                _unitOfWork.ShoppingCart.Delete(shoppingCart);
                _unitOfWork.Save();
                var cnt = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ShoopingCart, cnt);
            }
            else
            {
                shoppingCart.Count -= 1;
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int shoppingCartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == shoppingCartId, includeProperties: "Product");
                _unitOfWork.ShoppingCart.Delete(shoppingCart);
            _unitOfWork.Save();
            var cnt = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ShoopingCart, cnt);
            return RedirectToAction(nameof(Index));
        }
    }
} 

     