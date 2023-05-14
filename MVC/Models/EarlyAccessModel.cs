using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MVC.Models
{
    public class EarlyAccessModel
    {
        public string ModId { get; set; }
        public string SelectedGroupIdentifier { get; set; } 
        public List<SelectListItem> AvailableGroups { get; set; }
    }
}
