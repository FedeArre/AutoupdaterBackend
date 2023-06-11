using Microsoft.AspNetCore.Mvc.Rendering;
using Objects;
using System.Collections.Generic;

namespace MVC.Models
{
    public class EarlyAccessModel
    {
        public string ModId { get; set; }
        public List<string> GroupsAllowed { get; set; }
        public List<string> GroupsAvailable { get; set; }
    }
}
