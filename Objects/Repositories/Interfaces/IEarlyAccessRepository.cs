using System;
using System.Collections.Generic;
using System.Text;

namespace Objects.Repositories.Interfaces
{
    public interface IEarlyAccessRepository : IRepository<EarlyAccessStatus>
    {
        List<EarlyAccessStatus> FindByModId(string modId);
        void CopyTesters(string goesToMod, string copyFromMod);

        void DeleteAllTesters(string modId);
        void CopyTestersToAll(List<string> mods, string modId);
    }
}
