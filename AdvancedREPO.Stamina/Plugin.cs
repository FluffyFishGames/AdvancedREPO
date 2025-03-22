using AdvancedREPO.Stamina;
using AdvancedREPO.Stamina.Patches;
using BepInEx;
using BepInEx.Logging;
using System;

namespace AdvancedREPO.Stamina
{
    [BepInPlugin("potatoepet.advancedrepo.stamina", "AdvancedREPO.Stamina", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource? Log;
        void Awake()
        {
            Log = base.Logger;
            Configuration.Initialize(Config);
            Log.LogInfo("Applying AdvancedREPO.Stamina...");
            PlayerControllerPatches.ApplyPatches();
            PunManagerPatches.ApplyPatches();
            Log.LogInfo("AdvancedREPO.Stamina applied!");
        }
    }
}
