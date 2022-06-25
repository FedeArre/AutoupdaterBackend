using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTO;
using Objects;
using Objects.Repositories.Interfaces;
using System.Collections.Generic;

namespace MVC.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModsController : ControllerBase
    {
        private IModRepository modsRepo;

        public ModsController(IModRepository modsRepo)
        {
            this.modsRepo = modsRepo;
        }

        // GET api/<ComprasController>/5
        [HttpGet, Route("Mods")]
        public ActionResult<List<ModToSendDTO>> Mods([FromBody] ModsDTO modsList)
        {
            if (modsList == null || modsList.mods == null)
                return BadRequest();

            List<ModToSendDTO> modsToUpdate = new List<ModToSendDTO>();
            try
            {
                foreach(var m in modsList.mods)
                {
                    Mod mod = modsRepo.FindById(m.modId);
                    if(mod != null)
                    {
                        if(mod.Version != m.version)
                        {
                            ModToSendDTO mts = new ModToSendDTO();
                            mts.mod_id = mod.ModId;
                            mts.file_name = mod.FileName;
                            mts.current_download_link = mod.DownloadLink;
                            mts.current_version = mod.Version;
                            mts.mod_name = mod.Name;

                            modsToUpdate.Add(mts);
                        }
                    }
                }
            }
            catch
            {
                return Problem();
            }

            return Ok(modsToUpdate);
        }
    }
}
