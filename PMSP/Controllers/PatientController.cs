using PMS.Models;
using PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMS.Filters;

namespace PMS.Controllers
{
    //[Authorize]
    [CustomAuthorize]
    public class PatientController : Controller
    {
        PatientRepo _patientRepo = new PatientRepo();
        UserRepo _userRepo = new UserRepo();
        PhysicianRepo _phyRepo = new PhysicianRepo();

        [CustomAuthorize(permissionEntity = "Patients")]
        public ActionResult Index()
        {
            var Patients = _patientRepo.GetPatients();
            ViewData["Patients"] = Patients; // When we use view data then we don't need to cast its type on the view side. While when we don't need type casting in case of viewbag 
            ViewBag.Patients = Patients;   // its a container that takes the value from controller and passes it to view, it can contain any string or object
            return View(Patients);

        }

        [CustomAuthorize(permissionEntity = "Patients")]
        public ActionResult ViewDetails(int id)
        {
            ViewBag.PatientDetail = _patientRepo.GetPatient(id);
            var pateitnPhysicians = _patientRepo.GetPatientPhysicians(id);
            int[] pIds = pateitnPhysicians.Select(p => p.PhysicianId).ToArray();
            ViewBag.PhysiciansList = _phyRepo.GetPhysicians().Where(p => !pIds.Contains(p.Id)).ToList();
            ViewBag.PPhysicians = pateitnPhysicians;
            return View();
        }
       

        // GET: Patient
        [CustomAuthorize(permissionEntity = "Patients")]
        public ActionResult Add()
        {

            PMSEntities1 db = new PMSEntities1();
            var Getlist = db.Roles.ToList();
            SelectList list = new SelectList(Getlist, "Id", "Title");
            ViewBag.RolesList = list;
            return View();
        }

        [CustomAuthorize(permissionEntity = "Patients")]  //authorize
        [ValidateAntiForgeryToken] //chk that request is secure or not and comming from our own application,
        [HttpPost]
        public ActionResult Save(User patient)
        {
            if( (IsEmailValid(patient.Email)) || (IsValid(patient.UserName)))
            {
                ViewBag.Message = "User name or Email already exists, please try another user name!";
                PMSEntities1 db = new PMSEntities1();
                return View("Add");

            }
            
            else
            { 
            int userId = _userRepo.Save(patient);
            patient.Patients.First().UserId = userId;
            int pateintId = _patientRepo.Save(patient.Patients.First());

            patient.Patients.First().PatientPhysicians.First().PatientId = pateintId;
            _patientRepo.SavePatientPhysician(patient.Patients.First().PatientPhysicians.First());
             return RedirectToAction("Index", "Patient", new { });
            }

        }

        [CustomAuthorize(permissionEntity = "Patients")]
        public ActionResult Edit(int id)
        {
            var patient = _patientRepo.GetPatient(id);
            return View("Add", patient);
        }

        [CustomAuthorize(permissionEntity = "Patients")]
        public ActionResult Delete(int id)
        {
            bool deleted = _patientRepo.Delete(id);
            return RedirectToAction("Index", "Patient", new { });
        }

        [HttpPost]
        public bool IsValid(string UserName)
        {
            return _userRepo.ValidateUser(UserName);
        }

        [HttpPost]
        public bool IsEmailValid(string Email)
        {
            return _patientRepo.ValidateEmail(Email);
        }
    }
}