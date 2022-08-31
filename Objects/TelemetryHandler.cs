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
        private TelemetryHandler()
        {
            ModData = new Dictionary<string, Telemetry>();
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

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
        
        public bool Add(Telemetry entity)
        {
            if (ModData.ContainsKey(entity.IP))
                ModData.Remove(entity.IP);

            entity.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            ModData.Add(entity.IP, entity);
            
            return true;
        }
        
        public int GetCurrentPlayerCount(string modId)
        {
            int playerCount = 0;

            foreach (KeyValuePair<string, Telemetry> entry in ModData)
            {
                if (entry.Value.UsingMods.Contains(modId))
                    playerCount++;
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
        }
    }
}
