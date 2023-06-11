using Microsoft.EntityFrameworkCore;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class ModRepository : IModRepository
    {

        private AutoupdaterContext db;
        public ModRepository(AutoupdaterContext db)
        {
            this.db = db;
        }

        public bool Add(Mod entity)
        {
            try
            {
                db.Mods.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Mod entity)
        {
            try
            {
                db.Mods.Remove(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<Mod> FindAll()
        {
            var mods = db.Mods.Include(v => v.LatestVersion).ToList();

            foreach (Mod m in mods)
            {
                m.CPC = TelemetryHandler.GetInstance().GetCurrentPlayerCount(m.ModId);
            }

            return mods;
        }

        public Mod FindById(string id)
        {
            var m = db.Mods.Include(v => v.LatestVersion).Include(v => v.Allowed).Where(m => m.ModId == id).FirstOrDefault();
            if(m != null)
                m.CPC = TelemetryHandler.GetInstance().GetCurrentPlayerCount(m.ModId);
            return m;
        }

        public Mod FindByFileName(string fileName)
        {
            var m = db.Mods.Where(m => m.FileName == fileName).Include(v => v.LatestVersion).FirstOrDefault();

            if (m != null)
                m.CPC = TelemetryHandler.GetInstance().GetCurrentPlayerCount(m.ModId);
            
            return m;
        }

        public List<Mod> FindByAuthor(string author)
        {
            var mods = db.Mods.Where(m => m.ModAuthor == author).Include(v => v.LatestVersion).ToList();
            
            foreach(Mod m in mods)
            {
                m.CPC = TelemetryHandler.GetInstance().GetCurrentPlayerCount(m.ModId);
            }
            
            return mods;
        }

        public bool Update(Mod entity)
        {
            try
            {
                db.Mods.Update(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
