using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Repository
{
    public class UserRepo
    {
        PMSEntities1 _db = new PMSEntities1();

        public User GetUserByName(string UserName, string Password)
        {
            var user = _db.Users.Include("Role.UserPermissions").Where(u => u.UserName == UserName && u.Password == Password).SingleOrDefault();
            return user;
        }

        public List<User> GetUsers()
        {
            return _db.Users.ToList();
        }

        public User GetUser(int id)
        {
            return _db.Users.Find(id);
        }

        /// <summary>
        /// We can use this method to save new user to database or we can update an existing user to the databse.
        /// </summary>
        /// <param name="user">This is a user </param>
        /// <returns></returns>
        public int Save(User user)
        {
            if (user.Id > 0)
                _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            else
                _db.Entry(user).State = System.Data.Entity.EntityState.Added;
            //db.Users.Add(user);
            _db.SaveChanges();
            return user.Id;
        }


        public int SavePermission(UserPermission userpermission)
        {
            int roleId = userpermission.RoleId;
            int moduleId = userpermission.ModuleId;

            var existingPermissions = _db.UserPermissions.Where(u => u.RoleId == roleId && u.ModuleId == moduleId).FirstOrDefault();
            if (existingPermissions != null && existingPermissions.Id > 0)
            {
                existingPermissions.CanView = userpermission.CanView;
                existingPermissions.CanWrite = userpermission.CanWrite;
                existingPermissions.CanDelete = userpermission.CanDelete;
            }
            else if (userpermission.Id > 0)
            {
                _db.Entry(userpermission).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                _db.Entry(userpermission).State = System.Data.Entity.EntityState.Added;
            }
            //_db.Users.Add(user);
            _db.SaveChanges();
            return userpermission.Id;
        }

        public bool Delete(int id)
        {
            try
            {
                var user = _db.Users.Find(id);
                //_db.Users.Remove(user);
                _db.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Validate(string txtUserName, string txtPassword)
        {
            bool isValid = _db.Users.Any(u => u.UserName == txtUserName && u.Password == txtPassword);
            return isValid;
        }
    }
}
