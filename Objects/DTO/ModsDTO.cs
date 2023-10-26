using System.Collections.Generic;

namespace MVC.Models.DTO
{
    public class ModsDTO
    {
        public List<ModDTO> mods { get; set; }
        public int BuildId { get; set; }
    }

    public class ModDTO
    {
        public string version { get; set; }
        public string modId { get; set; }
    }
}
