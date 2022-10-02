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

        public bool Add(EarlyAccessStatus entity)
        {
            try
            {
                db.EarlyAccess.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool Delete(EarlyAccessStatus entity)
        {
            try
            {
                db.EarlyAccess.Remove(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void CopyTesters(string goesToMod, string copyFromMod)
        {
            // First of all, delete all testers from goesTo mod.
            DeleteAllTesters(goesToMod);

            // Now, we copy testers from copyFromMod to our goesTo mod.
            List<EarlyAccessStatus> list = db.EarlyAccess.Where(x => x.ModId == copyFromMod).ToList();
            list.ForEach(x =>
            {
                x.ModId = goesToMod;
                Add(x);
            });
        }

        public void DeleteAllTesters(string modId)
        {
            List<EarlyAccessStatus> list = db.EarlyAccess.Where(x => x.ModId == modId).ToList();
            list.ForEach(x => Delete(x));
        }
        
        public IEnumerable<EarlyAccessStatus> FindAll()
        {
            throw new NotImplementedException();
        }

        public EarlyAccessStatus FindById(string id)
        {
            throw new NotImplementedException();
        }

        public List<EarlyAccessStatus> FindByModId(string modId)
        {
            return db.EarlyAccess.Where(x => x.ModId == modId).ToList();
        }

        public bool Update(EarlyAccessStatus entity)
        {
            throw new NotImplementedException();
        }
    }
}
