using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class ModsController : Controller
    {
        private IModRepository modsRepo;
        private IUserRepository userRepo;
        private IEarlyAccessRepository earlyRepo;
        private GameVersioningHandler gameVersioningHandler;

        public ModsController(IModRepository mods, IUserRepository users, IEarlyAccessRepository earlyAccess, GameVersioningHandler gameVersioningHandler)
        {
            modsRepo = mods;
            userRepo = users;
            earlyRepo = earlyAccess;
            this.gameVersioningHandler = gameVersioningHandler;
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
        public IActionResult Create(ModModel modData)
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

            if (string.IsNullOrEmpty(modData.ModFileName))
            {
                ViewBag.Msg = "Mod file name can't be empty!";
                return View(modData);
            }

            if (!modData.ModFileName.EndsWith(".dll"))
                modData.ModFileName += ".dll";

            Mod existingMod = modsRepo.FindById(modData.ModId);
            if (existingMod != null)
            {
                ViewBag.Msg = "A mod already exists with this id!";
                return View(modData);
            }

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                Mod createdMod = new Mod();
                createdMod.ModId = modData.ModId;
                createdMod.Name = modData.ModName;
                createdMod.ModAuthor = u.Username;
                createdMod.FileName = modData.ModFileName;

                if (modsRepo.Add(createdMod))
                {
                    ModelState.Clear();

                    ViewBag.Msg = "Mod created succesfully!";
                }
                else
                {
                    ViewBag.Msg = "Something went wrong while saving the mod. Try again!";
                }

                return View();
            }

            ViewBag.Msg = "Something went wrong :(";
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
                    ModUpdateModel mm = new ModUpdateModel();
                    mm.ModId = modObject.ModId;
                    mm.ModName = modObject.Name;
                    List<SelectListItem> selects = new List<SelectListItem>();
                    foreach(KeyValuePair<string,int> kvp in gameVersioningHandler.GetVersions())
                    {
                        selects.Add(new SelectListItem { Text = $"{kvp.Key} - {kvp.Value}", Value = kvp.Value.ToString() });
                    }
                    mm.GameBuilds = selects;
                    return View(mm);
                }
            }

            return View();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> NewVersion(ModUpdateModel modData)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));

            List<SelectListItem> selects = new List<SelectListItem>();
            foreach (KeyValuePair<string, int> kvp in gameVersioningHandler.GetVersions())
            {
                selects.Add(new SelectListItem { Text = $"{kvp.Key} - {kvp.Value}", Value = kvp.Value.ToString() });
            }
            modData.GameBuilds = selects;

            if (string.IsNullOrEmpty(modData.ModDownloadLink) && modData.ModFile == null)
            {
                ViewBag.Msg = "No mod download link / file was provided!";
                return View(modData);
            }

            // Direct download link provided
            if (modData.ModFile == null)
            {
                Uri uriResult;
                bool result = Uri.TryCreate(modData.ModDownloadLink, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!result)
                {
                    ViewBag.Msg = "The download link is invalid!";
                    return View(modData);
                }

                // User check
                if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
                {
                    Mod modObject = modsRepo.FindById(modData.ModId);
                    if (modObject != null && modObject.ModAuthor == u.Username)
                    {
                        modObject.LatestVersion = new ModVersion();
                        modObject.LatestVersion.DownloadLink = modData.ModDownloadLink;
                        modObject.LatestVersion.Version = modData.ModVersion;
                        modObject.LatestVersion.RequiredGameBuildId = modData.SelectedGameBuild;

                        if (modsRepo.Update(modObject))
                        {
                            return RedirectToAction("ModDetails", "Mods", new { modObject.ModId });
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
                if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
                {
                    Mod modObject = modsRepo.FindById(modData.ModId);
                    if (modObject != null && modObject.ModAuthor == u.Username)
                    {
                        if (modData.ModFile.Length > 83886080) // 80MB size check (but allowing ModUtils)
                        {
                            ViewBag.Msg = "File size is too big! (>80MB!)";
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

                        modObject.LatestVersion = new ModVersion();
                        modObject.LatestVersion.DownloadLink = "https://modding.fedes.uy/Mods/DownloadMod?mod=" + modData.ModId + ".dll"; ;
                        modObject.LatestVersion.Version = modData.ModVersion;
                        modObject.LatestVersion.RequiredGameBuildId = modData.SelectedGameBuild;

                        if (modsRepo.Update(modObject))
                        {
                            return RedirectToAction("ModDetails", "Mods", new { modObject.ModId });
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

        [HttpGet]
        public IActionResult UploadEAMod(string modId)
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
                    ModUpdateModel mm = new ModUpdateModel();
                    mm.ModId = modObject.ModId;
                    mm.ModName = modObject.Name;
                    return View(mm);
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Unsupported(string modId)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));

            // User check
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                Mod modObject = modsRepo.FindById(modId);
                if (modObject != null && modObject.ModAuthor == u.Username)
                {
                    modObject.ModUnsupported = !modObject.ModUnsupported;

                    if (modsRepo.Update(modObject))
                    {
                        return RedirectToAction("ModDetails", "Mods", new { modObject.ModId });
                    }
                    else
                    {
                        ViewBag.Msg = "Something went wrong :(";
                        return View();
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadEAMod(ModUpdateModel modData)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));

            if (modData.ModFile == null)
            {
                ViewBag.Msg = "No file was provided!";
                return View(modData);
            }


            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                Mod modObject = modsRepo.FindById(modData.ModId);
                if (modObject != null && modObject.ModAuthor == u.Username)
                {
                    if (modData.ModFile.Length > 83886080) // 80MB size check (but allowing ModUtils)
                    {
                        ViewBag.Msg = "File size is too big! (>80MB!)";
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
                        ViewBag.Msg = "Ilegal characters on mod ID - Change it";
                        return View(modData);
                    }

                    var path = Path.Combine(
                      Directory.GetCurrentDirectory(), "wwwroot/eamodfiles", (modData.ModId + ".dll"));

                    try
                    {
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await modData.ModFile.CopyToAsync(stream);
                        }

                        string code = @"
                            using System;
                            using System.Collections.Generic;

                            public class MyClass
                            {
                                public void MyMethod()
                                {
                                    List<string> myList = new List<string>();
                                    Console.WriteLine(""Hello from MyClass!"");
                                }
                            }";

                        string filePath = "MyClass.cs";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Msg = "Something went wrong uploading the mod! Please re-upload it";
                        return View();
                    }

                    EarlyAccessModObject eamo = earlyRepo.GetEAModObject(modData.ModId);
                    if(eamo != null)
                    {
                        // UPDATING OBJECT!
                    }
                    else // Creating new
                    {
                        eamo = new EarlyAccessModObject();
                        eamo.ModId = modData.ModId;
                        eamo.DownloadLink = "https://modding.fedes.uy/Mods/EarlyAccessDownload?mod=" + modData.ModId + ".dll";
                        eamo.CurrentKey = new System.Guid().ToString();
                    }

                    modObject.LatestVersion = new ModVersion();
                    modObject.LatestVersion.DownloadLink = "https://modding.fedes.uy/Mods/DownloadMod?mod=" + modData.ModId + ".dll"; ;
                    modObject.LatestVersion.Version = modData.ModVersion;
                    modObject.LatestVersion.RequiredGameBuildId = modData.SelectedGameBuild;

                    if (modsRepo.Update(modObject))
                    {
                        return RedirectToAction("ModDetails", "Mods", new { modObject.ModId });
                    }
                    else
                    {
                        ViewBag.Msg = "Something went wrong :(";
                        return View();
                    }
                }
            }

            ViewBag.Msg = "No permission!";
            return View(modData);
        }

        [HttpGet]
        public IActionResult DeleteMod(string modId)
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
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/modfiles", modId + ".dll");

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    modsRepo.Delete(modObject);

                    return RedirectToAction("MyMods", "Mods");
                }
            }

            return View();
        }
        
        [HttpGet]
        public IActionResult ModDetails(string modId)
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
                    ModManageModel mmm = new ModManageModel(modObject);
                    mmm.CPC = TelemetryHandler.GetInstance().GetCurrentPlayerCount(modObject.ModId);
                    mmm.CPC_ModUtils = TelemetryHandler.GetInstance().GetCurrentPlayerCount("ModUtils");
                    mmm.NotLongerSupported = modObject.ModUnsupported;
                    return View(mmm);
                }
            }

            return RedirectToAction("MyMods", "Mods");
        }

        [HttpGet]
        public IActionResult MyEAGroups()
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            // User check
            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                MyGroupsModel mgm = new MyGroupsModel();
                List<EarlyAccessGroup> EarlyAccessGroups = earlyRepo.FindGroupFromUser(u);
                mgm.GroupList = EarlyAccessGroups;

                return View(mgm);
            }

            return RedirectToAction("MyMods", "Mods");
        }

        [HttpGet]
        public IActionResult CreateEAGroup()
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult CreateEAGroup(EAGroupModel model)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(model.GroupName))
            {
                ViewBag.Msg = "Group name can't be empty!";
                return View(model);
            }

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                EarlyAccessGroup existinGroup = earlyRepo.FindSpecificGroupFromUser(model.GroupName, u);
                if (existinGroup != null)
                {
                    ViewBag.Msg = "A group already exists with this name!";
                    return View(model);
                }

                EarlyAccessGroup createdGroup = new EarlyAccessGroup();
                createdGroup.Owner = u;
                createdGroup.Users = new List<EAS>();
                createdGroup.GroupName = model.GroupName;

                if (earlyRepo.Add(createdGroup))
                {
                    return RedirectToAction("MyEAGroups", "Mods");
                }
                else
                {
                    ViewBag.Msg = "Something went wrong while saving the group. Try again!";
                }

                return View();
            }

            ViewBag.Msg = "Something went wrong";
            return View(model);
        }

        [HttpGet]
        public IActionResult ManageEAGroup(string groupName)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(groupName))
            {
                return RedirectToAction("MyEAGroups", "Mods");
            }

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                EarlyAccessGroup existinGroup = earlyRepo.FindSpecificGroupFromUser(groupName, u);
                if (existinGroup != null)
                {
                    EAGroupModel eagpmd = new EAGroupModel();
                    eagpmd.GroupName = groupName;
                    eagpmd.Users = existinGroup.Users;

                    return View(eagpmd);
                }

                return RedirectToAction("MyEAGroups", "Mods");
            }

            return RedirectToAction("MyEAGroups", "Mods");
        }

        [HttpGet]
        public IActionResult AddUserToGroup(string group)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(group))
            {
                return RedirectToAction("MyEAGroups", "Mods");
            }

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                EarlyAccessGroup existinGroup = earlyRepo.FindSpecificGroupFromUser(group, u);
                if (existinGroup != null)
                {
                    EarlyAccessAllowedModel eaam = new EarlyAccessAllowedModel();
                    eaam.GroupName = existinGroup.GroupName;

                    return View(eaam);
                }

                return RedirectToAction("MyEAGroups", "Mods");
            }

            return RedirectToAction("MyEAGroups", "Mods");
        }

        [HttpPost]
        public IActionResult AddUserToGroup(EarlyAccessAllowedModel eaam)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(eaam.GroupName))
            {
                return RedirectToAction("MyEAGroups", "Mods");
            }

            if(string.IsNullOrEmpty(eaam.Username))
            {
                ViewBag.Msg = "Username can't be empty!";
                return View(eaam);
            }

            if (string.IsNullOrEmpty(eaam.Steam64))
            {
                ViewBag.Msg = "Steam64 can't be empty!";
                return View(eaam);
            }

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                EarlyAccessGroup existinGroup = earlyRepo.FindSpecificGroupFromUser(eaam.GroupName, u);
                if (existinGroup != null)
                {
                    EAS repeatedUser = existinGroup.Users.Where(u => u.Steam64 == eaam.Steam64).FirstOrDefault();
                    if(repeatedUser != null)
                    {
                        ViewBag.Msg = $"A user already has that SteamID on the group! (Username: {repeatedUser.Username})";
                        return View(eaam);
                    }
                    else
                    {
                        EAS newTester = new EAS();
                        newTester.Username = eaam.Username;
                        newTester.Steam64 = eaam.Steam64;

                        if(earlyRepo.AddTesterToGroup(newTester, eaam.GroupName, u))
                        {
                            return RedirectToAction("ManageEAGroup", "Mods", new { groupName = eaam.GroupName});
                        }
                        else
                        {
                            ViewBag.Msg = "Something went wrong...";
                            return View(eaam);
                        }
                    }
                }

                return RedirectToAction("MyEAGroups", "Mods");
            }

            return RedirectToAction("MyEAGroups", "Mods");
        }

        [HttpGet]
        public IActionResult RemoveUserFromGroup(string group, string user)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(group))
            {
                return RedirectToAction("MyEAGroups", "Mods");
            }

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                EarlyAccessGroup existinGroup = earlyRepo.FindSpecificGroupFromUser(group, u);
                if (existinGroup != null)
                {
                    EAS easUser = existinGroup.Users.Where(u => u.Steam64 == user).FirstOrDefault();
                    if(easUser != null)
                    {
                        if(earlyRepo.RemoveTesterFromGroup(easUser, group, u))
                        {
                            return RedirectToAction("ManageEAGroup", "Mods", new { groupName = group });

                        }
                        else
                        {
                            return RedirectToAction("MyEAGroups", "Mods");
                        }
                    }
                    else
                    {
                        return RedirectToAction("MyEAGroups", "Mods");
                    }
                }

                return RedirectToAction("MyEAGroups", "Mods");
            }

            return RedirectToAction("MyEAGroups", "Mods");

        }

        // Upload new version
        [HttpGet]
        public IActionResult EarlyAccessManager(string modId)
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
                    EarlyAccessModel eam = new EarlyAccessModel();
                    List<EarlyAccessGroup> myGroups = earlyRepo.FindGroupFromUser(u);

                    eam.ModId = modId;
                    eam.GroupsAvailable = new List<string>();
                    eam.GroupsAllowed = new List<string>();

                    foreach(EarlyAccessGroup eag in myGroups)
                    {
                        if(modObject.Allowed.Where(g => g.Group.GroupName == eag.GroupName).FirstOrDefault() != null)
                        {
                            eam.GroupsAllowed.Add(eag.GroupName);
                        }
                        else
                        {
                            eam.GroupsAvailable.Add(eag.GroupName);
                        }
                    }

                    return View(eam);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AddGroupToMod(string modId, string group)
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
                    List<EarlyAccessGroup> myGroups = earlyRepo.FindGroupFromUser(u);

                    foreach (EarlyAccessGroup eag in myGroups)
                    {
                        if (eag.GroupName == group)
                        {
                            if (modObject.Allowed.Where(g => g.Group.GroupName == eag.GroupName).FirstOrDefault() == null)
                            {
                                ModAllowed ma = new ModAllowed();

                                ma.ModIdentifierString = modId;
                                ma.Group = eag;
                                ma.Mod = modObject;

                                modObject.Allowed.Add(ma);
                                modsRepo.Update(modObject);
                                return RedirectToAction("EarlyAccessManager", "Mods", new { modId = modId } );
                            }
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult RemoveGroupFromMod(string modId, string group)
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
                    List<EarlyAccessGroup> myGroups = earlyRepo.FindGroupFromUser(u);

                    foreach (EarlyAccessGroup eag in myGroups)
                    {
                        if (eag.GroupName == group)
                        {
                            ModAllowed ma = modObject.Allowed.Where(g => g.Group.GroupName == eag.GroupName).FirstOrDefault();
                            if (ma != null)
                            {
                                modObject.Allowed.Remove(ma);
                                modsRepo.Update(modObject);
                                return RedirectToAction("EarlyAccessManager", "Mods", new { modId = modId });
                            }
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
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
