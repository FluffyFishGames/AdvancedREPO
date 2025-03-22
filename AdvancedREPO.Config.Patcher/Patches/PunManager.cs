using AdvancedREPO.Patcher;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedREPO.Config.Patches
{
    public class PunManager
    {
        [Patch("PunManager")]
        public static void AddMethods(TypeDefinition type)
        {
            Patcher.Log.LogInfo("Adding sync methods to PunManager...");
            Dictionary<string, TypeReference> types = new Dictionary<string, TypeReference>()
            {
                { "String", type.Module.TypeSystem.String },
                { "Byte", type.Module.TypeSystem.Byte },
                { "SByte", type.Module.TypeSystem.SByte },
                { "Short", type.Module.TypeSystem.Int16 },
                { "UShort", type.Module.TypeSystem.UInt16 },
                { "Int", type.Module.TypeSystem.Int32 },
                { "UInt", type.Module.TypeSystem.UInt32 },
                { "Long", type.Module.TypeSystem.Int64 },
                { "ULong", type.Module.TypeSystem.UInt64 },
                { "Float", type.Module.TypeSystem.Single },
                { "Double", type.Module.TypeSystem.Double },
                { "Bool", type.Module.TypeSystem.Boolean }
            };

            var attribute = type.Methods.Where(e => e.Name == "CrownPlayerRPC").First().CustomAttributes[0];
            foreach (var kv in types)
            {
                var method = new MethodDefinition($"SyncConfig{kv.Key}", MethodAttributes.Public, type.Module.TypeSystem.Void);
                method.Parameters.Add(new ParameterDefinition(type.Module.TypeSystem.String));
                method.Parameters.Add(new ParameterDefinition(kv.Value));

                type.Methods.Add(method);
                
                var rpcMethod = new MethodDefinition($"SyncConfig{kv.Key}RPC", MethodAttributes.Public, type.Module.TypeSystem.Void);
                rpcMethod.Parameters.Add(new ParameterDefinition(type.Module.TypeSystem.String));
                rpcMethod.Parameters.Add(new ParameterDefinition(kv.Value));
                rpcMethod.CustomAttributes.Add(new CustomAttribute(attribute.Constructor));
                type.Methods.Add(rpcMethod);
            }
        }
    }
}
