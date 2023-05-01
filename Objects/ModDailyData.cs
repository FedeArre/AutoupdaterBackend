using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Objects
{
    public class ModDailyData
    {
        [Key]
        public string ModId { get; set; }
        public int PlayerCount { get; set; }
        public DateTime DataDate { get; set; }

        public ModDailyData(string modId, int playerCount, DateTime dataDate)
        {
            ModId = modId;
            PlayerCount = playerCount;
            DataDate = dataDate;
        }
    }
}
