using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Objects.Repositories.Interfaces;

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
