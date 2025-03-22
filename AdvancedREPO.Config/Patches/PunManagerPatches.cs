using HarmonyLib;
using Photon.Pun;
using System.Reflection;
using System.Linq;
using System;
using AdvancedREPO.Utils;

namespace AdvancedREPO.Config.Patches
{
    public class PunManagerPatches
    {
        private static Field<PunManager, PhotonView>? PhotonViewField;

        public static void ApplyPatches()
        {
            // Getting fields
            PhotonViewField = new(typeof(PunManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(e => e.Name == "photonView").First());

            // Patch
            Plugin.Log?.LogInfo("Patching PunManager...");
            Harmony.CreateAndPatchAll(typeof(PunManagerPatches));
            Plugin.Log?.LogInfo("PunManager patched!");
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigBool")]
        public static void SyncConfigBool(PunManager __instance, string propertyName, bool value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigBoolRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigBoolRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigByte")]
        public static void SyncConfigByte(PunManager __instance, string propertyName, byte value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigByteRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigByteRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigSByte")]
        public static void SyncConfigSByte(PunManager __instance, string propertyName, sbyte value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigSByteRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigSByteRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigShort")]
        public static void SyncConfigShort(PunManager __instance, string propertyName, short value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigShortRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigShortRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigUShort")]
        public static void SyncConfigUShort(PunManager __instance, string propertyName, ushort value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigUShortRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigUShortRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigInt")]
        public static void SyncConfigInt(PunManager __instance, string propertyName, int value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigIntRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigIntRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigUInt")]
        public static void SyncConfigUInt(PunManager __instance, string propertyName, uint value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigUIntRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigUIntRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigLong")]
        public static void SyncConfigLong(PunManager __instance, string propertyName, long value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigLongRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigLongRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigULong")]
        public static void SyncConfigULong(PunManager __instance, string propertyName, ulong value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigULongRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigULongRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigFloat")]
        public static void SyncConfigFloat(PunManager __instance, string propertyName, float value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigFloatRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigFloatRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigDouble")]
        public static void SyncConfigDouble(PunManager __instance, string propertyName, double value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigDoubleRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigDoubleRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigString")]
        public static void SyncConfigString(PunManager __instance, string propertyName, string value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigStringRPC", RpcTarget.Others, propertyName, value);
            }
            SyncConfigStringRPC(__instance, propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigBoolRPC")]
        public static void SyncConfigBoolRPC(PunManager __instance, string propertyName, bool value)
        {
            Sync.SyncSet<bool>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigByteRPC")]
        public static void SyncConfigByteRPC(PunManager __instance, string propertyName, byte value)
        {
            Sync.SyncSet<byte>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigSByteRPC")]
        public static void SyncConfigSByteRPC(PunManager __instance, string propertyName, sbyte value)
        {
            Sync.SyncSet<sbyte>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigShortRPC")]
        public static void SyncConfigShortRPC(PunManager __instance, string propertyName, short value)
        {
            Sync.SyncSet<short>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigUShortRPC")]
        public static void SyncConfigUShortRPC(PunManager __instance, string propertyName, ushort value)
        {
            Sync.SyncSet<ushort>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigIntRPC")]
        public static void SyncConfigIntRPC(PunManager __instance, string propertyName, int value)
        {
            Sync.SyncSet<int>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigUIntRPC")]
        public static void SyncConfigUIntRPC(PunManager __instance, string propertyName, uint value)
        {
            Sync.SyncSet<uint>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigLongRPC")]
        public static void SyncConfigLongRPC(PunManager __instance, string propertyName, long value)
        {
            Sync.SyncSet<long>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigULongRPC")]
        public static void SyncConfigULongRPC(PunManager __instance, string propertyName, ulong value)
        {
            Sync.SyncSet<ulong>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigFloatRPC")]
        public static void SyncConfigFloatRPC(PunManager __instance, string propertyName, float value)
        {
            Sync.SyncSet<float>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigDoubleRPC")]
        public static void SyncConfigDoubleRPC(PunManager __instance, string propertyName, double value)
        {
            Sync.SyncSet<double>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigStringRPC")]
        public static void SyncConfigStringRPC(PunManager __instance, string propertyName, string value)
        {
            Sync.SyncSet<string>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncAllDictionaries")]
        [HarmonyPrefix]
        public static void InitiateSync(PunManager __instance)
        {
            if (!SemiFunc.IsMultiplayer() || PhotonNetwork.IsMasterClient)
            {
                Plugin.Log?.LogInfo("Applying configuration to runtime.");
                Sync.ApplyLocal();
            }

            if (SemiFunc.IsMultiplayer())
            {
                Plugin.Log?.LogInfo("Synchronizing configuration with clients...");
                var types = new Type[] { typeof(float), typeof(bool) };
                foreach (var config in Sync.Configs)
                {
                    if (config.Value is ConfigField<bool> @bool) SyncConfigBool(__instance, @bool.Key, @bool.Entry?.Value ?? default(bool));
                    else if (config.Value is ConfigField<byte> @byte) SyncConfigByte(__instance, @byte.Key, @byte.Entry?.Value ?? default(byte));
                    else if (config.Value is ConfigField<sbyte> @sbyte) SyncConfigSByte(__instance, @sbyte.Key, @sbyte.Entry?.Value ?? default(sbyte));
                    else if (config.Value is ConfigField<short> @short) SyncConfigShort(__instance, @short.Key, @short.Entry?.Value ?? default(short));
                    else if (config.Value is ConfigField<ushort> @ushort) SyncConfigUShort(__instance, @ushort.Key, @ushort.Entry?.Value ?? default(ushort));
                    else if (config.Value is ConfigField<int> @int) SyncConfigInt(__instance, @int.Key, @int.Entry?.Value ?? default(int));
                    else if (config.Value is ConfigField<uint> @uint) SyncConfigUInt(__instance, @uint.Key, @uint.Entry?.Value ?? default(uint));
                    else if (config.Value is ConfigField<long> @long) SyncConfigLong(__instance, @long.Key, @long.Entry?.Value ?? default(long));
                    else if (config.Value is ConfigField<ulong> @ulong) SyncConfigULong(__instance, @ulong.Key, @ulong.Entry?.Value ?? default(ulong));
                    else if (config.Value is ConfigField<float> @float) SyncConfigFloat(__instance, @float.Key, @float.Entry?.Value ?? default(float));
                    else if (config.Value is ConfigField<double> @double) SyncConfigDouble(__instance, @double.Key, @double.Entry?.Value ?? default(double));
                    else if (config.Value is ConfigField<string> @string) SyncConfigString(__instance, @string.Key, @string.Entry?.Value ?? null);
                }
            }
        }
    }
}
