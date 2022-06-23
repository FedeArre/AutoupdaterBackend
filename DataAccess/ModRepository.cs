using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public bool Delete(Mod entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Mod> FindAll()
        {
            throw new NotImplementedException();
        }

        public Mod FindById(string id)
        {
            throw new NotImplementedException();
        }

        public Mod FindByFileName(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
