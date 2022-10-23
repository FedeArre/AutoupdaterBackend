using System;
using System.Collections.Generic;
using System.Text;

namespace MVC.Models.DTO
{
    public class PlayerDataDTO
    {
        public string ModId { get; set; }
        public int CurrentPlayers { get; set; }
        public int DailyPeak { get; set; }
        public int HistoricPeak { get; set; }
    }

    public class AllModsDTO
    {
        public List<ModAuthorPair> Mods { get; set; }
    }

    public class ModAuthorPair
    {
        public string ModId { get; set; }
        public string ModAuthor { get; set; }
    }
}
