using Microsoft.EntityFrameworkCore;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class EarlyAccessRepository : IEarlyAccessRepository
    {
        private AutoupdaterContext db;
        public EarlyAccessRepository(AutoupdaterContext db)
        {
            this.db = db;
        }

        public bool Add(EarlyAccessGroup entity)
        {
            try
            {
                db.EarlyAccess.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddTesterToGroup(EAS tester, string groupName, User owner)
        {
            EarlyAccessGroup eag = FindSpecificGroupFromUser(groupName, owner);
            if(eag != null)
            {
                tester.eag = eag;
                try
                {
                    db.EarlyAccessTesters.Add(tester);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public bool Delete(EarlyAccessGroup entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EarlyAccessGroup> FindAll()
        {
            return db.EarlyAccess.Include(u => u.Users).ToList();
        }

        public EarlyAccessGroup FindById(string id)
        {
            throw new NotImplementedException();
        }

        public List<EarlyAccessGroup> FindGroupFromUser(User owner)
        {
            try
            {
                return db.EarlyAccess.Include(m => m.Users).Where(m => m.Owner == owner).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public EarlyAccessGroup FindSpecificGroupFromUser(string groupName, User owner)
        {
            return db.EarlyAccess.Include(m => m.Users).Where(m => m.Owner == owner && m.GroupName == groupName).FirstOrDefault();
        }

        public bool RemoveTesterFromGroup(EAS tester, string groupName, User owner)
        {
            EarlyAccessGroup eag = FindSpecificGroupFromUser(groupName, owner);
            if (eag != null)
            {
                tester.eag = eag;
                try
                {
                    db.EarlyAccessTesters.Remove(tester);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }

            return false;
        }

        public bool Update(EarlyAccessGroup entity)
        {
            try
            {
                db.EarlyAccess.Update(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddEAModObject(EarlyAccessModObject eamo)
        {
            try
            {
                db.EAModObjects.Add(eamo);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public EarlyAccessModObject GetEAModObject(string modId)
        {
            return db.EAModObjects.Where(m => m.ModId == modId).FirstOrDefault();
        }

        public bool UpdateEAMod(EarlyAccessModObject eamo)
        {
            try
            {
                db.EAModObjects.Update(eamo);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public EarlyAccessModObject FindEAObjectByKey(string key)
        {
            return db.EAModObjects.Where(m => m.CurrentKey == key).FirstOrDefault();
        }
    }
}
