using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class ModAllowed
    {
        [Key]
        public int Id { get; set; }
        public string ModIdentifierString { get; set; }
        public Mod Mod { get; set; }
        public EarlyAccessGroup Group { get; set; }
    }
}
