using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class UserRepository : IUserRepository
    {
        public bool Add(User entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> FindAll()
        {
            throw new NotImplementedException();
        }

        public User FindById(string id)
        {
            throw new NotImplementedException();
        }
    }
}
