using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.DiscordBot;
using Objects;
using Objects.Repositories.Interfaces;

namespace MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository userRepo;

        public UserController(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }

        // Register
        [HttpGet]
        public IActionResult Register()
        {
            CheckUserStatus();
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            if (CheckUserStatus())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Msg = "The username is required";
                return View();
            }

            ViewBag.Username = username;

            if(string.IsNullOrEmpty(password))
            {
                ViewBag.Msg = "The password is required";
                return View();
            }

            if(password.Length < 6)
            {
                ViewBag.Msg = "The password length has to be at least 6 characters";
                return View();
            }

            User newUser = new User();
            newUser.Username = username;
            newUser.Password = password;
            newUser.Role = UserType.Unverified;

            if(userRepo.Add(newUser))
            {
                string token = TokenHandler.GetInstance().CreateTokenForUser(newUser.Username);
                HttpContext.Session.SetString("userLoginToken", token);
                HttpContext.Session.SetInt32("userRoleId", 0);
                HttpContext.Session.SetString("footerMessage", $"Currently logged as " + newUser.Username);
                BotHandler.GetInstance().SendDiscordMessage($"New user registered on developer page: {newUser.Username}");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Msg = "This username already exists on the database!";
            }

            return View();
        }

        // Login
        public IActionResult Login()
        {
            CheckUserStatus();
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (CheckUserStatus())
                return RedirectToAction("Index", "Home");


            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Msg = "The username is required";
                return View();
            }

            ViewBag.Username = username;

            if (string.IsNullOrEmpty(password))
            {
                ViewBag.Msg = "The password is required";
                return View();
            }

            if (password.Length < 6)
            {
                ViewBag.Msg = "The password length has to be at least 6 characters";
                return View();
            }

            User user = userRepo.Login(username, password);
            if(user == null)
            {
                ViewBag.Msg = "Wrong login details";
                return View();
            }
            else
            {
                string tempToken = TokenHandler.GetInstance().IsUserLogged(user);
                if (tempToken == null)
                {
                    tempToken = TokenHandler.GetInstance().CreateTokenForUser(user.Username);
                }

                HttpContext.Session.SetString("userLoginToken", tempToken);
                HttpContext.Session.SetInt32("userRoleId", (int)user.Role);
                HttpContext.Session.SetString("footerMessage", $"Currently logged as " + user.Username);

                return RedirectToAction("Index", "Home");
            }
        }

        // Logout
        public IActionResult Logout()
        {
            string token = HttpContext.Session.GetString("userLoginToken");
            string username = TokenHandler.GetInstance().IsUserLogged(token);

            HttpContext.Session.Remove("userLoginToken");
            HttpContext.Session.SetInt32("userRoleId", 0);
            HttpContext.Session.Remove("footerMessage");

            if (username != null)
            {
                TokenHandler.GetInstance().DestroyUserToken(username);
            }

            return RedirectToAction("Index", "Home");
        }

        public bool CheckUserStatus()
        {
            string currentToken = HttpContext.Session.GetString("userLoginToken");

            if (currentToken == null)
                return false;

            if(TokenHandler.GetInstance().IsUserLogged(currentToken) == null)
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
