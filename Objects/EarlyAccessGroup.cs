using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class EarlyAccessGroup
    {
        [Key]
        public string GroupName { get; set; }

        [Key]
        public User Owner { get; set; }

        public virtual ICollection<EAS> Users { get; set; }
    }
}
