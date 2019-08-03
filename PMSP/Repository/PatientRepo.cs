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
            try { 
            return _db.Patients.ToList();
            }
            catch(Exception ex)
            {
                return new List<Patient>();
            }
        }

        public Patient GetPatient(int id)
        {
            return _db.Patients.Find(id);
        }

        public int SavePatientPhysician(PatientPhysician patientphysician)
        {
            if (patientphysician.Id > 0)
                _db.Entry(patientphysician).State = System.Data.Entity.EntityState.Modified;
            else
                _db.Entry(patientphysician).State = System.Data.Entity.EntityState.Added;
            //_db.Users.Add(user);
            _db.SaveChanges();
            return patientphysician.Id;
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
            return _db.PatientPhysicians.Include("Physician.User").Where(u => u.PatientId == patientId).ToList();
        }

        public int Save(Patient patient)
        {
            if (patient.Id > 0)
                _db.Entry(patient).State = System.Data.Entity.EntityState.Modified;
            else
                _db.Entry(patient).State = System.Data.Entity.EntityState.Added;
            //_db.Users.Add(user);
            _db.SaveChanges();
            return patient.Id;
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
    }
}
