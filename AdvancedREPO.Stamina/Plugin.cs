using BepInEx;
using BepInEx.Logging;
using System;

namespace AdvancedREPO.Stamina
{
    [BepInPlugin("potatoepet.advancedrepo.stamina", "AdvancedREPO.Stamina", "1.0.1")]
    [BepInDependency("potatoepet.advancedrepo.api.stamina", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("potatoepet.advancedrepo.config", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource? Log;
        void Awake()
        {
            Log = base.Logger;
            Log.LogInfo("Applying AdvancedREPO.Stamina...");
            Configuration.Initialize(Config);
            Log.LogInfo("AdvancedREPO.Stamina applied!");
        }
    }
}
