﻿using PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PMS.Models;
using PMS.Filters;
using PMS.SendGridEmailService;
using System.IO;
using System.Web.Security;
using System.Web.Script.Serialization;

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
            ViewBag.message = "Hello users m testing ";
            return View(users);   // it ll map the model on view that we can use there to show/list the values from DB, can use one of them at a time

        }

        [CustomAuthorize(permissionEntity = "Users")]  //authorize
        [ValidateAntiForgeryToken] //chk that request is secure or not and comming from our own application,
        [HttpPost] // chk request type, all of these 3 are action filters
        public ActionResult Save(User user)
        {
            if ((IsEmailValid(user.Email)) || (IsValid(user.UserName)))
            {
                ViewBag.Message = "Username or Email already exists, please try with a different username and email address!";
                RoleList();
                return View("Register");
            }
            /* 
              else if (IsValid(user.Email))
              {
                  ViewBag.Message = "Email already exists, please try with a different email address!";
                  RoleList();
                  return View("Register");
              }
              else if (IsValid(user.UserName))
                   {
                  ViewBag.Message = "Username already exists, please try with a different username!";
                  RoleList();
                  return View("Register");
              }*/
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
            RoleList();
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


        #region fileing

        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult UploadFile(int userId, HttpPostedFileBase fileInfo)
        {
            string fileName = Path.GetFileName(fileInfo.FileName);
            //string filePath = userId.ToString() + "-" + DateTime.Now.ToString("MMddyyyyhhmmsstt") + Path.GetExtension(fileInfo.FileName);
            //string rootedPath = Path.Combine(Server.MapPath("~/Files"), filePath); // full direction from root, complete path
            //fileInfo.SaveAs(rootedPath);
            //_userRepo.UpdateImagePath(userId, fileName, filePath);
            MemoryStream ms = new MemoryStream();
            fileInfo.InputStream.CopyTo(ms);
            byte[] fileArray = ms.ToArray();
            _userRepo.UpdateImagePath(userId, fileArray, fileName);
            return RedirectToAction("Index", "User", new { });
        }


        #endregion

        #region Permissions

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

        #endregion

        #region Login page

        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.Message = TempData["Message"];
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
                // below 2 lines are for session
                var user = _userRepo.GetUserByName(txtUserName, txtPassword);
                Session["User"] = user;

                //Session.Timeout = 30;
                //rest of the lines are being written for cookie
                User u = new User();
                u.FirstName = user.FirstName;
                u.LastName = user.LastName;
                u.Id = user.Id;
                u.Role_Id = user.Role_Id;

                // Javascript Serializer class is used to searialize or deserialize object into or from JSON strong format.
                // JSON string format is something like a key paired value string object which we can use to show the data on browsers using any front end framework like angular.js or even JSON format can be used in ajax requests.
                JavaScriptSerializer seralizer = new JavaScriptSerializer();
                string userData = seralizer.Serialize(u);

                // Forms Authentication Ticket class is used to create a ticket for encrypting user data using FormsAuthnetication class used below.
                FormsAuthenticationTicket faTicket = new FormsAuthenticationTicket
                    (1, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(1440), false, userData);

                var encryptedData = FormsAuthentication.Encrypt(faTicket);

                // HttpCookie class is used to create browser based cookie to store user data in that. Wich we can used throughout our application just like a session.
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedData);
                cookie.Expires = DateTime.Now.AddMinutes(1440);

                //Adding cookie to browser using response.cookie method.
                Response.Cookies.Add(cookie);

                return RedirectToAction("Index", "Home", new { });
            }
            ViewBag.Message = "Invalid Username/Password!";
            return View("Login");
        }

        public ActionResult RcoverPwd(string txtUserName)
        {
            var user = _userRepo.GetUserByName(txtUserName);
            if (user != null)
            {
                string url = "http://localhost:62710/User/ChangePassword/" + user.Id;
                string body = "<p>You have requested for reset password.</p><p>Click here to <a href='" + url + "'>Change Password</a></p>";  // link for small page
                SendGridEmail.SendEmail(user.Email, "Change Password", body);
                return RedirectToAction("SuccessMessage", "User", new { });
            }
            else
            {
                return RedirectToAction("Login", "User", new { });
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult UpdatePwd(string password, int userId)
        {
            _userRepo.Savepwd(userId, password);
            TempData["Message"] = "Password modified successfully!";   //not using view bag bcz v r not going to view directly, we r redirecting so temp data is using from 1 action to other
            return RedirectToAction("Login", "User", new { });
        }

        public ActionResult ChangePassword(int? id)
        {
            ViewBag.userId = id;
            return View();
        }

        public ActionResult SuccessMessage()
        {
            return View();
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
        public bool IsValid(string UserName) // chks same username already existes in db ?
        {
            return _userRepo.ValidateUser(UserName);
        }

        [HttpPost]
        public bool IsEmailValid(string Email) // chks same email exists in db ??
        {
            return _userRepo.ValidateEmail(Email);
        }

        #endregion
        public void RoleList()
        {
            PMSEntities1 db = new PMSEntities1();
            var roles = db.Roles.Where(r => r.Title != "Patient" && r.Title != "Physician" && r.Title != "Admin").ToList();
            ViewBag.RolesList = roles;

        }

    }

}