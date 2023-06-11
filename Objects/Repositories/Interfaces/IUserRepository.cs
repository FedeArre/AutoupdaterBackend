using System;
using System.Collections.Generic;
using System.Text;

namespace Objects.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        public User Login(string username, string plainTextPassword);
        public User FindByToken(string token);
        public User FindByDiscordToken(string discordToken);
        public User FindByDiscordId(string id);
    }
}
