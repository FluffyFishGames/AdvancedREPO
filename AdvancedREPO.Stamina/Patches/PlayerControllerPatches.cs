using AdvancedREPO.Utils;
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
        private static Field<PlayerController, bool>? IsJumpingField;

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
            IsJumpingField = new(typeof(PlayerController).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(e => e.Name == "IsJumping").First());
            
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
            playerController.SprintSpeed = 5f;

            // set sprint jump field to false when player is grounded
            if (GroundedField?.GetValue(playerController.CollisionGrounded) ?? false)
            {
                IsJumpingField?.SetValue(playerController, false);
            }
            
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
                IsJumpingField?.SetValue(playerController, true);// SprintJumpField.GetValue(playerController) || playerController.sprinting);
            }
            
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

        public static bool EnoughStaminaForSprint(PlayerController playerController)
        {
            return playerController.EnergyCurrent >= 1f || (Configuration.NoSlowdownDuringJump.Value && (IsJumpingField?.GetValue(playerController) ?? false));
        }

        public static float GetSprintLerpChange(PlayerController playerController)
        {
            return !Configuration.NoAccelerationDuringJump.Value || !IsJumpingField.GetValue(playerController) ? playerController.SprintAcceleration * Time.fixedDeltaTime : 0;
        }
        public static float GetStaminaDrain(PlayerController playerController)
        {
            return Configuration.NoStaminaDrainDuringJump.Value && IsJumpingField.GetValue(playerController) ? 0f : Configuration.StaminaSprintDrainRate.Value / 100f;
        }

        public static float GetStartStamina()
        {
            return Configuration.StartingStamina.Value;
        }
        public static float GetStaminaPerUpgrade()
        {
            return Configuration.StaminaPerUpgrade.Value;
        }
        /// <summary>
        /// Patch for the FixedUpdate method of the global::PlayerController.
        /// Will add a call to PlayerControllerPatches::FixedUpdate
        /// </summary>
        /// <param name="instructions">Instructions</param>
        /// <returns>Modified instructions</returns>
        [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PatchFixedUpdate(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Plugin.Log?.LogMessage("Patching PlayerController->FixedUpdate...");

            var fixedUpdateMethod = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == "FixedUpdate").First();
            var enoughStaminaForSprint = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == "EnoughStaminaForSprint").First();
            var getSprintLerpChange = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == "GetSprintLerpChange").First();
            var getStaminaDrain = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == "GetStaminaDrain").First();
            var inst = new List<CodeInstruction>(instructions);
            bool success1 = false;
            bool success2 = false;
            bool success3 = false;
            bool success4 = false;
            for (var i = 0; i < inst.Count; i++)
            {
                // add method before "OverrideSpeedTick"
                if (!success1 && 
                    (inst[i].opcode == OpCodes.Call && inst[i].operand is MethodInfo m && m.Name == "OverrideSpeedTick"))
                {
                    inst.Insert(i - 2, new CodeInstruction(OpCodes.Call, fixedUpdateMethod));
                    inst.Insert(i - 2, new CodeInstruction(OpCodes.Ldarg_0));
                    success1 = true;
                }
                // add no slowdown check to sprint
                if (!success2 && i < inst.Count - 2 && 
                    (inst[i].opcode == OpCodes.Ldfld && inst[i].operand is FieldInfo f1 && f1.Name == "EnergyCurrent") &&
                    (inst[i + 2].opcode == OpCodes.Blt_Un))
                {
                    inst[i] = new CodeInstruction(OpCodes.Call, enoughStaminaForSprint);
                    inst.RemoveAt(i + 1);
                    inst[i + 1].opcode = OpCodes.Brfalse;
                    success2 = true;
                }
                // prevent accelerating while sprint jumping
                if (!success3 && i > 0 &&
                    (inst[i - 1].opcode == OpCodes.Add) &&
                    (inst[i].opcode == OpCodes.Stfld && inst[i].operand is FieldInfo f2 && f2.Name == "SprintSpeedLerp"))
                {
                    inst.RemoveAt(i - 2);
                    inst.RemoveAt(i - 4);
                    inst.RemoveAt(i - 4);
                    inst.Insert(i - 4, new CodeInstruction(OpCodes.Call, getSprintLerpChange));
                    i -= 2;
                    success3 = true;
                }
                // change energy consumption behavior
                if (!success4 && i < inst.Count - 3 &&
                    (inst[i].opcode == OpCodes.Ldfld && inst[i].operand is FieldInfo f3 && f3.Name == "EnergyCurrent") &&
                    (inst[i + 2].opcode == OpCodes.Call && inst[i + 2].operand is MethodInfo m1 && m1.Name == "get_fixedDeltaTime") &&
                    (inst[i + 3].opcode == OpCodes.Mul))
                {
                    inst.Insert(i + 4, new CodeInstruction(OpCodes.Mul));
                    inst.Insert(i + 4, new CodeInstruction(OpCodes.Call, getStaminaDrain));
                    inst.Insert(i + 4, new CodeInstruction(OpCodes.Ldarg_0));
                    success4 = true;
                }
                if (success1 && success2 && success3 && success4)
                    break;
            }
            
            int successCount = (success1 ? 1 : 0) + (success2 ? 1 : 0) + (success3 ? 1 : 0) + (success4 ? 1 : 0);
            if (successCount == 4)
                Plugin.Log?.LogMessage("Patched PlayerController->FixedUpdate!");
            else
                Plugin.Log?.LogError($"Failed to patch PlayerController->FixedUpdate! ({successCount}/4 patches applied)");
            return inst.AsEnumerable();
        }

        /// <summary>
        /// Patch for the Update method of the global::PlayerController.
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

        /// <summary>
        /// Patch for the LateStart method of the global::LateStart.
        /// </summary>
        /// <param name="instructions">Instructions</param>
        /// <returns>Modified instructions</returns>
        [HarmonyPatch(typeof(PlayerController), "LateStart", MethodType.Enumerator)]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PatchLateStart(IEnumerable<CodeInstruction> instructions)
        {
            Plugin.Log?.LogMessage("Patching PlayerController->LateStart...");

            var energyStart = typeof(PlayerController).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(e => e.Name == "EnergyStart").First();
            var getStaminaPerUpgrade = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == "GetStaminaPerUpgrade").First();
            var getStartStamina = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == "GetStartStamina").First();
            var inst = new List<CodeInstruction>(instructions);
            bool success1 = false;
            bool success2 = false;
            for (var i = 0; i < inst.Count; i++)
            {
                if (!success1 && i < inst.Count - 1 &&
                    (inst[i].opcode == OpCodes.Ldsfld && inst[i].operand is FieldInfo f && f.Name == "instance") &&
                    (inst[i + 1].opcode == OpCodes.Ldfld && inst[i + 1].operand is FieldInfo f1 && f1.Name == "playerUpgradeStamina"))
                {
                    inst.Insert(i, new CodeInstruction(OpCodes.Stfld, energyStart));
                    inst.Insert(i, new CodeInstruction(OpCodes.Call, getStartStamina));
                    inst.Insert(i, new CodeInstruction(OpCodes.Ldloc_1));
                    success1 = true;
                }
                if (!success2 && i < inst.Count - 1 &&
                    (inst[i].opcode == OpCodes.Ldc_R4 && inst[i].operand is float fl && fl == 10f) &&
                    (inst[i + 1].opcode == OpCodes.Mul))
                {
                    inst.RemoveAt(i);
                    inst.Insert(i, new CodeInstruction(OpCodes.Call, getStaminaPerUpgrade));
                    success2 = true;
                }
                if (success1 && success2)
                    break;
            }
            var successCount = (success1 ? 1 : 0) + (success2 ? 1 : 0);
            if (successCount == 2)
                Plugin.Log?.LogMessage("Patched PlayerController->LateStart!");
            else
                Plugin.Log?.LogError($"Failed to patch PlayerController->LateStart! ({successCount}/3 patches applied)");
            return inst.AsEnumerable();
        }

    }
}