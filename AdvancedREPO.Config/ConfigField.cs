using AdvancedREPO.Config.Patches;
using BepInEx.Configuration;
using JetBrains.Annotations;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdvancedREPO.Config
{
    public class ConfigField
    {
        internal bool Sync = false;
        internal string Key = null;

        public void AddToSync()
        {
            AdvancedREPO.Config.Sync.Configs.Add(this.Key, this);
        }

        internal virtual void SyncWithClients()
        {

        }
    }

    public class ConfigField<T> : ConfigField
    {
        public ConfigEntry<T> Entry;
        internal T? SyncedValue;

        /// <summary>
        /// When set saves the value in config.
        /// When get returns either the config value or the lobby synced value. 
        /// </summary>
        public T? Value
        {
            get
            {
                if (Sync)
                    return SyncedValue;
                if (Entry != null)
                    return Entry.Value;
                return default(T);
            }
            set
            {
                if (Entry != null)
                    Entry.Value = value;
            }
        }

        internal override void SyncWithClients()
        {
            var __instance = PunManagerPatches.Instance;
            if (__instance != null && !SemiFunc.IsMultiplayer() || PhotonNetwork.IsMasterClient)
            {
                if (typeof(T) == typeof(bool)) PunManagerPatches.SyncConfigBool(__instance, Key, (bool)(object)Entry.Value);
                else if (typeof(T) == typeof(byte)) PunManagerPatches.SyncConfigByte(__instance, Key, (byte)(object)Entry.Value);
                else if (typeof(T) == typeof(sbyte)) PunManagerPatches.SyncConfigSByte(__instance, Key, (sbyte)(object)Entry.Value);
                else if (typeof(T) == typeof(short)) PunManagerPatches.SyncConfigShort(__instance, Key, (short)(object)Entry.Value);
                else if (typeof(T) == typeof(ushort)) PunManagerPatches.SyncConfigUShort(__instance, Key, (ushort)(object)Entry.Value);
                else if (typeof(T) == typeof(int)) PunManagerPatches.SyncConfigInt(__instance, Key, (int)(object)Entry.Value);
                else if (typeof(T) == typeof(uint)) PunManagerPatches.SyncConfigUInt(__instance, Key, (uint)(object)Entry.Value);
                else if (typeof(T) == typeof(long)) PunManagerPatches.SyncConfigLong(__instance, Key, (long)(object)Entry.Value);
                else if (typeof(T) == typeof(ulong)) PunManagerPatches.SyncConfigULong(__instance, Key, (ulong)(object)Entry.Value);
                else if (typeof(T) == typeof(float)) PunManagerPatches.SyncConfigFloat(__instance, Key, (float)(object)Entry.Value);
                else if (typeof(T) == typeof(double)) PunManagerPatches.SyncConfigDouble(__instance, Key, (double)(object)Entry.Value);
                else if (typeof(T) == typeof(string)) PunManagerPatches.SyncConfigString(__instance, Key, (string)(object)Entry.Value);
            }
        }
    }
}
