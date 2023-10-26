using Objects;

namespace MVC.Models
{
    public class ModManageModel
    {
        public string ModId { get; set; }
        public string ModName { get; set; }
        public string Version { get; set; }
        public int RequiredGameBuild { get; set; }
        public int CPC { get; set; }
        public int CPC_ModUtils { get; set; }
        public bool EA { get; set; }
        public bool NotLongerSupported { get; set; }
        public ModManageModel(Mod mod)
        {
            ModId = mod.ModId;
            ModName = mod.Name;
            if(mod.LatestVersion != null)
            {
                Version = mod.LatestVersion.Version;
                RequiredGameBuild = mod.LatestVersion.RequiredGameBuildId;
            }

            //EA = mod.EarlyAccessEnabled;
        }
    }
}
