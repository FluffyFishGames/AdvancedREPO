using BepInEx.Configuration;
using BepInEx;
using System.IO;
using AdvancedREPO.Config;

namespace AdvancedREPO.Stamina
{
    /// <summary>
    /// Configuration file
    /// </summary>
    public static class Configuration
    {
        public static ConfigField<bool> NoStaminaDrainDuringJump;
        public static ConfigField<bool> NoSlowdownDuringJump;
        public static ConfigField<bool> NoAccelerationDuringJump;
        public static ConfigField<int> JumpStaminaCost;
        public static ConfigField<bool> JumpStaminaPrevent;
        public static ConfigField<int> StaminaSprintDrainRate;
        public static ConfigField<int> StaminaRechargeRate;
        public static ConfigField<int> StaminaRechargeStandingRate;
        public static ConfigField<int> StaminaRechargeCrouchingRate;
        public static ConfigField<int> StaminaRechargeCrouchingStillRate;
        public static ConfigField<int> StartingStamina;
        public static ConfigField<int> StaminaPerUpgrade;

        /// <summary>
        /// Initializes the configuration fields.
        /// </summary>
        public static void Initialize(ConfigFile configFile)
        {
            NoSlowdownDuringJump = configFile.Bind<bool>(new ConfigDefinition("Jumping", "No slow down"), true, new ConfigDescription("Do not slow down the player when stamina reaches zero while jumping.")).Sync();
            NoAccelerationDuringJump = configFile.Bind<bool>(new ConfigDefinition("Jumping", "No acceleration"), true, new ConfigDescription("Do not accelerate the player when sprint is hold while jumping.")).Sync();
            NoStaminaDrainDuringJump = configFile.Bind<bool>(new ConfigDefinition("Jumping", "No stamina drain"), false, new ConfigDescription("Do not drain stamina during jumping.")).Sync();
            JumpStaminaCost = configFile.Bind<int>(new ConfigDefinition("Jumping", "Jump stamina cost"), 0, new ConfigDescription("How much stamina should a jump cost? Zero is the default game behavior.", new AcceptableValueRange<int>(0, 100))).Sync();
            JumpStaminaPrevent = configFile.Bind<bool>(new ConfigDefinition("Jumping", "Prevent jump"), false, new ConfigDescription("Should a jump be prevented if stamina is insufficient?")).Sync();
            StaminaSprintDrainRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina sprint drain rate"), 100, new ConfigDescription("How fast should stamina drain while sprinting? 100 is the game default.", new AcceptableValueRange<int>(0, 500))).Sync();
            StaminaRechargeRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina standard recharge rate"), 100, new ConfigDescription("How fast should stamina recharge? 100 is the game default.", new AcceptableValueRange<int>(0, 1000))).Sync();
            StaminaRechargeStandingRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina recharge rate while standing"), 150, new ConfigDescription("How fast should stamina recharge while standing still? 100 is the game default.", new AcceptableValueRange<int>(0, 1000))).Sync();
            StaminaRechargeCrouchingRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina recharge rate while crouching"), 150, new ConfigDescription("How fast should stamina recharge while crouching? 100 is the game default.", new AcceptableValueRange<int>(0, 1000))).Sync();
            StaminaRechargeCrouchingStillRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina recharge rate while crouching still"), 200, new ConfigDescription("How fast should stamina recharge while crouching and not moving? 100 is the game default.", new AcceptableValueRange<int>(0, 1000))).Sync();
            StartingStamina = configFile.Bind<int>(new ConfigDefinition("Stamina", "Starting stamina"), 40, new ConfigDescription("How much stamina every player should start with.", new AcceptableValueRange<int>(0, 1000))).Sync();
            StaminaPerUpgrade = configFile.Bind<int>(new ConfigDefinition("Stamina", "Upgrade stamina"), 10, new ConfigDescription("How much stamina every upgrade gives.", new AcceptableValueRange<int>(0, 100))).Sync();
        }
    }
}
