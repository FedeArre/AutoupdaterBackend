using System.Collections.Generic;

namespace MVC.Models
{
    public class EarlyAccessModel
    {
        public string ModId { get; set; }
        public bool IsEarlyAccess { get; set; }
        public List<EarlyAccessAllowedModel> Authors { get; set; }
    }
}
