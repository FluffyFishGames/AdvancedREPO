using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace AdvancedREPO.API.Patches
{
    public class PunManagerPatches
    {
        /// <summary>
        /// Will apply all patches contained in this class
        /// </summary>
        public static void ApplyPatches()
        {
            // Patch
            Stamina.Log?.LogInfo("Patching PunManager...");
            Harmony.CreateAndPatchAll(typeof(PunManagerPatches));
            Stamina.Log?.LogInfo("Patched PunManager!");
        }

        /// <summary>
        /// Patch for the UpdateEnergyRightAway method of the global::PunManager.
        /// </summary>
        /// <param name="instructions">Instructions</param>
        /// <returns>Modified instructions</returns>
        [HarmonyPatch(typeof(PunManager), "UpdateEnergyRightAway")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PatchUpdateEnergyRightAway(IEnumerable<CodeInstruction> instructions)
        {
            Stamina.Log?.LogMessage("Patching PunManager->UpdateEnergyRightAway...");

            var getStaminaPerUpgrade = typeof(Stamina).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(Stamina.GetStaminaPerUpgrade)).First();
            var inst = new List<CodeInstruction>(instructions);
            bool success = false;
            for (var i = 0; i < inst.Count - 3; i++)
            {
                if ((inst[i].opcode == OpCodes.Ldc_R4 && inst[i].operand is float fl && fl == 10f))
                {
                    inst.RemoveAt(i);
                    inst.Insert(i, new CodeInstruction(OpCodes.Call, getStaminaPerUpgrade));
                    success = true;
                    break;
                }
            }
            if (success)
                Stamina.Log?.LogMessage("Patched PunManager->UpdateEnergyRightAway!");
            else
                Stamina.Log?.LogError("Failed to patch PunManager->UpdateEnergyRightAway!");
            return inst.AsEnumerable();
        }
    }
}
