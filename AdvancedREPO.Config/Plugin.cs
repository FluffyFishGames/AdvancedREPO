using AdvancedREPO.Config.Patches;
using BepInEx;
using BepInEx.Logging;

namespace AdvancedREPO.Config
{
    [BepInPlugin("potatoepet.advancedrepo.config", "AdvancedREPO.Config", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource? Log;
        void Awake()
        {
            Log = base.Logger;
            Log.LogInfo("Applying AdvancedREPO.Config...");
            PunManagerPatches.ApplyPatches();
            Log.LogInfo("AdvancedREPO.Config applied!");
        }
    }
}
