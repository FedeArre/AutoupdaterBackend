using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.DiscordBot;
using MVC.Models;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class ModsController : Controller
    {
        private IModRepository modsRepo;
        private IUserRepository userRepo;
        private IEarlyAccessRepository earlyRepo;

        public ModsController(IModRepository mods, IUserRepository users, IEarlyAccessRepository earlyAccess)
        {
            modsRepo = mods;
            userRepo = users;
            earlyRepo = earlyAccess;
        }

        // Create
        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ModModel modData)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(modData.ModId))
            {
                ViewBag.Msg = "Mod ID can't be empty!";
                return View(modData);
            }

            if (string.IsNullOrEmpty(modData.ModName))
            {
                ViewBag.Msg = "Mod name can't be empty!";
                return View(modData);
            }

            if (string.IsNullOrEmpty(modData.ModVersion))
            {
                ViewBag.Msg = "Mod version can't be empty!";
                return View(modData);
            }

            if (string.IsNullOrEmpty(modData.ModFileName))
            {
                ViewBag.Msg = "File name can't be empty!";
                return View(modData);
            }

            if (string.IsNullOrEmpty(modData.ModDownloadLink) && modData.ModFile == null && !modData.AutoupdatingDisabled)
            {
                ViewBag.Msg = "No mod download link / file was provided!";
                return View(modData);
            }

            Mod existingMod = modsRepo.FindById(modData.ModId);
            if (existingMod != null)
            {
                ViewBag.Msg = "A mod already exists with this id!";
                return View(modData);
            }

            if (!modData.ModFileName.EndsWith(".dll"))
            {
                modData.ModFileName += ".dll";
            }

            // Direct download link provided
            if (modData.ModFile == null || modData.AutoupdatingDisabled)
            {
                if (!modData.AutoupdatingDisabled)
                {
                    Uri uriResult;
                    bool result = Uri.TryCreate(modData.ModDownloadLink, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if (!result)
                    {
                        ViewBag.Msg = "The download link is invalid!";
                        return View(modData);
                    }

                }
                
                string token = HttpContext.Session.GetString("userLoginToken");
                User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
                if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
                {
                    Mod createdMod = new Mod();
                    createdMod.ModId = modData.ModId;
                    createdMod.Name = modData.ModName;
                    createdMod.Version = modData.ModVersion;
                    createdMod.DownloadLink = modData.ModDownloadLink;
                    createdMod.FileName = modData.ModFileName;
                    createdMod.ModAuthor = u.Username;
                    //createdMod.DisableAutoupdating = modData.AutoupdatingDisabled;
                    
                    if (modsRepo.Add(createdMod))
                    {
                        ModelState.Clear(); 
                        BotHandler.GetInstance().SendDiscordMessage($"{u.Username} created mod {createdMod.Name}");

                        ViewBag.Msg = "Mod created succesfully!";
                    }
                    else
                    {
                        ViewBag.Msg = "Something went wrong while saving the mod. Try again!";
                    }

                    return View();
                }

                ViewBag.Msg = "Something went wrong :(";
            }
            else // File check
            {
                string token = HttpContext.Session.GetString("userLoginToken");
                User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
                if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
                {
                    if (modData.ModFile.Length > 10485761 && modData.ModId != "ModUtils") // 10MB size check (but allowing ModUtils (useless btw since too big))
                    {
                        ViewBag.Msg = "File size is too big!";
                        return View(modData);
                    }

                    if (modData.ModId.Length > 32)
                    {
                        ViewBag.Msg = "Mod ID can't be longer than 32 characters if using file upload method";
                        return View(modData);
                    }

                    if (!modData.ModFile.FileName.EndsWith(".dll"))
                    {
                        ViewBag.Msg = "Not a DLL!";
                        return View(modData);
                    }

                    // Ilegal characters check
                    if (!IsValidFilename(modData.ModId))
                    {
                        ViewBag.Msg = "Ilegal characters on mod ID - Use direct link instead";
                        return View(modData);
                    }

                    var path = Path.Combine(
                      Directory.GetCurrentDirectory(), "wwwroot/modfiles", (modData.ModId + ".dll"));

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await modData.ModFile.CopyToAsync(stream);
                    }

                    Mod createdMod = new Mod();
                    createdMod.ModId = modData.ModId;
                    createdMod.Name = modData.ModName;
                    createdMod.Version = modData.ModVersion;
                    createdMod.DownloadLink = "https://mygaragemod.xyz/Mods/DownloadMod?mod=" + modData.ModId + ".dll";
                    createdMod.FileName = modData.ModFileName;
                    createdMod.ModAuthor = u.Username;

                    if (modsRepo.Add(createdMod))
                    {
                        ModelState.Clear();
                        BotHandler.GetInstance().SendDiscordMessage($"{u.Username} created mod {createdMod.Name}");
                        
                        ViewBag.Msg = "Mod created succesfully!";
                    }
                    else
                    {
                        ViewBag.Msg = "Something went wrong while saving the mod. Try again!";
                    }

                    return View();
                }
                ViewBag.Msg = "Something went wrong :(";
            }
            return View(modData);
        }

        // ModDownload
        public IActionResult DownloadMod(string mod)
        {
            if (string.IsNullOrEmpty(mod))
                return Problem();

            var path = Path.Combine(
                      Directory.GetCurrentDirectory(), "wwwroot/modfiles", mod);

            if (!System.IO.File.Exists(path))
                return Problem();

            return PhysicalFile(path, "application/octet-stream", mod);
        }

        // MyMods
        [HttpGet]
        public IActionResult MyMods()
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            // User check
            MyModsModel mmm = new MyModsModel();
            
            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                mmm.ModList = modsRepo.FindByAuthor(u.Username);
                mmm.PlayerCount = TelemetryHandler.GetInstance().GetCurrentPlayerCount("ModUtils");
                
                foreach(Mod m in mmm.ModList)
                {
                    /*if (!TelemetryHandler.GetInstance().Peak24.ContainsKey(m.ModId))
                        continue;
                    
                    m.Peak24 = TelemetryHandler.GetInstance().Peak24[m.ModId];
                    if (m.Peak24 > m.PeakMax)
                        m.PeakMax = m.Peak24;*/
                }

                return View(mmm);
            }

            return View(mmm);
        }

        // Upload new version
        [HttpGet]
        public IActionResult NewVersion(string modId)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            // User check
            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                Mod modObject = modsRepo.FindById(modId);
                if (modObject != null && modObject.ModAuthor == u.Username)
                {
                    ModModel mm = new ModModel();
                    mm.ModId = modObject.ModId;
                    mm.ModName = modObject.Name;
                    return View(mm);
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult DeleteMod(string modId)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            // User check
            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            /*if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                Mod modObject = modsRepo.FindById(modId);
                if (modObject != null && modObject.ModAuthor == u.Username)
                {
                    earlyRepo.DeleteAllTesters(modId);
                    modsRepo.Delete(modObject);

                    BotHandler.GetInstance().SendDiscordMessage($"{u.Username} has deleted mod {modObject.Name}");
                    return RedirectToAction("MyMods", "Mods");
                }
            }*/

            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> NewVersion(ModModel modData)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(modData.ModDownloadLink) && modData.ModFile == null && !modData.AutoupdatingDisabled)
            {
                ViewBag.Msg = "No mod download link / file was provided!";
                return View(modData);
            }

            // Direct download link provided
            if (modData.ModFile == null || modData.AutoupdatingDisabled)
            {
                if(!modData.AutoupdatingDisabled)
                {
                    Uri uriResult;
                    bool result = Uri.TryCreate(modData.ModDownloadLink, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if (!result)
                    {
                        ViewBag.Msg = "The download link is invalid!";
                        return View(modData);
                    }
                }

                // User check
                string token = HttpContext.Session.GetString("userLoginToken");
                User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
                if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
                {
                    Mod modObject = modsRepo.FindById(modData.ModId);
                    if (modObject != null && modObject.ModAuthor == u.Username)
                    {
                        modObject.DownloadLink = modData.ModDownloadLink;
                        modObject.Version = modData.ModVersion;
                        modObject.DisableAutoupdating = modData.AutoupdatingDisabled;
                        
                        if (modsRepo.Update(modObject))
                        {
                            BotHandler.GetInstance().SendDiscordMessage($"{u.Username} updated mod {modObject.Name} to {modObject.Version}");

                            return RedirectToAction("MyMods", "Mods");
                        }
                        else
                        {
                            ViewBag.Msg = "Something went wrong :(";
                            return View();
                        }
                    }
                }
            }
            else
            {
                string token = HttpContext.Session.GetString("userLoginToken");
                User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
                if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
                {
                    Mod modObject = modsRepo.FindById(modData.ModId);
                    if (modObject != null && modObject.ModAuthor == u.Username)
                    {
                        if (modData.ModFile.Length > 10485761 && modData.ModId != "ModUtils") // 10MB size check (but allowing ModUtils)
                        {
                            ViewBag.Msg = "File size is too big!";
                            return View(modData);
                        }

                        if (modData.ModId.Length > 32)
                        {
                            ViewBag.Msg = "Mod ID can't be longer than 32 characters if using file upload method";
                            return View(modData);
                        }

                        if (!modData.ModFile.FileName.EndsWith(".dll"))
                        {
                            ViewBag.Msg = "Not a DLL!";
                            return View(modData);
                        }

                        // Ilegal characters check
                        if (!IsValidFilename(modData.ModId))
                        {
                            ViewBag.Msg = "Ilegal characters on mod ID - Use direct link instead";
                            return View(modData);
                        }

                        var path = Path.Combine(
                          Directory.GetCurrentDirectory(), "wwwroot/modfiles", (modData.ModId + ".dll"));

                        try
                        {
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await modData.ModFile.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Msg = "Something went wrong uploading the mod! Please re-upload it";
                            return View();
                        }

                        modObject.DownloadLink = "https://mygaragemod.xyz/Mods/DownloadMod?mod=" + modData.ModId + ".dll";
                        modObject.Version = modData.ModVersion;
                        if (modsRepo.Update(modObject))
                        {
                            BotHandler.GetInstance().SendDiscordMessage($"{u.Username} updated mod {modObject.Name} to {modObject.Version}");
                            return RedirectToAction("MyMods", "Mods");
                        }
                        else
                        {
                            ViewBag.Msg = "Something went wrong :(";
                            return View();
                        }
                    }
                }

            }

            ViewBag.Msg = "No permission!";
            return View(modData);
        }

        public bool CheckUserStatus()
        {
            string currentToken = HttpContext.Session.GetString("userLoginToken");

            if (currentToken == null)
                return false;

            if (TokenHandler.GetInstance().IsUserLogged(currentToken) == null)
            {
                HttpContext.Session.Remove("userLoginToken");
                HttpContext.Session.SetInt32("userRoleId", 0);
                HttpContext.Session.Remove("footerMessage");
                return false;
            }

            return true;
        }

        bool IsValidFilename(string testName)
        {
            Regex containsABadCharacter = new Regex("["
                  + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(testName)) { return false; };

            return true;
        }
    }
}
