using Mono.Cecil;
using System.Collections.Generic;

namespace AdvancedREPO.Stamina
{ 
    public class Patcher : AdvancedREPO.Patcher.Patcher
    {
        public new static IEnumerable<string> TargetDLLs
        {
            get
            {
                return AdvancedREPO.Patcher.Patcher.TargetDLLs;
            }
        }

        public new static void Patch(AssemblyDefinition assembly)
        {
            AdvancedREPO.Patcher.Patcher.Patch(assembly);
        }
    }
}
