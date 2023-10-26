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
                        if(mod.LatestVersion != null && mod.LatestVersion.Version != m.version)
                        {
                            if(mod.LatestVersion.RequiredGameBuildId <= modsList.BuildId)
                            {
                                ModToSendDTO mts = new ModToSendDTO();
                                mts.mod_id = mod.ModId;
                                mts.file_name = mod.FileName;
                                mts.current_download_link = mod.LatestVersion.DownloadLink;
                                mts.current_version = mod.LatestVersion.Version;
                                mts.mod_name = mod.Name;

                                modsToUpdate.Add(mts);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("ISSUE AT MODS ENDPOINT!");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
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
            IEnumerable<EarlyAccessGroup> EAGs = earlyRepo.FindAll(); // don't ask me why, but groups wont load without this...

            if(m == null)
            {
                return Ok(false);
            }

            foreach(ModAllowed ma in m.Allowed)
            {
                if(ma.Group != null)
                {
                    foreach(EAS tester in ma.Group.Users)
                    {
                        if (tester.Steam64 == eaj.SteamId)
                            return Ok(true);
                    }
                }
            }
            return Ok(false);
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

        [HttpPost, Route("eapatreon")]
        public ActionResult<bool> EAPatreon(PatreonEAModel model)
        {
            User u = userRepo.FindByToken(model.Token);
            if (String.IsNullOrWhiteSpace(model.Token) || u == null)
                return BadRequest("Invalid token");

            IEnumerable<EarlyAccessGroup> EAGs = earlyRepo.FindAll(); // don't ask me why, but groups wont load without this...

            foreach (var ea in EAGs)
            {
                if (ea.GroupName == model.Group)
                {
                    EAS existingTester = null;
                    foreach(var user in ea.Users)
                    {
                        if(user.Username == model.DiscordIdentifier)
                        {
                            existingTester = user;
                            break;
                        }
                    }


                    if(String.IsNullOrEmpty(model.NewSteamId))
                    {
                        if(existingTester != null)
                        {
                            ea.Users.Remove(existingTester);
                            earlyRepo.Update(ea);
                            return Ok($"User {model.DiscordIdentifier} removed");
                        }
                    }
                    else
                    {
                        if (existingTester != null)
                        {
                            existingTester.Steam64 = model.NewSteamId;
                            earlyRepo.Update(ea);
                            return Ok($"{model.NewSteamId} was set for user {model.DiscordIdentifier} succesfully");
                        }
                        else
                        {
                            EAS newTester = new EAS();
                            newTester.eag = ea;
                            newTester.Username = model.DiscordIdentifier;
                            newTester.Steam64 = model.NewSteamId;
                            ea.Users.Add(newTester);

                            earlyRepo.Update(ea);
                            return Ok($"{model.NewSteamId} was added for user {model.DiscordIdentifier} successfully");
                        }
                    }
                }
            }
            
            return Ok($"No action taken on user: {model.DiscordIdentifier} | {model.NewSteamId}");
        }
    }
}
