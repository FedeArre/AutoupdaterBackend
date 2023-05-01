using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MVC.BackgroundTasks
{
    public class TelemetryService : IHostedService
    {
        private System.Threading.Timer _timer;
        private System.Threading.Timer _timer2;
        private System.Threading.Timer _timer3;

        public AutoupdaterContext _context;
        public IModRepository _modsRepo;

        public TelemetryService(AutoupdaterContext context, IModRepository modsRepo)
        {
            _context = context;
            _modsRepo = modsRepo;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(20);
            var periodTimeSpan2 = TimeSpan.FromSeconds(60);
            var periodTimeSpan3 = TimeSpan.FromHours(24);

            // KeepAlive timer
            _timer = new System.Threading.Timer((e) =>
            {
                // We check every 20 seconds if the connections are still alive and remove them if they are not
                TelemetryHandler.GetInstance().StillAlive();
            }, null, startTimeSpan, periodTimeSpan);

            // CheckPeak
            _timer2 = new System.Threading.Timer((e) =>
            {
                // We check every 60 seconds if we have a hourly peak.
                TelemetryHandler.GetInstance().PeakCheck();
            }, null, startTimeSpan, periodTimeSpan2);

            // DailyCheck
            _timer3 = new System.Threading.Timer((e) =>
            {
                DailyCheck();
            }, null, startTimeSpan, periodTimeSpan3);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void DailyCheck()
        {
            foreach(KeyValuePair<string, int> entry in TelemetryHandler.GetInstance().MinutePeaks)
            {
                // First, we ignore mods that are not registered on the Mod manager.
                if (_modsRepo.FindById(entry.Key) == null)
                    continue;

                // We create a ModDailyData object and add it to the database.
                ModDailyData mdd = new ModDailyData(entry.Key, entry.Value, DateTime.Now.Date);
                _context.ModDailyData.Add(mdd);
            }

            // After this, we apply the changes to the database.
            _context.SaveChanges();

            // Last, we delete all existing data from the MinutePeaks dictionary
            TelemetryHandler.GetInstance().MinutePeaks = new Dictionary<string, int>();
        }
    }
}
