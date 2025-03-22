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
        public static ConfigField<float> JumpStaminaCost;
        public static ConfigField<bool> JumpStaminaPrevent;
        public static ConfigField<float> StaminaSprintDrainRate;
        public static ConfigField<float> StaminaRechargeRate;
        public static ConfigField<float> StaminaRechargeStandingRate;
        public static ConfigField<float> StaminaRechargeCrouchingRate;
        public static ConfigField<float> StartingStamina;
        public static ConfigField<float> StaminaPerUpgrade;
        public static void Initialize()
        {
            var configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "AdvancedREPO/AdvancedStamina.cfg"), true);

            NoSlowdownDuringJump = configFile.Bind<bool>("Jumping", "No slow down", true, "Do not slow down the player when stamina reaches zero while jumping.").Sync();
            NoAccelerationDuringJump = configFile.Bind<bool>("Jumping", "No acceleration", true, "Do not accelerate the player when sprint is hold while jumping.").Sync();
            NoStaminaDrainDuringJump = configFile.Bind<bool>("Jumping", "No stamina drain", true, "Do not drain stamina during jumping.").Sync();
            JumpStaminaCost = configFile.Bind<float>("Jumping", "Jump stamina cost", 0f, "How much stamina should a jump cost? Zero is the default game behavior.").Sync();
            JumpStaminaPrevent = configFile.Bind<bool>("Jumping", "Prevent jump", false, "Should a jump be prevented if stamina is insufficient?").Sync();
            StaminaSprintDrainRate = configFile.Bind<float>("Stamina", "Stamina sprint drain rate", 100f, "How fast should stamina drain while sprinting? 100 is the game default.").Sync();
            StaminaRechargeRate = configFile.Bind<float>("Stamina", "Stamina standard recharge rate", 100f, "How fast should stamina recharge? 100 is the game default.").Sync();
            StaminaRechargeStandingRate = configFile.Bind<float>("Stamina", "Stamina recharge rate while standing", 200f, "How fast should stamina recharge while standing still? 100 is the game default.").Sync();
            StaminaRechargeCrouchingRate = configFile.Bind<float>("Stamina", "Stamina recharge rate while crouching", 150f, "How fast should stamina recharge while crouching? When standing still while crouching the standing recharge rate is used instead. 100 is the game default.").Sync();
            StartingStamina = configFile.Bind<float>("Stamina", "Starting stamina", 40f, "How much stamina every player should start with.").Sync();
            StaminaPerUpgrade = configFile.Bind<float>("Stamina", "Upgrade stamina", 10f, "How much stamina every upgrade gives.").Sync();
        }
    }
}
