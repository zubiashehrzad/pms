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
            return _db.Physicians.ToList();
        }

        public Physician GetPhysician(int id)
        {
            return _db.Physicians.Find(id);
        }

        public int Save(Physician physician)
        {
            if (physician.Id > 0)
                _db.Entry(physician).State = System.Data.Entity.EntityState.Modified;
            else
                _db.Entry(physician).State = System.Data.Entity.EntityState.Added;
            //_db.Users.Add(user);
            _db.SaveChanges();
            return physician.Id;
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
    }
}