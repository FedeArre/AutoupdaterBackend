using System;
using System.Collections.Generic;
using System.Text;

namespace Objects.Repositories.Interfaces
{
    public interface IEarlyAccessRepository : IRepository<EarlyAccessGroup>
    {
        bool AddTesterToGroup(EAS tester, string groupName, User owner);
        bool RemoveTesterFromGroup(EAS tester, string groupName, User owner);
        List<EarlyAccessGroup> FindGroupFromUser(User owner);
        EarlyAccessGroup FindSpecificGroupFromUser(string groupName, User owner);
    }
}
