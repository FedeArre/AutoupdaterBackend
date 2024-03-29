﻿using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace DataAccess
{
    public class UserRepository : IUserRepository
    {
        AutoupdaterContext db;
        public UserRepository(AutoupdaterContext db)
        {
            this.db = db;
        }

        public bool Add(User entity)
        {
            User account = db.Users.Where(user => user.Username == entity.Username).FirstOrDefault();
            if (account != null)
                return false;

            try
            {
                entity.Password = HashPassword(entity.Password);

                db.Users.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> FindAll()
        {
            return db.Users.ToList();
        }

        public User FindById(string id)
        {
            return db.Users.Where(user => user.Username == id).FirstOrDefault();
        }
        public User FindByToken(string token)
        {
            return db.Users.Where(user => user.TokenAPI == token).FirstOrDefault();
        }
        
        public User Login(string username, string plainTextPassword)
        {
            User account = db.Users.Where(user => user.Username.ToLower() == username.ToLower()).FirstOrDefault();
            if(account == null || !BC.Verify(plainTextPassword, account.Password))
            {
                return null;
            }

            return account;
        }

        public bool Update(User entity)
        {
            try
            {
                db.Users.Update(entity);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string HashPassword(string plainText)
        {
            return BC.HashPassword(plainText, BC.GenerateSalt(12));
        }
    }
}
