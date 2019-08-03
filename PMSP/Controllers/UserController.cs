﻿using PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PMS.Models;
using PMS.Filters;



namespace PMS.Controllers
{
    //[CustomAuthorize]
    public class UserController : Controller
    {
        UserRepo _userRepo = new UserRepo();
        PhysicianRepo _physicianRepo = new PhysicianRepo();
        PatientRepo _patientRepo = new PatientRepo();

        #region User
        // GET: User
        [CustomAuthorize(permissionEntity = "Users")]
        public ActionResult Index()
        {
            var users = _userRepo.GetUsers();
            ViewData["users"] = users; // When we use view data then we don't need to cast its type on the view side. While when we don't need type casting in case of viewbag 
            ViewBag.users = users;   // its a container that takes the value from controller and passes it to view, it can contain any string or object
            return View(users);   // it ll map the model on view that we can use there to show/list the values from DB, can use one of them at a time
        }

        [CustomAuthorize(permissionEntity = "Users")]  //authorize
        [ValidateAntiForgeryToken] //chk that request is secure or not and comming from our own application,
        [HttpPost] // chk request type, all of these 3 are action filters
        public ActionResult Save(User user)
        {
            if (IsValid(user.UserName))
            {
                ViewBag.Message = "Username already taken, please try with a different username!";
                PMSEntities1 db = new PMSEntities1();
                var roles = db.Roles.Where(r => r.Title != "Patient" && r.Title != "Physician" && r.Title != "Admin").ToList();
                ViewBag.RolesList = roles;
                return View("Register");
            }
            else
            {
                var repoUser = _userRepo.Save(user);   //save
                return RedirectToAction("Index", "User", new { });  //view and show, redirect on this page
            }
        }

        // GET: Register
        [CustomAuthorize(permissionEntity = "Users")]
        public ActionResult Register()
        {
            PMSEntities1 db = new PMSEntities1();
            var roles = db.Roles.Where(r => r.Title != "Patient" && r.Title != "Physician" && r.Title != "Admin").ToList();
            //SelectList list = new SelectList(Getlist, "Id", "Title");
            ViewBag.RolesList = roles;
            return View();
        }

        [CustomAuthorize(permissionEntity = "Users")]
        public ActionResult ViewDetails(int id)
        {
            ViewBag.UserDetail = _userRepo.GetUser(id);
            return View();
        }

        [CustomAuthorize(permissionEntity = "Users")]
        public ActionResult Edit(int id)
        {
            var User = _userRepo.GetUser(id);
            PMSEntities1 db = new PMSEntities1();
            var roles = db.Roles.Where(r => r.Title != "Patient" && r.Title != "Physician").ToList();
            //SelectList list = new SelectList(Getlist, "Id", "Title");
            ViewBag.RolesList = roles;

            return View("Register", User);
        }

        [CustomAuthorize(permissionEntity = "Users")]
        public ActionResult Delete(int id)
        {
            bool deleted = _userRepo.Delete(id);
            return RedirectToAction("Index", "user", new { });
        }

        #endregion

        [CustomAuthorize]
        public ActionResult SavePermission(UserPermission userpermission)
        {

            var repoUser = _userRepo.SavePermission(userpermission);
            //save

            SetRolesAndModules();

            ViewBag.message = "Permissions Saved Successfully...!";
            return View("RegisterPermission");  //view and show, redirect on this page
                                                ////0r
                                                //  return RedirectToAction("~/User");
        }

        [CustomAuthorize(permissionEntity = "Permissions")]
        public ActionResult ViewPermissionDetails(int id)
        {
            ViewBag.PermissionDetail = _userRepo.GetUserP(id);

            return View();

        }

        [CustomAuthorize(permissionEntity = "Permissions")]
        public ActionResult IndexPermission()
        {
            var users = _userRepo.GetUsersP().Where(p => p.RoleId != 1).ToList();
            // When we use view data then we don't need to cast its type on the view side. While when we don't need type casting in case of viewbag 
            ViewBag.users = users;   // its a container that takes the value from controller and passes it to view, it can contain any string or object
            return View(users);   // it ll map the model on view that we can use there to show/list the values from DB, can use one of them at a time
        }

        //[HttpPost]

        [CustomAuthorize]
        public ActionResult AssociatePhysician(int physicianId, int patientId, bool isPrimary)
        {
            //Save selected physician for current patient (The patient whos detail is being shown)
            PatientPhysician pPhysician = new PatientPhysician();
            pPhysician.PhysicianId = physicianId;
            pPhysician.PatientId = patientId;
            pPhysician.IsPrimary = isPrimary;
            _patientRepo.SavePatientPhysician(pPhysician);

            //Get list of saved patients for the current patient. and return  this list to the patient detail page via ajax.
            ViewBag.PPhysicians = _patientRepo.GetPatientPhysicians(patientId);
            return PartialView("_PartialPhysicians");
            //return "response done";
        }

        [CustomAuthorize]
        public ActionResult DeassociatePhysician(int recid, int patientId)
        {
            _patientRepo.RemovePatientPhysician(recid);

            //Get list of saved patients for the current patient. and return  this list to the patient detail page via ajax.
            ViewBag.PPhysicians = _patientRepo.GetPatientPhysicians(patientId);
            return PartialView("_PartialPhysicians");
            //return "response done";
        }

        [CustomAuthorize]
        public ActionResult RegisterPermission()
        {

            SetRolesAndModules();
            return View();
        }

        [CustomAuthorize(permissionEntity = "Permissions")]
        public ActionResult DeletePermission(int id)
        {
            bool deleted = _userRepo.DeleteP(id);
            return RedirectToAction("IndexPermission", "user", new { });

        }

        [CustomAuthorize(permissionEntity = "Permissions")]
        public ActionResult EditPermission(int id)
        {
            var User = _userRepo.GetUserP(id);
            PMSEntities1 db = new PMSEntities1();
            SetRolesAndModules();


            return View("RegisterPermission", User);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session["User"] = null; //removes only one session, session.remove means only remove this one session
            Session.Remove("User");
            Session.Abandon(); // flush all of the sessions 
            return RedirectToAction("Login", "User", new { });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Authenticate(string txtUserName, string txtPassword)
        {
            bool isValid = _userRepo.Validate(txtUserName, txtPassword);
            if (isValid)
            {
                Session["User"] = _userRepo.GetUserByName(txtUserName, txtPassword);
                //Session.Timeout = 30;
                return RedirectToAction("Index", "Home", new { });
            }
            ViewBag.Message = "Invalid Username/Password!";
            return View("Login");
        }

        public void SetRolesAndModules()
        {
            PMSEntities1 db = new PMSEntities1();
            var roles = db.Roles.Where(r => r.Title != "Admin").ToList();
            //SelectList list = new SelectList(Getlist, "Id", "Title");
            ViewBag.RolesList = roles;
            var modules = db.Modules.ToList();
            ViewBag.ModulesList = modules;
        }

        [HttpPost]
        public bool IsValid(string UserName)
        {
            return _userRepo.ValidateUser(UserName);
        }

    }

}