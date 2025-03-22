using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;

namespace AdvancedREPO.Config
{
    /// <summary>
    /// Extension methods for easier use
    /// </summary>
    public static class ConfigBuilderStatic
    {
        /// <summary>
        /// Synchronizes this config entry. Use the returned ConfigField to read the value during gameplay.
        /// </summary>
        /// <typeparam name="T">The type of the config entry</typeparam>
        /// <param name="entry">The ConfigEntry</param>
        /// <returns>The synced ConfigField</returns>
        public static ConfigField<T> Sync<T>(this ConfigEntry<T> entry)
        {
            return ConfigBuilder<T>.Sync(entry, Assembly.GetCallingAssembly().GetName().Name);
        }
    }

    /// <summary>
    /// Offers a simple interface to build ConfigFields
    /// </summary>
    /// <typeparam name="T">The type of the config value</typeparam>
    public class ConfigBuilder<T>
    {
        private static Dictionary<string, ConfigFile> AssignedFiles = new(StringComparer.OrdinalIgnoreCase);
        private ConfigFile? _File;
        private bool _Sync = false;
        private string? _Description;
        private string? _Section;
        private string? _Key;
        private T[]? _Acceptable;
        private T? _Min;
        private T? _Max;
        private T? _Default;
        private ConfigEntry<T>? _Entry;
        private string _ModName;

        /// <summary>
        /// Constructs a new config builder and gathers the mod name from the calling assembly name
        /// </summary>
        public ConfigBuilder()
        {
            _ModName = Assembly.GetCallingAssembly().GetName().Name;
        }

        /// <summary>
        /// Constructs a new config builder with a given mod name
        /// </summary>
        /// <param name="modName">The name of the mod</param>
        public ConfigBuilder(string modName)
        {
            _ModName = modName;
        }

        /// <summary>
        /// Quick method to sync an already existing ConfigEntry
        /// </summary>
        /// <param name="entry">The config entry to sync</param>
        /// <returns>The config field</returns>
        public static ConfigField<T> Sync(ConfigEntry<T> entry, string modName = null)
        {
            return new ConfigBuilder<T>(modName != null ? modName : Assembly.GetCallingAssembly().GetName().Name).Entry(entry).Sync().Build();
        }

        /// <summary>
        /// Quick method to create a ConfigBuilder from an already constructed ConfigEntry
        /// </summary>
        /// <param name="entry">The ConfigEntry</param>
        /// <returns>The builder</returns>
        public static ConfigBuilder<T> FromConfigEntry(ConfigEntry<T> entry)
        {
            return new ConfigBuilder<T>(Assembly.GetCallingAssembly().GetName().Name).Entry(entry);
        }

        /// <summary>
        /// Assigns a prebuilt ConfigEntry for this ConfigField
        /// </summary>
        /// <param name="entry">The ConfigEntry</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> Entry(ConfigEntry<T> entry)
        {
            _Entry = entry;
            return this;
        }

        private static ConfigFile FindFile(string modName, string fileName)
        {
            var path = Path.Combine(BepInEx.Paths.ConfigPath, modName, fileName);
            if (!AssignedFiles.ContainsKey(path))
                AssignedFiles.Add(path, new ConfigFile(path, true));
            return AssignedFiles[path];
        }

        /// <summary>
        /// Defines the description for this config entry
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> Description(string description)
        {
            _Description = description;
            return this;
        }

        /// <summary>
        /// Defines the section for this config entry
        /// </summary>
        /// <param name="section">The section</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> Section(string section)
        {
            _Section = section;
            return this;
        }

        /// <summary>
        /// Defines the key for this config entry (required)
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> Key(string key)
        {
            _Key = key;
            return this;
        }

        /// <summary>
        /// Defines the min and max value for this config entry
        /// </summary>
        /// <param name="min">The min value</param>
        /// <param name="max">The max value</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> Range(T min, T max)
        {
            _Min = min;
            _Max = max;
            return this;
        }

        /// <summary>
        /// Defines the default value for this config entry
        /// </summary>
        /// <param name="default">The default value</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> Default(T @default)
        {
            _Default = @default;
            return this;
        }

        /// <summary>
        /// Defines acceptable values for this config entry
        /// </summary>
        /// <param name="values">The acceptable values</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> Acceptable(T[] values)
        {
            _Acceptable = values;
            return this;
        }

        /// <summary>
        /// Enables host sync for this config entry
        /// </summary>
        /// <param name="val">If this entry should be synced</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> Sync(bool val = true)
        {
            _Sync = val;
            return this;
        }

        /// <summary>
        /// Assigns a file for this config entry
        /// </summary>
        /// <param name="file">The config file</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> File(ConfigFile file)
        {
            _File = file;
            return this;
        }

        /// <summary>
        /// Defines a file name for this config entry
        /// </summary>
        /// <param name="fileName">A filename for the config file</param>
        /// <returns>The builder</returns>
        public ConfigBuilder<T> File(string fileName)
        {
            _File = FindFile(_ModName, fileName);
            return this;
        }

        /// <summary>
        /// Builds the prepared ConfigField
        /// </summary>
        /// <returns>The finished ConfigField</returns>
        public ConfigField<T> Build()
        {
            var ret = new ConfigField<T>();
            ret.Sync = _Sync;
            
            if (_Entry != null)
                ret.Entry = _Entry;
            else if (_File != null)
            {
                // we need to create the entry ourselves
                AcceptableValueBase acceptable = null;
                if (_Min != null && _Max != null && default(T) is IComparable<T>)
                    acceptable = (AcceptableValueBase)Activator.CreateInstance(typeof(AcceptableValueRange<>).MakeGenericType(typeof(T)), _Min, _Max);
                else if (_Acceptable != null && default(T) is IEquatable<T>)
                    acceptable = (AcceptableValueBase) Activator.CreateInstance(typeof(AcceptableValueList<>).MakeGenericType(typeof(T)), _Acceptable);
                ret.Entry = _File.Bind<T>(new ConfigDefinition(_Section, _Key), _Default, new ConfigDescription(_Description, acceptable));
            }
            ret.Key = _ModName;
            if (ret.Entry.Definition.Section != null)
                ret.Key += $":{ret.Entry.Definition.Section}";
            ret.Key += $":{ret.Entry.Definition.Key}";
            if (ret.Sync)
                ret.AddToSync();
            return ret;
        }
    }
}
