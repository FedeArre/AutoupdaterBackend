﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class Mod
    {
        [Key]
        public string ModId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string DownloadLink { get; set; }
        public string FileName { get; set; }
        public string ModAuthor { get; set; }
        public int CPC { get; set; } // Current player count
        public int PeakMax { get; set; }
        public bool EarlyAccessEnabled { get; set; }
        public bool DisableAutoupdating { get; set; }
        public int Peak24;
    }
}
