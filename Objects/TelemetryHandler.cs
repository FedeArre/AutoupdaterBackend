using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public IServiceScope Services;

        private TelemetryHandler()
        {
            ModData = new Dictionary<string, Telemetry>();
        }

        public static TelemetryHandler GetInstance()
        {
            if (Instance == null)
                Instance = new TelemetryHandler();
            return Instance;
        }
        
        private Dictionary<string, Telemetry> ModData = new Dictionary<string, Telemetry>();
        public Dictionary<string, int> MinutePeaks = new Dictionary<string, int>();

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

            try
            {
                foreach (KeyValuePair<string, Telemetry> entry in ModData.ToList()) // Create a copy of the ModData to work safely on it
                {
                    if (entry.Value.UsingMods.Contains(modId))
                        playerCount++;
                }

            }
            catch (Exception) { return -1; }
            
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

        public List<string> GetAllIdentifiers()
        {
            List<string> toReturn = new List<string>();
            foreach (KeyValuePair<string, Telemetry> entry in ModData)
            {
                toReturn.Add(entry.Key);
            }
            return toReturn;
        }

        public void PeakCheck()
        {
            // First, we get a per mod count
            Dictionary<string, int> Counts = new Dictionary<string, int>();

            foreach (KeyValuePair<string, Telemetry> entry in ModData.ToList()) // Create a copy of ModData to work safely on it
            {
                if (!Counts.ContainsKey(entry.Key))
                    Counts.Add(entry.Key, 1);
                else
                    Counts[entry.Key]++;
            }

            // Then we compare to our current peaks data and change it to the new value if the other is lower
            foreach(KeyValuePair<string, int> entry in Counts)
            {
                if (!MinutePeaks.ContainsKey(entry.Key))
                    MinutePeaks.Add(entry.Key, entry.Value);
                else
                {
                    if (Counts[entry.Key] > MinutePeaks[entry.Key])
                        MinutePeaks[entry.Key] = Counts[entry.Key];
                }
            }
        }
    }
}
