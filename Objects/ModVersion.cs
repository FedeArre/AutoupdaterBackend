using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class ModVersion
    {
        [Key]
        public string ModId { get; set; }
        public string Version { get; set; }
        public int RequiredGameBuildId { get; set; }
    }
}
