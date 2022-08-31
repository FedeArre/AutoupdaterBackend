using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTO;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.HttpOverrides;

namespace MVC.Controllers.API
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private IModRepository modsRepo;
        
        public ApiController(IModRepository modsRepo)
        {
            this.modsRepo = modsRepo;
        }

        [HttpPost, Route("Mods")]
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
            catch(Exception ex)
            {
                return Problem();
            }

            return Ok(modsToUpdate);
        }
        
        [HttpPost, Route("alive")]
        public ActionResult<bool> Alive([FromBody] ModsDTO modsList)
        {
            if (modsList == null || modsList.mods == null)
                return BadRequest();

            try
            {
                Telemetry telemetry = new Telemetry();
                foreach (var m in modsList.mods)
                {
                    telemetry.UsingMods.Add(m.modId);
                }

                // Get IP adresss of the request
                telemetry.IP = HttpContext.Connection.RemoteIpAddress.ToString();
                TelemetryHandler.GetInstance().Add(telemetry);
            }
            catch
            {
                return Problem();
            }

            return Ok(true);
        }

        [HttpGet, Route("Autoupdater")]
        public ActionResult<AutoupdaterDTO> Autoupdater()
        {
            AutoupdaterDTO auto = new AutoupdaterDTO();
            auto.current_version = "v1.2.0";
            auto.download_link = "empty";   
            return Ok(auto);
        }

    }
}
