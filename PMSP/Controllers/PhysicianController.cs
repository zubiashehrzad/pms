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
        public ActionResult Index()
        {

            var Physicians = _physicianRepo.GetPhysicians();
            ViewData["Physicians"] = Physicians;
            ViewBag.Physicians = Physicians;
            return View(Physicians);

        }

        public ActionResult Add()
        {
            PMSEntities1 db = new PMSEntities1();
            var Getlist = db.Roles.ToList();
            SelectList list = new SelectList(Getlist, "Id", "Title");
            ViewBag.RolesList = list;
            return View();
        }

        public ActionResult Save(User physician)
        {
            int userId = _userRepo.Save(physician);
            physician.Physicians.First().UserId = userId;
            _physicianRepo.Save(physician.Physicians.First());
            return RedirectToAction("Index", "Physician", new { });
        }

        public ActionResult Edit(int id)
        {
            var physician = _physicianRepo.GetPhysician(id);
            return View("Add", physician);
        }

        public ActionResult Delete(int id)
        {
            bool deleted = _physicianRepo.Delete(id);
            return RedirectToAction("Index", "Physician", new { });
        }
        public ActionResult ViewDetails(int id)
        {
            ViewBag.PhysicianDetail = _physicianRepo.GetPhysician(id);
            return View();

        }
    }
}