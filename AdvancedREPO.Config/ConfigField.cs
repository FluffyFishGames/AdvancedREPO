using BepInEx.Configuration;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

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
    }

    public class ConfigField<T> : ConfigField
    {
        public ConfigEntry<T>? Entry;
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
    }
}
