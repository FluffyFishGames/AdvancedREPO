using HarmonyLib;
using Photon.Pun;
using System.Reflection;
using System.Linq;
using System;
using AdvancedREPO.Utils;
using UnityEngine;

namespace AdvancedREPO.Config.Patches
{
    public class PunManagerPatches
    {
        private static PunManager _Instance;
        public static PunManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = GameObject.FindFirstObjectByType<PunManager>();
                return _Instance;
            }
            set
            {
                if (value != null)
                    _Instance = value;
            }
        }
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
        [HarmonyPrefix]
        public static void SyncConfigBool(PunManager __instance, string propertyName, bool value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigBoolRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigByte")]
        [HarmonyPrefix]
        public static void SyncConfigByte(PunManager __instance, string propertyName, byte value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigByteRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigSByte")]
        [HarmonyPrefix]
        public static void SyncConfigSByte(PunManager __instance, string propertyName, sbyte value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigSByteRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigShort")]
        [HarmonyPrefix]
        public static void SyncConfigShort(PunManager __instance, string propertyName, short value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigShortRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigUShort")]
        [HarmonyPrefix]
        public static void SyncConfigUShort(PunManager __instance, string propertyName, ushort value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigUShortRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigInt")]
        [HarmonyPrefix]
        public static void SyncConfigInt(PunManager __instance, string propertyName, int value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigIntRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigUInt")]
        [HarmonyPrefix]
        public static void SyncConfigUInt(PunManager __instance, string propertyName, uint value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigUIntRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigLong")]
        [HarmonyPrefix]
        public static void SyncConfigLong(PunManager __instance, string propertyName, long value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigLongRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigULong")]
        [HarmonyPrefix]
        public static void SyncConfigULong(PunManager __instance, string propertyName, ulong value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigULongRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigFloat")]
        [HarmonyPrefix]
        public static void SyncConfigFloat(PunManager __instance, string propertyName, float value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigFloatRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigDouble")]
        [HarmonyPrefix]
        public static void SyncConfigDouble(PunManager __instance, string propertyName, double value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigDoubleRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigString")]
        [HarmonyPrefix]
        public static void SyncConfigString(PunManager __instance, string propertyName, string value)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
            {
                var photonView = PhotonViewField?.GetValue(__instance) ?? null;
                photonView?.RPC("SyncConfigStringRPC", RpcTarget.All, propertyName, value);
            }
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigBoolRPC")]
        [HarmonyPrefix]
        public static void SyncConfigBoolRPC(PunManager __instance, string propertyName, bool value)
        {
            Sync.SyncSet<bool>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigByteRPC")]
        [HarmonyPrefix]
        public static void SyncConfigByteRPC(PunManager __instance, string propertyName, byte value)
        {
            Sync.SyncSet<byte>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigSByteRPC")]
        [HarmonyPrefix]
        public static void SyncConfigSByteRPC(PunManager __instance, string propertyName, sbyte value)
        {
            Sync.SyncSet<sbyte>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigShortRPC")]
        [HarmonyPrefix]
        public static void SyncConfigShortRPC(PunManager __instance, string propertyName, short value)
        {
            Sync.SyncSet<short>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigUShortRPC")]
        [HarmonyPrefix]
        public static void SyncConfigUShortRPC(PunManager __instance, string propertyName, ushort value)
        {
            Sync.SyncSet<ushort>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigIntRPC")]
        [HarmonyPrefix]
        public static void SyncConfigIntRPC(PunManager __instance, string propertyName, int value)
        {
            Sync.SyncSet<int>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigUIntRPC")]
        [HarmonyPrefix]
        public static void SyncConfigUIntRPC(PunManager __instance, string propertyName, uint value)
        {
            Sync.SyncSet<uint>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigLongRPC")]
        [HarmonyPrefix]
        public static void SyncConfigLongRPC(PunManager __instance, string propertyName, long value)
        {
            Sync.SyncSet<long>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigULongRPC")]
        [HarmonyPrefix]
        public static void SyncConfigULongRPC(PunManager __instance, string propertyName, ulong value)
        {
            Sync.SyncSet<ulong>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigFloatRPC")]
        [HarmonyPrefix]
        public static void SyncConfigFloatRPC(PunManager __instance, string propertyName, float value)
        {
            Sync.SyncSet<float>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigDoubleRPC")]
        [HarmonyPrefix]
        public static void SyncConfigDoubleRPC(PunManager __instance, string propertyName, double value)
        {
            Sync.SyncSet<double>(propertyName, value);
        }

        [HarmonyPatch(typeof(PunManager), "SyncConfigStringRPC")]
        [HarmonyPrefix]
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
                Plugin.Log?.LogInfo("Applying local configuration to runtime.");
                Sync.ApplyLocal();
            }

            if (SemiFunc.IsMultiplayer() && PhotonNetwork.IsMasterClient)
            {
                Plugin.Log?.LogInfo("Synchronizing configuration with clients...");
                var types = new Type[] { typeof(float), typeof(bool) };
                foreach (var config in Sync.Configs)
                {
                    config.Value.SyncWithClients();
                }
            }
        }
    }
}
