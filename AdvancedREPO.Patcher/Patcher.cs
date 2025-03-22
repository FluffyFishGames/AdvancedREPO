using BepInEx.Logging;
using BepInEx;
using BepInEx.Preloader;
using BepInEx.Preloader.Patching;
using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;
using Mono.Cecil.Rocks;

namespace AdvancedREPO.Patcher
{ 
    /// <summary>
    /// Patcher offers attribute based pre patching of DLLs.
    /// </summary>
    public class Patcher
    {
        /// <summary>
        /// The log
        /// </summary>
        public static ManualLogSource Log;

        /// <summary>
        /// Will initialize the patcher before returning the DLLs one by one
        /// </summary>
        public static IEnumerable<string> TargetDLLs 
        { 
            get
            {
                Log = BepInEx.Logging.Logger.CreateLogSource("AdvancedREPO.Patcher");
                var assembly = Assembly.GetCallingAssembly();
                var assemblyName = assembly.GetName().Name;
                if (!assemblyName.StartsWith("BepInEx", StringComparison.OrdinalIgnoreCase))
                {
                    // clean up
                    AssemblyNames = null;
                    Assemblies = new();
                    Types = new();
                    Methods = new();

                    Log.LogInfo("Collecting patches for assembly " + assembly.GetName().Name);
                    Initialize(assembly);
                    // load all targetted dlls
                    if (AssemblyNames == null)
                        Log.LogWarning("No targetted dlls found. Skipping patching.");
                    else
                        return AssemblyNames;
                }
                return new string[0];
            }
        }
        
        /// <summary>
        /// All assembly names to be patched
        /// </summary>
        private static List<string>? AssemblyNames = null;

        /// <summary>
        /// All assemblies to be patched
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentBag<Action<AssemblyDefinition>>>? Assemblies;

        /// <summary>
        /// All types to be patched
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentBag<Action<TypeDefinition>>>>? Types;

        /// <summary>
        /// All methods to be patched
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentBag<Action<MethodDefinition>>>>>? Methods;

        private static bool Initialized = false;
        /// <summary>
        /// Initializes the logger
        /// </summary>
        public static void Initialize(Assembly assembly)
        {
            // find all patches
            if (!assembly.GetReferencedAssemblies().Any(e => e.Name == "AdvancedREPO.Patcher"))
                return;
            assembly.GetTypes().AsParallel().ForAll((type) =>
            {
                type.GetMethods().AsParallel().ForAll((method) =>
                {
                    var patchAttribute = method.GetCustomAttribute<Patch>();
                    if (patchAttribute == null) return;
                    if (patchAttribute.TypeName != null)
                    {
                        if (patchAttribute.MethodName != null)
                        {
                            try
                            {
                                Methods
                                    .GetOrAdd(patchAttribute.AssemblyName, (key) => { return new(); })
                                    .GetOrAdd(patchAttribute.TypeName, (key) => { return new(); })
                                    .GetOrAdd(patchAttribute.MethodName, (key) => { return new(); })
                                    .Add((Action<MethodDefinition>)method.CreateDelegate(typeof(Action<MethodDefinition>)));
                            }
                            catch (Exception)
                            {
                                Log.LogWarning($"Method patcher found in {method.DeclaringType.FullName}::{method.Name} has an invalid signature.");
                            }
                        }
                        else
                        {
                            try
                            {
                                Types
                                    .GetOrAdd(patchAttribute.AssemblyName, (key) => { return new(); })
                                    .GetOrAdd(patchAttribute.TypeName, (key) => { return new(); })
                                    .Add((Action<TypeDefinition>)method.CreateDelegate(typeof(Action<TypeDefinition>)));
                            }
                            catch (Exception)
                            {
                                Log.LogWarning($"Type patcher found in {method.DeclaringType.FullName}::{method.Name} has an invalid signature.");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            Assemblies
                                .GetOrAdd(patchAttribute.AssemblyName, (key) => { return new(); })
                                .Add((Action<AssemblyDefinition>)method.CreateDelegate(typeof(Action<AssemblyDefinition>)));
                        }
                        catch (Exception)
                        {
                            Log.LogWarning($"Assembly patcher found in {method.DeclaringType.FullName}::{method.Name} has an invalid signature.");
                        }
                    }
                });
            });

            // add all assemblies
            HashSet<string> assemblies = new HashSet<string>();
            Methods.Keys.ToList().ForEach((key) => { assemblies.Add(key); });
            Types.Keys.ToList().ForEach((key) => { assemblies.Add(key); });
            Assemblies.Keys.ToList().ForEach((key) => { assemblies.Add(key); });
            AssemblyNames = assemblies.ToList();
        }

        /// <summary>
        /// Patch the game assemblies
        /// </summary>
        /// <param name="assembly">The assembly to be patched</param>
        public static void Patch(AssemblyDefinition assembly)
        {
            Log.LogInfo("Patching assembly " + assembly.Name.Name);
            var dllName = assembly.Name.Name + ".dll";
            // patch assemblies
            if (Assemblies.TryGetValue(dllName, out var assemblyPatches))
            {
                Log.LogInfo("Applying patches for assembly " + assembly.Name.Name);
                assemblyPatches.AsParallel().ForAll(p => p(assembly));
            }
            // patch types
            if (Types.TryGetValue(dllName, out var typesPatches))
            {
                typesPatches.AsParallel().ForAll(t => 
                {
                    var type = assembly.MainModule?.GetType(t.Key) ?? null;
                    if (type != null)
                    {
                        Log.LogInfo("Applying patches for type " + type.FullName);
                        t.Value.AsParallel().ForAll(p => p(type));
                    }
                });
            }
            // patch methods
            if (Methods.TryGetValue(dllName, out var methodsPatches))
            {
                methodsPatches.AsParallel().ForAll(t =>
                {
                    var type = assembly.MainModule?.GetType(t.Key) ?? null;
                    if (type != null)
                    {
                        t.Value.AsParallel().ForAll(m =>
                        {
                            var methods = type.GetMethods();
                            methods.AsParallel().ForAll((method) =>
                            {
                                if (method.Name == m.Key)
                                {
                                    Log.LogInfo("Applying patches for method " + method.FullName);
                                    m.Value.AsParallel().ForAll((p) => p(method));
                                }
                            });
                        });
                    }
                });
            }
        }

        /// <summary>
        /// Finish and cleanup
        /// </summary>
        public static void Finish()
        {
            Log.Dispose();
        }
    }
}
