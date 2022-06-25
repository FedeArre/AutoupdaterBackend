using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace MVC.Controllers
{
    public class ModsController : Controller
    {
        private IModRepository modsRepo;
        private IUserRepository userRepo;

        public ModsController(IModRepository mods, IUserRepository users)
        {
            modsRepo = mods;
            userRepo = users;
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

            if (string.IsNullOrEmpty(modData.ModDownloadLink))
            {
                ViewBag.Msg = "Mod download link can't be empty!";
                return View(modData);
            }

            Mod existingMod = modsRepo.FindById(modData.ModId);
            if(existingMod != null)
            {
                ViewBag.Msg = "A mod already exists with this id!";
                return View(modData);
            }

            Uri uriResult;
            bool result = Uri.TryCreate(modData.ModDownloadLink, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if(!result)
            {
                ViewBag.Msg = "The download link is invalid!";
                return View(modData);
            }

            // User check
            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if(u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                Mod createdMod = new Mod();
                createdMod.ModId = modData.ModId;
                createdMod.Name = modData.ModName;
                createdMod.Version = modData.ModVersion;
                createdMod.DownloadLink = modData.ModDownloadLink;
                createdMod.FileName = modData.ModFileName;
                createdMod.ModAuthor = u.Username;

                if(modsRepo.Add(createdMod))
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

        // MyMods
        [HttpGet]
        public IActionResult MyMods()
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            // User check
            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && (u.Role == UserType.Modder || u.Role == UserType.AutoupdaterDev))
            {
                return View(modsRepo.FindByAuthor(u.Username));
            }

            return View(new List<Mod>());
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
                if(modObject != null && modObject.ModAuthor == u.Username)
                {
                    ModModel mm = new ModModel();
                    mm.ModId = modObject.ModId;
                    mm.ModName = modObject.Name;
                    return View(mm);
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult NewVersion(ModModel modData)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            Uri uriResult;
            bool result = Uri.TryCreate(modData.ModDownloadLink, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!result)
            {
                ViewBag.Msg = "The download link is invalid!";
                return View(modData);
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
                    if(modsRepo.Update(modObject))
                    {
                        return RedirectToAction("MyMods", "Mods");
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
    }
}
