using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Repository
{
    public class PhysicianRepo
    {
        PMSEntities1 _db = new PMSEntities1();

        public List<Physician> GetPhysicians()
        {
            try { 
            return _db.Physicians.ToList();
            }
            catch (Exception ex)
            {
                return new List<Physician>();
            }
        }

        public Physician GetPhysician(int id)
        {
            try { 
            return _db.Physicians.Find(id);
            }
            catch(Exception ex)
            {
                return new Physician(); 
            }
        }

        public int Save(Physician physician)
        {
            try
            {
                if (physician.Id > 0)
                    _db.Entry(physician).State = System.Data.Entity.EntityState.Modified;
                else
                    _db.Entry(physician).State = System.Data.Entity.EntityState.Added;
                //_db.Users.Add(user);
                _db.SaveChanges();
                return physician.Id;
            
            }
            catch(Exception ex)
            {
                return 0;
            }
            
        }

        public bool Delete(int id)
        {
            try
            {

                var physician = _db.Physicians.Find(id);
                var user = _db.Users.Find(physician.UserId);
                //_db.Users.Remove(user);
                _db.Entry(physician).State = System.Data.Entity.EntityState.Deleted;
                _db.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ValidateEmail(string txtEmail)  //chks that email already exists
        {
            try
            {
                bool isValid = _db.Users.Any(u => u.Email == txtEmail);
                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}