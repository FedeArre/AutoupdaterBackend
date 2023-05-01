using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class Telemetry
    {
        [Key]
        public string IP;
        public long Timestamp;
        public List<string> UsingMods = new List<string>();
    }
}
