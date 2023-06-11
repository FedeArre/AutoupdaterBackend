using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Objects;
using System.Collections.Generic;

namespace MVC.Models
{
    public class EAGroupModel
    {
        public string GroupName { get; set; }
        public ICollection<EAS> Users { get; set; }
    }
}
