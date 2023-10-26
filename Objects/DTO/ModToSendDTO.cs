using System;

namespace MVC.Models.DTO
{
    public class ModToSendDTO
    {
        public string mod_id { get; set; }
        public string mod_name { get; set; }
        public string current_version { get; set; }
        public string current_download_link { get; set; }
        public string created_by { get; set; }
        public string file_name { get; set; } 
        public DateTime last_update { get; set; }
        public bool unsupported { get; set; }
    }
}
