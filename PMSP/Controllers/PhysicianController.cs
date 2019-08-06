using PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMS.Models;
using PMS.Filters;



namespace PMS.Controllers
{
    [CustomAuthorize]
    public class PhysicianController : Controller
    {
        PhysicianRepo _physicianRepo = new PhysicianRepo();
        UserRepo _userRepo = new UserRepo();

        // GET: Physician
        [CustomAuthorize(permissionEntity = "Physicians")]
        public ActionResult Index()
        {
            var Physicians = _physicianRepo.GetPhysicians();
            ViewData["Physicians"] = Physicians;
            ViewBag.Physicians = Physicians;
            return View(Physicians);
        }

        [CustomAuthorize(permissionEntity = "Physicians")]
        public ActionResult Add()
        {
            PMSEntities1 db = new PMSEntities1();
            var Getlist = db.Roles.ToList();
            SelectList list = new SelectList(Getlist, "Id", "Title");
            ViewBag.RolesList = list;
            return View();
        }

        [CustomAuthorize(permissionEntity = "Physicians")]
        [ValidateAntiForgeryToken] //chk that request is secure or not and comming from our own application,
        [HttpPost]
        public ActionResult Save(User physician)
        {
            if ((IsEmailValid(physician.Email)) || (IsValid(physician.UserName)))
            {
                ViewBag.Message = "User name or Email already exists, please try another user name!";
                PMSEntities1 db = new PMSEntities1();
                return View("Add");

            }
            else
            { 
            int userId = _userRepo.Save(physician);
            physician.Physicians.First().UserId = userId;
            _physicianRepo.Save(physician.Physicians.First());
            return RedirectToAction("Index", "Physician", new { });
            }
        }

        [CustomAuthorize(permissionEntity = "Physicians")]
        public ActionResult Edit(int id)
        {
            var physician = _physicianRepo.GetPhysician(id);
            return View("Add", physician);
        }

        [CustomAuthorize(permissionEntity = "Physicians")]
        public ActionResult Delete(int id)
        {
            bool deleted = _physicianRepo.Delete(id);
            return RedirectToAction("Index", "Physician", new { });
        }

        [CustomAuthorize(permissionEntity = "Physicians")]
        public ActionResult ViewDetails(int id)
        {
            ViewBag.PhysicianDetail = _physicianRepo.GetPhysician(id);
            return View();

        }

        [HttpPost]
        public bool IsValid(string UserName)
        {
            return _userRepo.ValidateUser(UserName);
        }

        [HttpPost]
        public bool IsEmailValid(string Email)
        {
            return _physicianRepo.ValidateEmail(Email);
        }
    }
}