using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMS.Models;
using PMS.Repository;
using System.IO;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class DocumentController : Controller
    {
        // GET: Document
        UserRepo _userRepo = new UserRepo();
        DocumentRepo _docRepo = new DocumentRepo();
        public ActionResult UploadDocument(int user_Id, HttpPostedFileBase documentInfo)
        {
            string fileName = Path.GetFileName(documentInfo.FileName);
            string filePath = user_Id.ToString() + "-" + DateTime.Now.ToString("MMddyyyyhhmmsstt") + Path.GetExtension(documentInfo.FileName);
            string rootedPath = Path.Combine(Server.MapPath("~/Files"), filePath); // full direction from root, complete path
            documentInfo.SaveAs(rootedPath);

            UserDocument doc = new UserDocument();
            doc.Id = 0;
            doc.FileName = fileName;
            doc.FilePath = filePath;
            doc.User_Id = user_Id;
            doc.UploadedBy = ((User)Session["User"]).Id;
            doc.UploadOn = DateTime.Now;
            _docRepo.SaveDocument(doc);
            //_userRepo.UpdateImagePath(user_Id, fileName, filePath);

            /* MemoryStream ms = new MemoryStream();
             fileInfo.InputStream.CopyTo(ms);
             byte[] fileArray = ms.ToArray();
             _userRepo.UpdateImagePath(userId, fileArray, fileName);*/

            return RedirectToAction("ViewDetails", "User", new { id = user_Id });
        }

        public ActionResult DeleteFile(int id)
        {
            var docuemnt = _docRepo.GetUserDocuments().Where(d => d.Id == id).SingleOrDefault();

            string FileName = docuemnt.FilePath;
            int user_Id = docuemnt.User_Id;

            ViewBag.deleteSuccess = "false";


            var fullPath = Server.MapPath("~/Files/" + FileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                ViewBag.deleteSuccess = "true";
            }
            _docRepo.DeleteDocument(id);
            return RedirectToAction("ViewDetails", "User", new { id = user_Id });
        }

        public FileContentResult DownloadFile(int id)
        {
            var docuemnt = _docRepo.GetUserDocuments().Where(d => d.Id == id).SingleOrDefault();
            var fullPath = Server.MapPath("~/Files/" + docuemnt.FilePath);

            //string filepath = AppDomain.CurrentDomain.BaseDirectory + fullPath;
            byte[] filedata = System.IO.File.ReadAllBytes(fullPath); //byte i.e binary data
            string contentType = MimeMapping.GetMimeMapping(fullPath); //content type means kis type ki file hai, wats its extension e.g doc,jpg,xls

            var cd = new System.Net.Mime.ContentDisposition  // file humy jis tara wapis karni hai, us ka naam method, jo file humay wapas karni hai, asal mai jis naam sy upload ki thi, usi naam sy aur extension sy sb kuj wapas hoga
            {
                FileName = docuemnt.FileName,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString()); // header tells wat is coming in response data   

            return File(filedata, contentType);
        }

    }
}