using Microsoft.Extensions.DependencyInjection;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Objects
{
    public class TelemetryHandler
    {
        // Singleton
        private static TelemetryHandler Instance;
        private Timer timer;
        public IServiceScope Services;
        
        private TelemetryHandler()
        {
            ModData = new Dictionary<string, Telemetry>();
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(20);

            timer = new System.Threading.Timer((e) =>
            {
                StillAlive();
            }, null, startTimeSpan, periodTimeSpan);
        }
        
        public static TelemetryHandler GetInstance()
        {
            if (Instance == null)
                Instance = new TelemetryHandler();
            return Instance;
        }
        
        private Dictionary<string, Telemetry> ModData = new Dictionary<string, Telemetry>();
        public Dictionary<string, int> Peak24 = new Dictionary<string, int>();
        
        public bool Add(Telemetry entity)
        {
            if (ModData.ContainsKey(entity.IP))
                ModData.Remove(entity.IP);

            entity.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            ModData.Add(entity.IP, entity);
            
            return true;
        }
        
        public int GetCurrentPlayerCount(string modId, bool repeating = false)
        {
            int playerCount = 0;

            try
            {
                foreach (KeyValuePair<string, Telemetry> entry in ModData)
                {
                    if (entry.Value.UsingMods.Contains(modId))
                        playerCount++;
                }

            }
            catch (Exception ex)
            {
                // Second try
                if(!repeating)
                    playerCount = GetCurrentPlayerCount(modId, true);
            }
            
            return playerCount;
        }

        public void StillAlive()
        {
            List<string> keysToRemove = new List<string>();
            
            foreach (KeyValuePair<string, Telemetry> entry in ModData)
            {
                long expirationTime = entry.Value.Timestamp + (60 * 3);
                if (DateTimeOffset.Now.ToUnixTimeSeconds() > expirationTime)
                {
                    keysToRemove.Add(entry.Key);
                }
            }

            foreach(string s in keysToRemove)
            {
                ModData.Remove(s);
            }

            if(Services == null)
                return;
            
            IEnumerable<Mod> allMods = Services.ServiceProvider.GetRequiredService<IModRepository>().FindAll().ToList();
            foreach (Mod m in allMods)
            {
                if(!Peak24.ContainsKey(m.ModId))
                {
                    Peak24.Add(m.ModId, GetCurrentPlayerCount(m.ModId));
                }
                else
                {
                    int modCPC = GetCurrentPlayerCount(m.ModId);
                    if (modCPC > Peak24[m.ModId])
                    {
                        Peak24[m.ModId] = modCPC;
                    }
                }
            }
        }

        public List<string> GetAllIdentifiers()
        {
            List<string> toReturn = new List<string>();
            foreach (KeyValuePair<string, Telemetry> entry in ModData)
            {
                toReturn.Add(entry.Key);
            }
            return toReturn;
        }
    }
}
