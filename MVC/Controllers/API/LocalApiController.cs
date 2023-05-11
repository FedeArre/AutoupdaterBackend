using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Objects;
using Objects.DTO;
using System;
using System.Diagnostics;

namespace MVC.Controllers.API
{
    [Route("internal")]
    [ApiController]
    public class LocalApiController : ControllerBase
    {
        GameVersioningHandler versioning;
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        public LocalApiController(GameVersioningHandler versioning)
        {
            this.versioning = versioning;
        }

        [HttpPost, Route("heartbeat")]
        public ActionResult<NotificationModel> Heartbeat(GameVersionDTO Versions)
        {
            PerformanceCounter
            versioning.GAME_PUBLIC_BUILDID = Versions.Public;
            versioning.GAME_TEST_BUILDID = Versions.Test;
            versioning.GAME_BETA_BUILDID = Versions.Beta;

            Console.WriteLine(Versions.Public);
            NotificationModel notification = new NotificationModel();
            return notification;
        }
    }
}
