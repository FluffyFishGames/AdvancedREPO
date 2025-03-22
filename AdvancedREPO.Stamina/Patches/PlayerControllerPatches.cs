using AdvancedREPO.Utils;
using AdvancedStamina;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace AdvancedREPO.Stamina.Patches
{
    /// <summary>
    /// Contains patches for global::PlayerController
    /// </summary>
    public class PlayerControllerPatches
    {
        /// <summary>
        /// The SprintJump field added by pre-patcher
        /// </summary>
        private static Field<PlayerController, bool>? SprintJumpField;

        /// <summary>
        /// The JumpImpulse field
        /// </summary>
        private static Field<PlayerController, bool>? JumpImpulseField;

        /// <summary>
        /// The Grounded field of PlayerCollisionGrounded
        /// </summary>
        private static Field<PlayerCollisionGrounded, bool>? GroundedField;

        /// <summary>
        /// Will apply all patches contained in this class
        /// </summary>
        public static void ApplyPatches()
        {
            // Getting the sprint jump field added by the pre-patcher
            SprintJumpField = new(typeof(PlayerController).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(e => e.Name == "SprintJump").First());
            
            // Getting other fields
            JumpImpulseField = new(typeof(PlayerController).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(e => e.Name == "JumpImpulse").First());
            GroundedField = new(typeof(PlayerCollisionGrounded).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(e => e.Name == "Grounded").First());

            // Patch
            Plugin.Log?.LogInfo("Patching PlayerController...");
            Harmony.CreateAndPatchAll(typeof(PlayerControllerPatches));
            Plugin.Log?.LogInfo("Patched PlayerController!");
        }

        /// <summary>
        /// Changes the behavior of the PlayerController. Not to be called manually.
        /// </summary>
        /// <param name="playerController">The player controller</param>
        public static void FixedUpdate(PlayerController playerController)
        {
            // set sprint jump field to true if sprint was active when jump was initiated.
            if (JumpImpulseField?.GetValue(playerController) ?? false)
            {
                if (Configuration.JumpStaminaCost.Value != 0)
                {
                    if (!Configuration.JumpStaminaPrevent.Value || playerController.EnergyCurrent > Configuration.JumpStaminaCost.Value)
                        playerController.EnergyCurrent = Mathf.Min(playerController.EnergyStart, Mathf.Max(0, playerController.EnergyCurrent - Configuration.JumpStaminaCost.Value));
                    else if (Configuration.JumpStaminaPrevent.Value)
                        JumpImpulseField?.SetValue(playerController, false);
                }
                SprintJumpField?.SetValue(playerController, SprintJumpField.GetValue(playerController) || playerController.sprinting);
            }
            
            // set sprint jump field to false when player is grounded
            if (GroundedField?.GetValue(playerController.CollisionGrounded) ?? false)
                SprintJumpField?.SetValue(playerController, false);
        }

        /// <summary>
        /// Returns the stamina recharge rate for the player depending on the players state.
        /// </summary>
        /// <param name="playerController">The player</param>
        /// <returns>The recharge rate</returns>
        public static float GetStaminaRechargeRate(PlayerController playerController)
        {
            if (playerController.Crouching || playerController.Crawling)
                return Configuration.StaminaRechargeCrouchingRate.Value / 100f;
            if (playerController.moving)
                return Configuration.StaminaRechargeRate.Value / 100f;
            return Configuration.StaminaRechargeStandingRate.Value / 100f;
        }

        /// <summary>
        /// Patch for the FixedUpdate method of the global::PlayerController.
        /// Will add a call to PlayerControllerPatches::FixedUpdate
        /// </summary>
        /// <param name="instructions">Instructions</param>
        /// <returns>Modified instructions</returns>
        [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PatchFixedUpdate(IEnumerable<CodeInstruction> instructions)
        {
            Plugin.Log?.LogMessage("Patching PlayerController->FixedUpdate...");

            var fixedUpdateMethod = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == "FixedUpdate").First();
            var inst = new List<CodeInstruction>(instructions);
            bool success = false;
            for (var i = 0; i < inst.Count; i++)
            {
                // add method before "OverrideSpeedTick"
                if ((inst[i].opcode == OpCodes.Call && inst[i].operand is MethodInfo m && m.Name == "OverrideSpeedTick"))
                {
                    inst.Insert(i - 2, new CodeInstruction(OpCodes.Call, fixedUpdateMethod));
                    inst.Insert(i - 2, new CodeInstruction(OpCodes.Ldarg_0));
                    success = true;
                    break;
                }
            }
            if (success)
                Plugin.Log?.LogMessage("Patched PlayerController->FixedUpdate!");
            else
                Plugin.Log?.LogError("Failed to patch PlayerController->FixedUpdate!");
            return inst.AsEnumerable();
        }

        /// <summary>
        /// Patch for the Update method of the global::PlayerController.
        /// Will add a multiplication to stamina recharge with PlayerControllerPatches::GetStaminaRechargeRate
        /// </summary>
        /// <param name="instructions">Instructions</param>
        /// <returns>Modified instructions</returns>
        [HarmonyPatch(typeof(PlayerController), "Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PatchUpdate(IEnumerable<CodeInstruction> instructions)
        {
            Plugin.Log?.LogMessage("Patching PlayerController->Update...");

            var getStaminaRechargeRate = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == "GetStaminaRechargeRate").First();
            var inst = new List<CodeInstruction>(instructions);
            bool success = false;
            for (var i = 0; i < inst.Count - 3; i++)
            {
                // add method before "OverrideSpeedTick"
                if ((inst[i].opcode == OpCodes.Ldfld && inst[i].operand is FieldInfo f && f.Name == "EnergyCurrent") &&
                    (inst[i + 1].opcode == OpCodes.Ldloc_0) &&
                    (inst[i + 3].opcode == OpCodes.Mul))
                {
                    inst.Insert(i + 3, new CodeInstruction(OpCodes.Mul));
                    inst.Insert(i + 3, new CodeInstruction(OpCodes.Call, getStaminaRechargeRate));
                    inst.Insert(i + 3, new CodeInstruction(OpCodes.Ldarg_0, getStaminaRechargeRate));
                    success = true;
                    break;
                }
            }
            if (success)
                Plugin.Log?.LogMessage("Patched PlayerController->Update!");
            else
                Plugin.Log?.LogError("Failed to patch PlayerController->Update!");
            return inst.AsEnumerable();
        }

    }
}
