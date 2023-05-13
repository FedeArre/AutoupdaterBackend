using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTO;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.HttpOverrides;
using MVC.Models;
using System.Linq;

namespace MVC.Controllers.API
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private IModRepository modsRepo;
        private IEarlyAccessRepository earlyRepo;
        private IUserRepository userRepo;
        public ApiController(IModRepository modsRepo, IEarlyAccessRepository earlyRepo,IUserRepository userRepo)
        {
            this.modsRepo = modsRepo;
            this.earlyRepo = earlyRepo;
            this.userRepo = userRepo;
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
                        if(mod.Version != m.version /*&& /*!mod.DisableAutoupdating*/)
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

        [HttpPost, Route("eacheck")]
        public ActionResult<bool> EarlyAccess(EarlyAccessJson eaj)
        {            
            if (eaj == null || eaj.ModId == null || eaj.SteamId == null)
                return BadRequest();

            Mod m = modsRepo.FindById(eaj.ModId);
            if(m == null || !m.EarlyAccessEnabled)
            {
                return Ok(false);
            }

            EarlyAccessStatus eas = null;
            /*foreach (var ea in earlyRepo.FindByModId(eaj.ModId))
            {
                if (ea.Steam64 == eaj.SteamId)
                {
                    eas = ea;
                    break;
                }
            }*/
            
            return Ok(eas != null);
        }

        [HttpGet, Route("Autoupdater")]
        public ActionResult<AutoupdaterDTO> Autoupdater()
        {
            AutoupdaterDTO auto = new AutoupdaterDTO();
            auto.current_version = "v1.2.0";
            auto.download_link = "empty";   
            return Ok(auto);
        }

        [HttpGet, Route("playercount")]
        public ActionResult<PlayerDataDTO> GetPlayerCount(string token, string modId)
        {
            if (String.IsNullOrWhiteSpace(token) || userRepo.FindByToken(token) == null)
                return BadRequest("Invalid token");

            if (String.IsNullOrWhiteSpace(modId))
                return BadRequest("Invalid mod");
            
            Mod m = modsRepo.FindById(modId);
            if (m == null)
                return BadRequest("Invalid mod");
            
            PlayerDataDTO data = new PlayerDataDTO();
            data.ModId = modId;
            data.CurrentPlayers = TelemetryHandler.GetInstance().GetCurrentPlayerCount(modId);
            /*if (TelemetryHandler.GetInstance().Peak24.ContainsKey(modId))
                data.DailyPeak = TelemetryHandler.GetInstance().Peak24[modId];
                    
            data.HistoricPeak = m.Peak24 > m.PeakMax ? m.Peak24 : m.PeakMax;*/
            return Ok(data);
        }

        [HttpGet, Route("allmods")]
        public ActionResult<AllModsDTO> GetAllMods(string token)
        {
            if (String.IsNullOrWhiteSpace(token) || userRepo.FindByToken(token) == null)
                return BadRequest("Invalid token");

            List<Mod> mods = modsRepo.FindAll().ToList();
            AllModsDTO allMods = new AllModsDTO();
            allMods.Mods = new List<ModAuthorPair>();
            foreach (Mod m in mods)
            {
                //if (m.DisableAutoupdating)
                //    continue;
                
                ModAuthorPair map = new ModAuthorPair();
                map.ModAuthor = m.ModAuthor;
                map.ModId = m.ModId;

                allMods.Mods.Add(map);
            }
            
            return Ok(allMods);
        }

        [HttpGet, Route("eapatreon")]
        public ActionResult<bool> EAPatreon(PatreonEAModel model)
        {
            User u = userRepo.FindByToken(model.Token);
            if (String.IsNullOrWhiteSpace(model.Token) || u == null)
                return BadRequest("Invalid token");

            Mod m = modsRepo.FindById(model.DummyModId);
            if (m == null || !m.EarlyAccessEnabled)
            {
                return Ok(false);
            }

            /*foreach (var ea in earlyRepo.FindByModId(model.DummyModId))
            {
                if (ea.Username == model.DiscordIdentifier)
                {
                    ea.Steam64 = model.NewSteamId;
                    earlyRepo.Update(ea);

                    List<string> mods = new List<string>();
                    foreach(Mod mod in modsRepo.FindByAuthor(u.Username))
                    {
                        if (mod.ModId != model.DummyModId)
                            mods.Add(mod.ModId);
                    }

                    earlyRepo.CopyTestersToAll(mods, model.DummyModId);
                    break;
                }
            }*/
            
            return Ok(false);
        }
    }
}
