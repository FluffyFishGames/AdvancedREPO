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
        public static ConfigField<int> StartingStamina;
        public static ConfigField<int> StaminaPerUpgrade;
        public static ConfigField<int> SlideStaminaCost;

        /// <summary>
        /// Initializes the configuration fields.
        /// </summary>
        public static void Initialize(ConfigFile configFile)
        {
            NoSlowdownDuringJump = configFile.Bind<bool>(new ConfigDefinition("Jumping", "No slow down"), true, new ConfigDescription("Do not slow down the player when stamina reaches zero while jumping.")).Sync();
            API.Stamina.SetNoSlowdownDuringJump(NoSlowdownDuringJump.Value);
            NoSlowdownDuringJump.ValueChanged += (s, e) => { API.Stamina.SetNoSlowdownDuringJump(NoSlowdownDuringJump.Value); };
            
            NoAccelerationDuringJump = configFile.Bind<bool>(new ConfigDefinition("Jumping", "No acceleration"), true, new ConfigDescription("Do not accelerate the player when sprint is hold while jumping.")).Sync();
            API.Stamina.SetNoAccelerationDuringJump(NoAccelerationDuringJump.Value);
            NoAccelerationDuringJump.ValueChanged += (s, e) => { API.Stamina.SetNoAccelerationDuringJump(NoAccelerationDuringJump.Value); };
            
            NoStaminaDrainDuringJump = configFile.Bind<bool>(new ConfigDefinition("Jumping", "No stamina drain"), false, new ConfigDescription("Do not drain stamina during jumping.")).Sync();
            API.Stamina.SetNoStaminaDrainDuringJump(NoStaminaDrainDuringJump.Value);
            NoStaminaDrainDuringJump.ValueChanged += (s, e) => { API.Stamina.SetNoStaminaDrainDuringJump(NoStaminaDrainDuringJump.Value); };
            
            JumpStaminaCost = configFile.Bind<int>(new ConfigDefinition("Jumping", "Jump stamina cost"), 0, new ConfigDescription("How much stamina should a jump cost? Zero is the default game behavior.", new AcceptableValueRange<int>(0, 100))).Sync();
            API.Stamina.AddJumpStaminaCost(JumpStaminaCost.Value);
            JumpStaminaCost.ValueChanged += (s, e) => { API.Stamina.AddJumpStaminaCost(JumpStaminaCost.Value); };
            
            JumpStaminaPrevent = configFile.Bind<bool>(new ConfigDefinition("Jumping", "Prevent jump"), false, new ConfigDescription("Should a jump be prevented if stamina is insufficient?")).Sync();
            API.Stamina.SetJumpStaminaPrevent(JumpStaminaPrevent.Value);
            JumpStaminaPrevent.ValueChanged += (s, e) => { API.Stamina.SetJumpStaminaPrevent(JumpStaminaPrevent.Value); };
            
            StaminaSprintDrainRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina sprint drain rate"), 100, new ConfigDescription("How fast should stamina drain while sprinting? 100 is the game default.", new AcceptableValueRange<int>(0, 500))).Sync();
            API.Stamina.SetStaminaSprintDrainRate(StaminaSprintDrainRate.Value / 100f);
            StaminaSprintDrainRate.ValueChanged += (s, e) => { API.Stamina.SetStaminaSprintDrainRate(StaminaSprintDrainRate.Value / 100f); };
            
            StaminaRechargeRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina standard recharge rate"), 100, new ConfigDescription("How fast should stamina recharge? 100 is the game default.", new AcceptableValueRange<int>(0, 1000))).Sync();
            API.Stamina.SetStaminaRechargeRate(StaminaRechargeRate.Value / 100f);
            StaminaRechargeRate.ValueChanged += (s, e) => { API.Stamina.SetStaminaRechargeRate(StaminaRechargeRate.Value / 100f); };
            
            StaminaRechargeStandingRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina recharge multiplier while standing"), 150, new ConfigDescription("How fast should stamina recharge while standing still? 100 is the game default.", new AcceptableValueRange<int>(0, 1000))).Sync();
            API.Stamina.SetStandingStaminaRechargeMultiplier(StaminaRechargeStandingRate.Value / 100f);
            StaminaRechargeStandingRate.ValueChanged += (s, e) => { API.Stamina.SetStandingStaminaRechargeMultiplier(StaminaRechargeStandingRate.Value / 100f); };
            
            StaminaRechargeCrouchingRate = configFile.Bind<int>(new ConfigDefinition("Stamina", "Stamina recharge multiplier while crouching"), 150, new ConfigDescription("How fast should stamina recharge while crouching? 100 is the game default.", new AcceptableValueRange<int>(0, 1000))).Sync();
            API.Stamina.SetCrouchingStaminaRechargeMultiplier(StaminaRechargeCrouchingRate.Value / 100f);
            StaminaRechargeCrouchingRate.ValueChanged += (s, e) => { API.Stamina.SetCrouchingStaminaRechargeMultiplier(StaminaRechargeCrouchingRate.Value / 100f); };
            
            StartingStamina = configFile.Bind<int>(new ConfigDefinition("Stamina", "Starting stamina"), 40, new ConfigDescription("How much stamina every player should start with.", new AcceptableValueRange<int>(0, 1000))).Sync();
            API.Stamina.AddStartStamina(StartingStamina.Value - 40);
            StartingStamina.ValueChanged += (s, e) => { API.Stamina.AddStartStamina(StartingStamina.Value - 40); };

            StaminaPerUpgrade = configFile.Bind<int>(new ConfigDefinition("Stamina", "Upgrade stamina"), 10, new ConfigDescription("How much stamina every upgrade gives.", new AcceptableValueRange<int>(0, 100))).Sync();
            API.Stamina.AddStaminaPerUpgrade(StaminaPerUpgrade.Value - 10);
            StaminaPerUpgrade.ValueChanged += (s, e) => { API.Stamina.AddStaminaPerUpgrade(StaminaPerUpgrade.Value - 10); };

            SlideStaminaCost = configFile.Bind<int>(new ConfigDefinition("Stamina", "Slide stamina cost"), 5, new ConfigDescription("How much stamina a slide cost.", new AcceptableValueRange<int>(0, 100))).Sync();
            API.Stamina.AddSlideStaminaCost(SlideStaminaCost.Value - 5);
            SlideStaminaCost.ValueChanged += (s, e) => { API.Stamina.AddSlideStaminaCost(SlideStaminaCost.Value - 5); };
        }
    }
}
