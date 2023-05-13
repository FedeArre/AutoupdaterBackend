using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Objects;
using Objects.DTO;
using Objects.Repositories.Interfaces;
using System;
using System.Diagnostics;

namespace MVC.Controllers.API
{
    [Route("internal")]
    [ApiController]
    public class LocalApiController : ControllerBase
    {
        GameVersioningHandler versioning;

        IUserRepository userRepo;
        public LocalApiController(GameVersioningHandler versioning, IUserRepository userRepo)
        {
            this.versioning = versioning;
            this.userRepo = userRepo;
        }

        [HttpPost, Route("heartbeat")]
        public ActionResult<NotificationModel> Heartbeat(GameVersionDTO Versions)
        {
            versioning.GAME_PUBLIC_BUILDID = Versions.Public;
            versioning.GAME_TEST_BUILDID = Versions.Test;
            versioning.GAME_BETA_BUILDID = Versions.Beta;

            Console.WriteLine(Versions.Public);
            NotificationModel notification = new NotificationModel();
            return notification;
        }

        [HttpPost, Route("tokencheck")]
        public ActionResult<NotificationModel> TokenCheck(DiscordVerificationModel model)
        {
            NotificationModel nm = new NotificationModel();
            nm.Message = "Action could not be completed. Make sure your activation token is valid";
            if (model == null)
            {
                return BadRequest(nm);
            }

            if(String.IsNullOrEmpty(model.DiscordId) || String.IsNullOrEmpty(model.GivenToken))
            {
                return Conflict(nm);
            }
            
            User u = userRepo.FindByDiscordToken(model.GivenToken);
            if(u == null)
            {
                return NotFound(nm);
            }

            if(u.Role == UserType.Unverified)
            {
                u.Role = UserType.Modder;
                userRepo.Update(u);
                nm.Message = "Your account has been succesfully activated. You may need to log in again in order to start using Mod Manager";
                return Ok(nm);
            }
            else
            {
                nm.Message = $"Couldnt check user {u.Username} {u.Role}";
                return Accepted(nm);
            }
        }
    }
}
