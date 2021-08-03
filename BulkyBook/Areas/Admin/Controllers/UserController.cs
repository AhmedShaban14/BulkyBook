using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Lock(string id)
        {
            if (id == null)
                return NotFound();
            _unitOfWork.ApplicationUser.Lock(id);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnLock(string id)
        {
            if (id == null)
                return NotFound();
            _unitOfWork.ApplicationUser.UnLock(id);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        #region Api's Calls 

        public IActionResult GetAllUsers()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var loggedInUserId = user.Value;

            var usersList = _unitOfWork.ApplicationUser.GetAll(x => x.Id != loggedInUserId,includeProperties:"Company");
            return Json(new { data = usersList });
        }

        [HttpDelete]
        public IActionResult DeleteUser(string id )
        {

            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(x=>x.Id == id);
            if (user == null)
                return Json(new { success="false",message="Deleted failed !!"});
            _unitOfWork.ApplicationUser.Delete(user);
            _unitOfWork.Save();
            return Json(new { success="true",message="Deleted Successfully !!"});
        }
        #endregion

    }
}
