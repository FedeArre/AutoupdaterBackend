using System;
using System.Collections.Generic;
using System.Text;

namespace Objects.Repositories.Interfaces
{
    public interface IEarlyAccessRepository : IRepository<EarlyAccessGroup>
    {
        bool AddTesterToGroup(EarlyAccessStatus tester, string groupName, User owner);
        bool RemoveTesterFromGroup(EarlyAccessStatus tester, string groupName, User owner);
    }
}
