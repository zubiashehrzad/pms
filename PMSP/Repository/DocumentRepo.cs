using System;
using System.Collections.Generic;
using System.Linq;
using PMS.Models;
using System.Web;


namespace PMS.Repository
{
    public class DocumentRepo
    {
        PMSEntities1 _db = new PMSEntities1();

        public List<UserDocument> GetUserDocuments()
        {
            return _db.UserDocuments.ToList();


        }

        public int SaveDocument(UserDocument doc)
        {
            try
            {
                _db.Entry(doc).State = System.Data.Entity.EntityState.Added;
                //db.Users.Add(user);
                _db.SaveChanges();
                return doc.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool DeleteDocument(int docId)
        {
            try
            {
                var userDoc = _db.UserDocuments.Find(docId);
                //_db.Users.Remove(user);
                _db.Entry(userDoc).State = System.Data.Entity.EntityState.Deleted;
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}