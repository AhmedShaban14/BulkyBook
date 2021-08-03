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

    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int ? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
                return View(coverType);
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null)
                return NotFound();
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    //Create
                    _unitOfWork.CoverType.Create(coverType);
                }
                else
                {
                    //Update
                    _unitOfWork.CoverType.Update(coverType);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        #region Api's Call 
        public IActionResult GetAllCoverType()
        {
            var coverType = _unitOfWork.CoverType.GetAll();
            return Json(new { data = coverType });
        }
        [HttpDelete]
        public IActionResult DeleteCoverType(int id)
        {
            var coverType = _unitOfWork.CoverType.Get(id);
            if (coverType == null)
                return Json(new { success = "false", message = "failed during delete ! " });
            _unitOfWork.CoverType.Delete(coverType);
            _unitOfWork.Save();
            return Json(new { success = "true", message = "deleted successfully !" });
        }
        #endregion
    }
}
