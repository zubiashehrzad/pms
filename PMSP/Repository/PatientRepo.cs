using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Repository
{
    public class PatientRepo
    {
        PMSEntities1 _db = new PMSEntities1();

        public List<Patient> GetPatients()
        {
            try
            { 
            return _db.Patients.ToList();
            }
            catch(Exception ex)
            {
                return new List<Patient>();
            }
        }

        public Patient GetPatient(int id)
        {
            try { 
            return _db.Patients.Find(id);
            }
            catch(Exception ex)
            {
                return new Patient();
            }
        }

        public int SavePatientPhysician(PatientPhysician patientphysician)
        {
            try
            {
                if (patientphysician.Id > 0)
                    _db.Entry(patientphysician).State = System.Data.Entity.EntityState.Modified;
                else
                    _db.Entry(patientphysician).State = System.Data.Entity.EntityState.Added;
                //_db.Users.Add(user);
                _db.SaveChanges();
                return patientphysician.Id;

            }
            catch(Exception ex)
            {
                return 0;
            }
                   }

        public bool RemovePatientPhysician(int id)
        {
            try
            {

                var physician = _db.PatientPhysicians.Find(id);
                _db.Entry(physician).State = System.Data.Entity.EntityState.Deleted;
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<PatientPhysician> GetPatientPhysicians(int patientId)
        {
            try
            {
                return _db.PatientPhysicians.Include("Physician.User").Where(u => u.PatientId == patientId).ToList();
            }
            catch(Exception ex)
            {
                return new List<PatientPhysician>();
            }
            
        }

        public int Save(Patient patient)
        {
            try
            {
                if (patient.Id > 0)
                    _db.Entry(patient).State = System.Data.Entity.EntityState.Modified;
                else
                    _db.Entry(patient).State = System.Data.Entity.EntityState.Added;
                //_db.Users.Add(user);
                _db.SaveChanges();
                return patient.Id;
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

                var patient = _db.Patients.Find(id);
                var user = _db.Users.Find(patient.UserId);
                //_db.Users.Remove(user);
                _db.Entry(patient).State = System.Data.Entity.EntityState.Deleted;
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
