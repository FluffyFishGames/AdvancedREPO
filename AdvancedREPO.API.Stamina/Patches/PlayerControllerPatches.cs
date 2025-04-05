using AdvancedREPO.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace AdvancedREPO.API.Patches
{
    /// <summary>
    /// Contains patches for global::PlayerController
    /// </summary>
    public class PlayerControllerPatches
    {
        /// <summary>
        /// The SprintJump field added by pre-patcher
        /// </summary>
        internal static Field<PlayerController, bool>? IsJumpingField;

        /// <summary>
        /// The JumpImpulse field
        /// </summary>
        internal static Field<PlayerController, bool>? JumpImpulseField;

        /// <summary>
        /// The Grounded field of PlayerCollisionGrounded
        /// </summary>
        internal static Field<PlayerCollisionGrounded, bool>? GroundedField;

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
            Stamina.Log?.LogInfo("Patching PlayerController...");
            Harmony.CreateAndPatchAll(typeof(PlayerControllerPatches));
            Stamina.Log?.LogInfo("Patched PlayerController!");
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
                if (Stamina.JumpStaminaCost != 0)
                {
                    if (!Stamina.JumpStaminaPrevent || playerController.EnergyCurrent >= Stamina.JumpStaminaCost)
                        playerController.EnergyCurrent = Mathf.Min(playerController.EnergyStart, Mathf.Max(0, playerController.EnergyCurrent - Stamina.JumpStaminaCost));
                    else if (Stamina.JumpStaminaPrevent)
                        JumpImpulseField?.SetValue(playerController, false);
                }
                IsJumpingField?.SetValue(playerController, true);// SprintJumpField.GetValue(playerController) || playerController.sprinting);
            }
            
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
            Stamina.Log?.LogMessage("Patching PlayerController->FixedUpdate...");

            var fixedUpdateMethod = typeof(PlayerControllerPatches).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(FixedUpdate)).First();
            var enoughStaminaForSprint = typeof(Stamina).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(Stamina.IsStaminaEnoughForSprint)).First();
            var getSprintLerpChange = typeof(Stamina).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(Stamina.GetSprintLerpChange)).First();
            var getStaminaDrain = typeof(Stamina).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(Stamina.GetStaminaDrainMultiplier)).First();
            var getSlideStaminaCost = typeof(Stamina).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(Stamina.GetSlideStaminaCost)).First();
            var inst = new List<CodeInstruction>(instructions);
            bool success1 = false;
            bool success2 = false;
            bool success3 = false;
            bool success4 = false;
            bool success5 = false;
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
                // change energy cost for slide
                if (!success5 &&
                    (inst[i].opcode == OpCodes.Ldc_R4 && inst[i].operand is float fl && fl == 5) &&
                    (inst[i - 1].opcode == OpCodes.Ldfld && inst[i - 1].operand is FieldInfo f4 && f4.Name == "EnergyCurrent"))
                {
                    inst[i] = new CodeInstruction(OpCodes.Call, getSlideStaminaCost);
                    success5 = true;
                }
                if (success1 && success2 && success3 && success4 && success5)
                    break;
            }
            
            int successCount = (success1 ? 1 : 0) + (success2 ? 1 : 0) + (success3 ? 1 : 0) + (success4 ? 1 : 0) + (success5 ? 1 : 0);
            if (successCount == 5)
                Stamina.Log?.LogMessage("Patched PlayerController->FixedUpdate!");
            else
                Stamina.Log?.LogError($"Failed to patch PlayerController->FixedUpdate! ({successCount}/5 patches applied)");
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
            Stamina.Log?.LogMessage("Patching PlayerController->Update...");

            var getStaminaRechargeMultiplier = typeof(Stamina).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(Stamina.GetStaminaRechargeMultiplier)).First();
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
                    inst.Insert(i + 3, new CodeInstruction(OpCodes.Call, getStaminaRechargeMultiplier));
                    inst.Insert(i + 3, new CodeInstruction(OpCodes.Ldarg_0, getStaminaRechargeMultiplier));
                    success = true;
                    break;
                }
            }
            if (success)
                Stamina.Log?.LogMessage("Patched PlayerController->Update!");
            else
                Stamina.Log?.LogError("Failed to patch PlayerController->Update!");
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
            Stamina.Log?.LogMessage("Patching PlayerController->LateStart...");

            var energyStart = typeof(PlayerController).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(e => e.Name == "EnergyStart").First();
            var getStaminaPerUpgrade = typeof(Stamina).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(Stamina.GetStaminaPerUpgrade)).First();
            var getStartStamina = typeof(Stamina).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == nameof(Stamina.GetStartStamina)).First();
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
                Stamina.Log?.LogMessage("Patched PlayerController->LateStart!");
            else
                Stamina.Log?.LogError($"Failed to patch PlayerController->LateStart! ({successCount}/3 patches applied)");
            return inst.AsEnumerable();
        }

    }
}