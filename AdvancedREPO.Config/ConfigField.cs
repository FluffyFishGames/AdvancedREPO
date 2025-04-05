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

        public virtual void AddToSync()
        {
            AdvancedREPO.Config.Sync.Configs.Add(this.Key, this);
        }

        internal virtual void SyncWithClients()
        {

        }
    }

    public class ConfigField<T> : ConfigField
    {
        public delegate void SettingChangedHandler(object sender, EventArgs e);
        public event SettingChangedHandler SettingChanged;
        public delegate void ValueChangedHandler(object sender, EventArgs e);
        public event ValueChangedHandler ValueChanged;
        private bool ReceivedSynced = false;
        private ConfigEntry<T> _Entry;
        public ConfigEntry<T> Entry
        {
            get
            {
                return _Entry;
            }
            set
            {
                if (_Entry != null)
                {
                    _Entry.SettingChanged -= SettingChangedListener;
                }
                _Entry = value;
                OldValue = _Entry.Value;
                _Entry.SettingChanged += SettingChangedListener;
            }
        }

        private void SettingChangedListener(object sender, EventArgs e)
        {
            if ((OldValue != null && !OldValue.Equals(Entry.Value)) || (OldValue == null && Entry.Value != null))
            {
                OldValue = Entry.Value;
                var oldValue = Value;
                SettingChanged?.Invoke(sender, e);
                if (!Value.Equals(oldValue))
                    ValueChanged?.Invoke(this, new EventArgs());
            }
            if (Sync && ((SyncedValue != null && !SyncedValue.Equals(Entry.Value)) || (SyncedValue == null && Entry.Value != null)))
            {
                SyncWithClients();
            }
        }

        private T OldValue;
        private T? _SyncedValue;
        internal T? SyncedValue
        {
            get
            {
                return _SyncedValue;
            }
            set
            {
                ReceivedSynced = true;
                bool changed = false;
                if ((value != null && !value.Equals(Value)) || (value == null && Value != null))
                {
                    changed = true;
                }
                _SyncedValue = value;
                if (changed)
                    ValueChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// When set saves the value in config.
        /// When get returns either the config value or the lobby synced value. 
        /// </summary>
        public T? Value
        {
            get
            {
                if (Sync && ReceivedSynced)
                    return SyncedValue;
                if (_Entry != null)
                    return _Entry.Value;
                return default(T);
            }
            set
            {
                if (_Entry != null)
                    _Entry.Value = value;
            }
        }

        internal override void SyncWithClients()
        {
            var __instance = PunManager.instance;
            if (__instance != null && (!SemiFunc.IsMultiplayer() || PhotonNetwork.IsMasterClient))
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
