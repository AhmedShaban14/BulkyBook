using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category cat = new Category();
            if (id == null)
            {
                //Create : 
                return View(cat);
            }
            else
            {
                cat = _unitOfWork.Category.Get(id.GetValueOrDefault());
                if (cat == null)
                    return NotFound();
            }
            return View(cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category cat)
        {
            if (ModelState.IsValid)
            {
                if (cat.Id == 0)
                {
                    //Create : 
                    _unitOfWork.Category.Create(cat);
                }
                else
                {
                    //Edit : 
                    _unitOfWork.Category.Update(cat);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(cat);
        }

        #region Api Calls 
        public IActionResult GetAllCategory()
        {
            var cat = _unitOfWork.Category.GetAll();
            return Json(new { data = cat });
        }

        public IActionResult DeleteCategory(int id)
        {
            var catDb = _unitOfWork.Category.Get(id);
            if (catDb == null)
                return Json(new { success = "false", message = "Failed during delete category" });
            _unitOfWork.Category.Delete(catDb);
            _unitOfWork.Save();
            return Json(new { success = "true", message = "Category deleted successfully" });
        }
        #endregion
    }
}
