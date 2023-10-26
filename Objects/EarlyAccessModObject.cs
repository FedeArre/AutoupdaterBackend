using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class EarlyAccessModObject
    {
        [Key]
        public string ModId {  get; set; }
        public string DownloadLink { get; set; }
        public string CurrentKey { get; set; }
    }
}
