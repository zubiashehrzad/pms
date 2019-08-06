using PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PMS.Models;
using PMS.SendGridEmailService;

namespace PMS.Controllers
{
    public class EmailController : Controller
    {
        public void SendEmail(string to, string subject, string body)
        {
            SendGridEmail.SendEmail(to, subject, body);
        }
    }
}