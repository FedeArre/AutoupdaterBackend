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
            return db.Mods.ToList();
        }

        public Mod FindById(string id)
        {
            return db.Mods.Where(m => m.ModId == id).FirstOrDefault();
        }

        public Mod FindByFileName(string fileName)
        {
            return db.Mods.Where(m => m.FileName == fileName).FirstOrDefault();
        }

        public List<Mod> FindByAuthor(string author)
        {
            return db.Mods.Where(m => m.ModAuthor == author).ToList();
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
