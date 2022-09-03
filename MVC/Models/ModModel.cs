using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Models
{
    public class ModModel
    {
        public string ModId { get; set; }
        public string ModName { get; set; }
        public string ModVersion { get; set; }
        public string ModFileName { get; set; }
        public string ModDownloadLink { get; set; }
        public IFormFile ModFile { get; set; }
    }
}
