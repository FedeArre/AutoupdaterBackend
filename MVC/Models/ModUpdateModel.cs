using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MVC.Models
{
    public class ModUpdateModel
    {
        public string ModId { get; set; }
        public string ModName { get; set; }
        public string ModVersion { get; set; }
        public string ModDownloadLink { get; set; }
        public IFormFile ModFile { get; set; }
        public List<SelectListItem> GameBuilds { get; set; }
        public int SelectedGameBuild { get; set; }
    }
}
