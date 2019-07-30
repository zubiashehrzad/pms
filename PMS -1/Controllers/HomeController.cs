﻿using PMS.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{

    public class HomeController : Controller
    {
        // GET: Home
        [CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }
        
    }
}