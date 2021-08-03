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
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee) ]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                //Create : 
                return View(company);
            }
            else
            {
                company = _unitOfWork.Company.Get(id.GetValueOrDefault());
                if (company == null)
                    return NotFound();
            }
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    //Create : 
                    _unitOfWork.Company.Create(company);
                }
                else
                {
                    //Edit : 
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        #region Api Calls 
        public IActionResult GetAllCompany()
        {
            var cat = _unitOfWork.Company.GetAll();
            return Json(new { data = cat });
        }

        public IActionResult DeleteCompany(int id)
        {
            var catDb = _unitOfWork.Company.Get(id);
            if (catDb == null)
                return Json(new { success = "false", message = "Failed during delete Company" });
            _unitOfWork.Company.Delete(catDb);
            _unitOfWork.Save();
            return Json(new { success = "true", message = "Company deleted successfully" });
        }
        #endregion
    }
}
