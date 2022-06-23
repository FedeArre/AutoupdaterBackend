using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Objects;
using Objects.Repositories.Interfaces;

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
            return View();
        }

        [HttpPost]
        public IActionResult Create(ModModel modData)
        {
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



            return View(modData);
        }
    }
}
