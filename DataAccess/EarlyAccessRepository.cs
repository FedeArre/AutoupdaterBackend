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
        public bool Add(EarlyAccessGroup entity)
        {
            throw new NotImplementedException();
        }

        public bool AddTesterToGroup(EarlyAccessStatus tester, string groupName, User owner)
        {
            throw new NotImplementedException();
        }

        public bool Delete(EarlyAccessGroup entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EarlyAccessGroup> FindAll()
        {
            throw new NotImplementedException();
        }

        public EarlyAccessGroup FindById(string id)
        {
            throw new NotImplementedException();
        }

        public bool RemoveTesterFromGroup(EarlyAccessStatus tester, string groupName, User owner)
        {
            throw new NotImplementedException();
        }

        public bool Update(EarlyAccessGroup entity)
        {
            throw new NotImplementedException();
        }
    }
}
