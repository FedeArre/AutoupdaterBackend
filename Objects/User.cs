using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType Role { get; set; }
        public string TokenAPI { get; set; }
    }

    public enum UserType
    {
        Unverified = 0,
        Modder,
        AutoupdaterDev,
        DisabledAccount
    }
}
