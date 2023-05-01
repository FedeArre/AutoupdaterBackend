using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Objects;
using Objects.Repositories.Interfaces;
using System.Collections.Generic;

namespace MVC.Controllers
{
    public class AdminController : Controller
    {
        private IUserRepository userRepo;
        public AdminController(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }

        [HttpGet]
        public IActionResult ManageUsers()
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            return View(userRepo.FindAll());
        }

        [HttpGet]
        public IActionResult Ban(string user)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if(u != null && u.Role == UserType.AutoupdaterDev)
            {
                if(u.Username != user)
                {
                    User foundUser = userRepo.FindById(user);
                    if(foundUser != null)
                    {
                        foundUser.Role = UserType.DisabledAccount;
                        userRepo.Update(foundUser);
                    }
                }
            }

            return RedirectToAction("ManageUsers", "Admin");
        }

        [HttpGet]
        public IActionResult MarkAsModder(string user)
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && u.Role == UserType.AutoupdaterDev)
            {
                if (u.Username != user)
                {
                    User foundUser = userRepo.FindById(user);
                    if (foundUser != null)
                    {
                        foundUser.Role = UserType.Modder;
                        userRepo.Update(foundUser);
                    }
                }
            }

            return RedirectToAction("ManageUsers", "Admin");
        }

        [HttpGet]
        public IActionResult WhoIsConnected()
        {
            if (!CheckUserStatus())
                return RedirectToAction("Index", "Home");

            string token = HttpContext.Session.GetString("userLoginToken");
            User u = userRepo.FindById(TokenHandler.GetInstance().IsUserLogged(token));
            if (u != null && u.Role == UserType.AutoupdaterDev)
            {
                List<string> connectedUsers = TelemetryHandler.GetInstance().GetAllIdentifiers();
                return View(connectedUsers);
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
    }
}
