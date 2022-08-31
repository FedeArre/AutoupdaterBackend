using System;
using System.Collections.Generic;
using System.Text;

namespace Objects.Repositories.Interfaces
{
    public interface ITelemetryRepository
    {
        public bool Add(Telemetry entity);
        public int GetCurrentPlayerCount(string modId);
    }
}
