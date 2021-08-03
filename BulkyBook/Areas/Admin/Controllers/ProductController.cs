using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hosting;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hosting )
        {
            _unitOfWork = unitOfWork;
            _hosting = hosting;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM pvm = new ProductVM
            {
                Product = new Models.Product(),
                Categories = _unitOfWork.Category.GetAll(),
                CoverTypes = _unitOfWork.CoverType.GetAll()
            };

            if (id != null)
            {
                //Update
                pvm.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
                if (pvm.Product == null)
                    return NotFound();
            }
            return View(pvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM pvm)
        {
            if (ModelState.IsValid)
            {
                var fileName = @"lawn_leaf.jpg";
                if (pvm.Product.Id == 0)
                {
                    //Create 
                    if (pvm.Product.File != null)
                    {
                        //var defultImage = @"images";
                        //Save New Images : 
                        fileName = Guid.NewGuid() + pvm.Product.File.FileName;
                        var upload = Path.Combine(_hosting.WebRootPath, @"images");
                        var fullPath = Path.Combine(upload, fileName);
                        FileStream s = new FileStream(fullPath, FileMode.Create);
                        pvm.Product.File.CopyTo(s);
                    }
                    pvm.Product.ImageUrl = fileName;
                    _unitOfWork.Product.Create(pvm.Product);
                }
                else
                {
                    
                    //Update : 
                    if (pvm.Product.File != null)
                    {
                        //user update photo
                        //Save New Images : 
                        fileName = Guid.NewGuid() + pvm.Product.File.FileName;
                        var upload = Path.Combine(_hosting.WebRootPath, @"images");
                        var fullPath = Path.Combine(upload, fileName);
                        FileStream s = new FileStream(fullPath, FileMode.Create);
                        pvm.Product.File.CopyTo(s);
                        //Delete Old Photo : 
                        var oldfullPath = Path.Combine(upload, pvm.Product.ImageUrl);
                        if (System.IO.File.Exists(oldfullPath))
                        {
                            System.IO.File.Delete(oldfullPath);
                        }
                        pvm.Product.ImageUrl = fileName;
                    }
                    _unitOfWork.Product.Update(pvm.Product);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                pvm.Categories = _unitOfWork.Category.GetAll();
                pvm.CoverTypes = _unitOfWork.CoverType.GetAll();
            }
            return View(pvm);
        }

        #region Api's Call
        public IActionResult GetAllProduct()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult DeleteProduct(int id)
        {
            var productDb = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id, includeProperties: "Category,CoverType");
            if (productDb == null)
                return Json(new { success = "false", message = "failed during deleted" });

            var upload = Path.Combine(_hosting.WebRootPath, @"images");
            //Delete Old Photo : 
            var oldfullPath = Path.Combine(upload, productDb.ImageUrl);
            if (System.IO.File.Exists(oldfullPath))
            {
                System.IO.File.Delete(oldfullPath);
            }
            _unitOfWork.Product.Delete(productDb);
            _unitOfWork.Save();
            return Json(new { success = "true", message = "deleted done successfully" });
        }
        #endregion
    }
}
