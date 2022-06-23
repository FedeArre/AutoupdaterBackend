using System;
using System.Collections.Generic;
using System.Text;

namespace Objects.Repositories.Interfaces
{
    public interface IModRepository : IRepository<Mod>
    {
        public Mod FindByFileName(string fileName);
    }
}
