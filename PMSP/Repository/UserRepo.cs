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
            try
            {
                var user = _db.Users.Include("Role.UserPermissions").Where(u => u.UserName == UserName && u.Password == Password).SingleOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                return new User(); //ex.Message (this ll give u detail of error msg detail, so we can save to db)
            }
        }


        public User GetUserByName(string UserName)
        {
            try
            {
                var user = _db.Users.Where(u => u.UserName == UserName).SingleOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                return new User(); //ex.Message (this ll give u detail of error msg detail, so we can save to db)
            }
        }

        public List<User> GetUsers()
        {
            try
            {
                return _db.Users.ToList();
            }
            catch (Exception ex)
            {
                return new List<User>();   // its called initialize object
            }
        }

        public List<UserPermission> GetUsersP()
        {
            try
            {
                return _db.UserPermissions.ToList();
            }
            catch (Exception ex)
            {
                return new List<UserPermission>();
            }


        }

        public User GetUser(int id)
        {
            try
            {
                return _db.Users.Find(id);
            }
            catch (Exception ex)
            {
                return new User();
            }

        }

        public UserPermission GetUserP(int id)
        {
            try
            {
                return _db.UserPermissions.Find(id);
            }
            catch (Exception ex)
            {
                return new UserPermission();
            }
        }



        /// <summary>
        /// We can use this method to save new user to database or we can update an existing user to the databse.
        /// </summary>
        /// <param name="user">This is a user </param>
        /// <returns></returns>
        public int Save(User user)
        {
            try
            {

                if (user.Id > 0)
                    _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                else
                    _db.Entry(user).State = System.Data.Entity.EntityState.Added;
                //db.Users.Add(user);
                _db.SaveChanges();
                return user.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public int Savepwd(int userId, string password)
        {
            try
            {

                var user = _db.Users.Find(userId);
                user.Password = password;
                _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return user.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int SavePermission(UserPermission userpermission)
        {
            try
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
            catch (Exception ex)
            {
                return 0;
            }
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

        public bool DeleteP(int id)
        {
            try
            {
                var user = _db.UserPermissions.Find(id);

                _db.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                //we can log exception to db
                return false;
            }
        }



        public bool Validate(string txtUserName, string txtPassword)//// login
        {
            try
            {

                bool isValid = _db.Users.Any(u => u.UserName == txtUserName && u.Password == txtPassword);
                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ValidateUser(string txtUserName)  //chks that username already exists
        {
            try
            {
                bool isValid = _db.Users.Any(u => u.UserName == txtUserName);
                return isValid;
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
