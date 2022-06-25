using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModsController : ControllerBase
    {
        // GET api/<ComprasController>/5
        [HttpGet, Route("GetName")]
        public ActionResult<string> GetName()
        {
            return Ok("name");
        }
    }
}
