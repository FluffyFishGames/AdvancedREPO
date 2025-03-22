using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdvancedREPO.Config
{
    internal class Sync
    {
        public static Dictionary<string, ConfigField> Configs = new();

        private static ConfigField<T>? GetConfig<T>(string propertyName)
        {
            if (Configs.ContainsKey(propertyName) && Configs[propertyName] is ConfigField<T> field)
                return field;
            return null;
        }

        internal static void ApplyLocal()
        {
            foreach (var config in Configs)
            {
                if (config.Value is ConfigField<bool> @bool) ApplyLocal<bool>(@bool);
                else if (config.Value is ConfigField<byte> @byte) ApplyLocal<byte>(@byte);
                else if (config.Value is ConfigField<sbyte> @sbyte) ApplyLocal<sbyte>(@sbyte);
                else if (config.Value is ConfigField<short> @short) ApplyLocal<short>(@short);
                else if (config.Value is ConfigField<ushort> @ushort) ApplyLocal<ushort>(@ushort);
                else if (config.Value is ConfigField<int> @int) ApplyLocal<int>(@int);
                else if (config.Value is ConfigField<uint> @uint) ApplyLocal<uint>(@uint);
                else if (config.Value is ConfigField<long> @long) ApplyLocal<long>(@long);
                else if (config.Value is ConfigField<ulong> @ulong) ApplyLocal<ulong>(@ulong);
                else if (config.Value is ConfigField<float> @float) ApplyLocal<float>(@float);
                else if (config.Value is ConfigField<double> @double) ApplyLocal<double>(@double);
                else if (config.Value is ConfigField<string> @string) ApplyLocal<string>(@string);
            }
        }

        internal static void ApplyLocal<T>(ConfigField<T> field)
        {
            if (field.Entry != null)
            {
                Plugin.Log?.LogDebug($"Received configuration for {field.Key}: {field.Entry.Value}");
                field.SyncedValue = field.Entry.Value;
            }
        }

        internal static void SyncSet<T>(string propertyName, T val)
        {
            Plugin.Log?.LogDebug($"Received configuration for {propertyName}: {val}");
            var config = GetConfig<T>(propertyName);
            if (config != null)
                config.SyncedValue = val;
        }
    }
}
