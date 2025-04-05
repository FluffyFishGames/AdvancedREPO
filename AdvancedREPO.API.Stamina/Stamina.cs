using AdvancedREPO.API.Patches;
using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdvancedREPO.API
{
    [BepInPlugin("potatoepet.advancedrepo.api.stamina", "AdvancedREPO.API.Stamina", "1.0.0")]
    public class Stamina : BaseUnityPlugin
    {
        public static ManualLogSource? Log;
        void Awake()
        {
            Log = base.Logger;
            Log.LogInfo("Applying AdvancedREPO.API.Stamina...");
            PlayerControllerPatches.ApplyPatches();
            PunManagerPatches.ApplyPatches();
            Log.LogInfo("AdvancedREPO.API.Stamina applied!");
        }

        private static Dictionary<string, bool> NoStaminaDrainDuringJumpValues = new Dictionary<string, bool>();
        private static bool _NoStaminaDrainDuringJump = false;
        public static bool NoStaminaDrainDuringJump
        {
            get
            {
                return _NoStaminaDrainDuringJump;
            }
            internal set
            {
                _NoStaminaDrainDuringJump = value;
            }
        }

        private static Dictionary<string, bool> NoSlowdownDuringJumpValues = new Dictionary<string, bool>();
        private static bool _NoSlowdownDuringJump = false;
        public static bool NoSlowdownDuringJump
        {
            get
            {
                return _NoSlowdownDuringJump;
            }
            internal set
            {
                _NoSlowdownDuringJump = value;
            }
        }

        private static Dictionary<string, bool> NoAccelerationDuringJumpValues = new Dictionary<string, bool>();
        private static bool _NoAccelerationDuringJump = false;
        public static bool NoAccelerationDuringJump
        {
            get
            {
                return _NoAccelerationDuringJump;
            }
            internal set
            {
                _NoAccelerationDuringJump = value;
            }
        }

        private static Dictionary<string, int> JumpStaminaCostValues = new Dictionary<string, int>();
        private static int _JumpStaminaCost = 0;
        public static int JumpStaminaCost
        {
            get
            {
                return _JumpStaminaCost;
            }
            internal set
            {
                _JumpStaminaCost = value;
            }
        }

        private static Dictionary<string, bool> JumpStaminaPreventValues = new Dictionary<string, bool>();
        private static bool _JumpStaminaPrevent = false;
        public static bool JumpStaminaPrevent
        {
            get
            {
                return _JumpStaminaPrevent;
            }
            internal set
            {
                _JumpStaminaPrevent = value;
            }
        }

        private static Dictionary<string, float> StaminaSprintDrainRateValues = new Dictionary<string, float>();
        private static float _StaminaSprintDrainRate = 1f;
        public static float StaminaSprintDrainRate
        {
            get
            {
                return _StaminaSprintDrainRate;
            }
            internal set
            {
                _StaminaSprintDrainRate = value;
            }
        }

        private static Dictionary<string, float> StaminaRechargeRateValues = new Dictionary<string, float>();
        private static float _StaminaRechargeRate = 1f;
        public static float StaminaRechargeRate
        {
            get
            {
                return _StaminaRechargeRate;
            }
            internal set
            {
                _StaminaRechargeRate = value;
            }
        }

        private static Dictionary<string, float> StaminaStandingRechargeMultiplierValues = new Dictionary<string, float>();
        private static float _StaminaStandingRechargeMultiplier = 1f;
        public static float StaminaStandingRechargeMultiplier
        {
            get
            {
                return _StaminaStandingRechargeMultiplier;
            }
            internal set
            {
                _StaminaStandingRechargeMultiplier = value;
            }
        }

        private static Dictionary<string, float> StaminaCrouchingRechargeMultiplierValues = new Dictionary<string, float>();
        private static float _StaminaCrouchingRechargeMultiplier = 1f;
        public static float StaminaCrouchingRechargeMultiplier
        {
            get
            {
                return _StaminaCrouchingRechargeMultiplier;
            }
            internal set
            {
                _StaminaCrouchingRechargeMultiplier = value;
            }
        }

        private static Dictionary<string, int> StartingStaminaValues = new Dictionary<string, int>();
        private static int _StartingStamina = 40;
        public static int StartingStamina
        {
            get
            {
                return _StartingStamina;
            }
            internal set
            {
                _StartingStamina = value;
            }
        }

        private static Dictionary<string, int> StaminaPerUpgradeValues = new Dictionary<string, int>();
        private static int _StaminaPerUpgrade = 10;
        public static int StaminaPerUpgrade
        {
            get
            {
                return _StaminaPerUpgrade;
            }
            internal set
            {
                _StaminaPerUpgrade = value;
            }
        }

        private static Dictionary<string, int> SlideStaminaCostValues = new Dictionary<string, int>();
        private static int _SlideStaminaCost = 5;
        public static int SlideStaminaCost
        {
            get
            {
                return _SlideStaminaCost;
            }
            internal set
            {
                _SlideStaminaCost = value;
            }
        }

        /// <summary>
        /// Changes if stamina can drain during jumps. The result is OR, so any mod setting this to true will result in true.
        /// </summary>
        /// <param name="status">If stamina should be drained during jump</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void SetNoStaminaDrainDuringJump(bool? status, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (status.HasValue)
            {
                NoStaminaDrainDuringJumpValues[modName + key] = status.Value;
                NoStaminaDrainDuringJump = status.Value || NoStaminaDrainDuringJumpValues.Any(x => x.Value);
            }
            else
            {
                NoStaminaDrainDuringJumpValues.Remove(modName + key);
                NoStaminaDrainDuringJump = NoStaminaDrainDuringJumpValues.Any(x => x.Value);
            }
            Log?.LogDebug($"Mod {modName} set no stamina drain during jump for key {key} to {status}. New value is {NoStaminaDrainDuringJump}");
        }

        /// <summary>
        /// Changes if player should slow down during jump when stamina runs out. The result is OR, so any mod setting this to true will result in true.
        /// </summary>
        /// <param name="status">If player should slow down on no stamina</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void SetNoSlowdownDuringJump(bool? status, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (status.HasValue)
            {
                NoSlowdownDuringJumpValues[modName + key] = status.Value;
                NoSlowdownDuringJump = status.Value || NoSlowdownDuringJumpValues.Any(x => x.Value);
            }
            else
            {
                NoSlowdownDuringJumpValues.Remove(modName + key);
                NoSlowdownDuringJump = NoSlowdownDuringJumpValues.Any(x => x.Value);
            }
            Log?.LogDebug($"Mod {modName} set no slowdown during jump for key {key} to {status}. New value is {NoSlowdownDuringJump}");
        }

        /// <summary>
        /// Changes if player should be able to increase speed during jump when sprinting in jump. The result is OR, so any mod setting this to true will result in true.
        /// </summary>
        /// <param name="status">If player should be able to increase speed during jump</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void SetNoAccelerationDuringJump(bool? status, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (status.HasValue)
            {
                NoAccelerationDuringJumpValues[modName + key] = status.Value;
                NoAccelerationDuringJump = status.Value || NoAccelerationDuringJumpValues.Any(x => x.Value);
            }
            else
            {
                NoAccelerationDuringJumpValues.Remove(modName + key);
                NoAccelerationDuringJump = NoAccelerationDuringJumpValues.Any(x => x.Value);
            }
            Log?.LogDebug($"Mod {modName} set no acceleration during jump for key {key} to {status}. New value is {NoAccelerationDuringJump}");
        }

        /// <summary>
        /// Changes the stamina cost for a jump. The resulting value is additive between all values of all mods and keys.
        /// 
        /// So if Mod A adds 10 stamina cost and Mod B adds 15 stamina cost, a jump will cost 25 stamina.
        /// </summary>
        /// <param name="cost">The stamina cost for a jump</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void AddJumpStaminaCost(int? cost, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (cost.HasValue)
                JumpStaminaCostValues[modName + key] = cost.Value;
            else
                JumpStaminaCostValues.Remove(modName + key);
            JumpStaminaCost = JumpStaminaCostValues.Sum(x => x.Value);
            Log?.LogDebug($"Mod {modName} set added jump stamina cost for key {key} to {cost}. New value is {JumpStaminaCost}");
        }

        /// <summary>
        /// Changes if player should only be able to jump with sufficient stamina. The result is OR, so any mod setting this to true will result in true.
        /// </summary>
        /// <param name="status">If player should only be able to jump with enough stamina</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void SetJumpStaminaPrevent(bool? status, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (status.HasValue)
            {
                JumpStaminaPreventValues[modName + key] = status.Value;
                JumpStaminaPrevent = status.Value || JumpStaminaPreventValues.Any(x => x.Value);
            }
            else
            {
                JumpStaminaPreventValues.Remove(modName + key);
                JumpStaminaPrevent = JumpStaminaPreventValues.Any(x => x.Value);
            }
            Log?.LogDebug($"Mod {modName} set jump stamina prevent for key {key} to {status}. New value is {JumpStaminaPrevent}");
        }

        /// <summary>
        /// Changes the rate at which stamina drains during sprinting. The result is multiplicative.
        /// 
        /// So if Mod A sets this to 1.5 and Mod B sets this to 0.5 the result is 0.75
        /// </summary>
        /// <param name="value">The rate at which the stamina drains during sprinting</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void SetStaminaSprintDrainRate(float? value, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (value.HasValue)
                StaminaSprintDrainRateValues[modName + key] = value.Value;
            else
                StaminaSprintDrainRateValues.Remove(modName + key);
            var val = 1f;
            foreach (var kv in StaminaSprintDrainRateValues)
                val = val * kv.Value;
            StaminaSprintDrainRate = val;
            Log?.LogDebug($"Mod {modName} set stamina sprint drain rate for key {key} to {value}. New value is {StaminaSprintDrainRate}");
        }

        /// <summary>
        /// Changes the rate at which stamina recharges. The result is multiplicative.
        /// 
        /// So if Mod A sets this to 1.5 and Mod B sets this to 0.5 the result is 0.75
        /// </summary>
        /// <param name="value">The rate at which the stamina recharges</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void SetStaminaRechargeRate(float? value, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (value.HasValue)
                StaminaRechargeRateValues[modName + key] = value.Value;
            else
                StaminaRechargeRateValues.Remove(modName + key);

            var val = 1f;
            foreach (var kv in StaminaRechargeRateValues)
                val = val * kv.Value;
            StaminaRechargeRate = val;
            Log?.LogDebug($"Mod {modName} set stamina recharge rate for key {key} to {value}. New value is {StaminaRechargeRate}");
        }

        /// <summary>
        /// Changes the multiplier at which stamina recharges while standing still. The result is multiplicative.
        /// 
        /// So if Mod A sets this to 1.5 and Mod B sets this to 0.5 the result is 0.75
        /// This value is applied as a modifier to the base recharge rate.
        /// </summary>
        /// <param name="value">The multiplier at which the stamina recharges while standing still</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void SetStandingStaminaRechargeMultiplier(float? value, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (value.HasValue)
                StaminaStandingRechargeMultiplierValues[modName + key] = value.Value;
            else
                StaminaStandingRechargeMultiplierValues.Remove(modName + key);

            var val = 1f;
            foreach (var kv in StaminaStandingRechargeMultiplierValues)
                val = val * kv.Value;
            StaminaStandingRechargeMultiplier = val;
            Log?.LogDebug($"Mod {modName} set standing stamina recharge multiplier for key {key} to {value}. New value is {StaminaStandingRechargeMultiplier}");
        }

        /// <summary>
        /// Changes the multiplier at which stamina recharges while crouching. The result is multiplicative.
        /// 
        /// So if Mod A sets this to 1.5 and Mod B sets this to 0.5 the result is 0.75
        /// This value is applied as a modifier to the base recharge rate.
        /// </summary>
        /// <param name="value">The multiplier at which the stamina recharges while crouching</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void SetCrouchingStaminaRechargeMultiplier(float? value, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (value.HasValue)
                StaminaCrouchingRechargeMultiplierValues[modName + key] = value.Value;
            else 
                StaminaCrouchingRechargeMultiplierValues.Remove(modName + key);

            var val = 1f;
            foreach (var kv in StaminaCrouchingRechargeMultiplierValues)
                val = val * kv.Value;
            StaminaCrouchingRechargeMultiplier = val;
            Log?.LogDebug($"Mod {modName} set crouching stamina recharge multiplier for key {key} to {value}. New value is {StaminaCrouchingRechargeMultiplier}");
        }

        /// <summary>
        /// Changes the starting stamina. The result is additive.
        /// 
        /// So if Mod A sets this to 10 and Mod B sets this to 20 the result is 70 (including 40 vanilla stamina)
        /// </summary>
        /// <param name="value">The starting stamina to be added</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void AddStartStamina(int? value, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (value.HasValue)
                StartingStaminaValues[modName + key] = value.Value;
            else
                StartingStaminaValues.Remove(modName + key);

            StartingStamina = 40 + StartingStaminaValues.Sum(x => x.Value);
            Log?.LogDebug($"Mod {modName} set added starting stamina for key {key} to {value}. New value is {StartingStamina}");
        }

        /// <summary>
        /// Changes the stamina per upgrade. The result is additive.
        /// 
        /// So if Mod A sets this to 10 and Mod B sets this to 20 the result is 40 (including 10 vanilla stamina per upgrade)
        /// </summary>
        /// <param name="value">The stamina to be added per upgrade</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void AddStaminaPerUpgrade(int? value, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (value.HasValue)
                StaminaPerUpgradeValues[modName + key] = value.Value;
            else 
                StaminaPerUpgradeValues.Remove(modName + key);

            StaminaPerUpgrade = 10 + StaminaPerUpgradeValues.Sum(x => x.Value);
            Log?.LogDebug($"Mod {modName} set added stamina per upgrade for key {key} to {value}. New value is {StaminaPerUpgrade}");
        }

        /// <summary>
        /// Changes the stamina needed for sliding. The result is additive.
        /// 
        /// So if Mod A sets this to 10 and Mod B sets this to 20 the result is 35 (including 5 vanilla cost for slide)
        /// </summary>
        /// <param name="cost">The stamina to be added per upgrade</param>
        /// <param name="key">The name of the mod. If null will automatically be set to assembly name of calling assembly.</param>
        /// <param name="key">A key for the value</param>
        public static void AddSlideStaminaCost(int? cost, string modName = null, string key = null)
        {
            if (modName == null)
                modName = Assembly.GetCallingAssembly()?.GetName()?.Name ?? "";
            if (cost.HasValue)
                SlideStaminaCostValues[modName + key] = cost.Value;
            else 
                SlideStaminaCostValues.Remove(modName + key);

            SlideStaminaCost = 10 + SlideStaminaCostValues.Sum(x => x.Value);
            Log?.LogDebug($"Mod {modName} set added slide stamina cost for key {key} to {cost}. New value is {SlideStaminaCost}");
        }

        /// <summary>
        /// Returns the stamina recharge rate multiplier for the player depending on the players state.
        /// </summary>
        /// <param name="playerController">The player</param>
        /// <returns>The recharge rate</returns>
        public static float GetStaminaRechargeMultiplier(PlayerController playerController)
        {
            var ret = StaminaRechargeRate * (playerController.moving ? 1f : StaminaStandingRechargeMultiplier) * (playerController.Crouching ? StaminaCrouchingRechargeMultiplier : 1f);
            //Log?.LogDebug("Recharge: " + ret + " (" + StaminaStandingRechargeMultiplier + ", " + StaminaCrouchingRechargeMultiplier + ", " + StaminaRechargeRate + ")");
            return ret;
        }

        /// <summary>
        /// Returns if the player has enough stamina to sprint (will return true if jumping and no slowdown during jump is active)
        /// </summary>
        /// <param name="playerController">The PlayerController</param>
        /// <returns>Whether sprint speed should be applied to the player</returns>
        public static bool IsStaminaEnoughForSprint(PlayerController playerController)
        {
            return playerController.EnergyCurrent >= 1f || (NoSlowdownDuringJump && (PlayerControllerPatches.IsJumpingField?.GetValue(playerController) ?? false));
        }

        /// <summary>
        /// How much to add to the time of the lerp of the sprint speed. If acceleration in jump is deactivated this will return 0.
        /// </summary>
        /// <param name="playerController">The PlayerController</param>
        /// <returns>The value to add to t</returns>
        public static float GetSprintLerpChange(PlayerController playerController)
        {
            return !NoAccelerationDuringJump || !(PlayerControllerPatches.IsJumpingField?.GetValue(playerController) ?? false) ? playerController.SprintAcceleration * UnityEngine.Time.fixedDeltaTime : 0;
        }

        /// <summary>
        /// Returns a multiplier for the stamina drain. If no stamina drain during jumping is active it returns 0.
        /// </summary>
        /// <param name="playerController">The PlayerController</param>
        /// <returns>A multiplier for stamina drain</returns>
        public static float GetStaminaDrainMultiplier(PlayerController playerController)
        {
            return NoStaminaDrainDuringJump && (PlayerControllerPatches.IsJumpingField?.GetValue(playerController) ?? false) ? 0f : StaminaSprintDrainRate;
        }

        /// <summary>
        /// Returns how much stamina is the baseline
        /// </summary>
        /// <returns>The base stamina</returns>
        public static float GetStartStamina()
        {
            return StartingStamina;
        }

        /// <summary>
        /// Returns how much stamina an upgrade give
        /// </summary>
        /// <returns>Stamina per upgrade</returns>
        public static float GetStaminaPerUpgrade()
        {
            return StaminaPerUpgrade;
        }

        /// <summary>
        /// Returns how much stamina a slide costs
        /// </summary>
        /// <returns>Stamina cost</returns>
        public static float GetSlideStaminaCost()
        {
            return SlideStaminaCost;
        }

    }
}
