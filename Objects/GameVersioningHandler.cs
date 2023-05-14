using System;
using System.Collections.Generic;
using System.Text;

namespace Objects
{
    public class GameVersioningHandler
    {
        public int GAME_PUBLIC_BUILDID = 0;
        public int GAME_TEST_BUILDID = 0;
        public int GAME_BETA_BUILDID = 0;

        public Dictionary<string, int> GetVersions()
        {
            Dictionary<string, int> versions = new Dictionary<string, int>
            {
                { "Public", GAME_PUBLIC_BUILDID },
                { "Test", GAME_TEST_BUILDID },
                { "Beta", GAME_BETA_BUILDID }
            };

            return versions;
        }
    }
}
