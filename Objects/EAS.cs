using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class EAS
    {
        public int Id { get; set; }
        public EarlyAccessGroup eag { get; set; }
        [MaxLength(50)]
        public string Steam64 { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
    }
}
