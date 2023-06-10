using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class EAS
    {
        public EarlyAccessGroup eag { get; set; }
        [MaxLength(50)]
        public string Steam64 { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(50)]
        public string Group { get; set; }
        [MaxLength(50)]
        public string OwnerUsername { get; set; }
    }
}
