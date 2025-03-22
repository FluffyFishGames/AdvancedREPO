
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdvancedREPO.Patcher
{
    /// <summary>
    /// Patch attribute to mark a method to be supplied with a TypeDefinition or (multiple) MethodDefinition
    /// </summary>
    public class Patch : System.Attribute
    {
        /// <summary>
        /// The type name to look for
        /// </summary>
        public string? TypeName = null;

        /// <summary>
        /// The method name to look for if any.
        /// </summary>
        public string? MethodName { get; set; } = null;

        private string _AssemblyName = "Assembly-CSharp.dll";
        /// <summary>
        /// The assembly to be patched. Standard value is Assembly-CSharp.dll
        /// </summary>
        public string AssemblyName 
        { 
            get
            {
                return _AssemblyName;
            }
            set
            {
                if (!value.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    value += ".dll";
                _AssemblyName = value;
            }
        }

        /// <summary>
        /// Indicates a method to be a patch method. 
        /// If MethodName property is supplied this method needs to accept MethodDefinition.
        /// If TypeName is provided a TypeDefinition needs to be accepted.
        /// If none is provided an AssemblyDefinition needs to be accepted.
        /// 
        /// Patches will always be applied in the order of biggest to smallest.
        /// Assembly > Type > Method
        /// </summary>
        /// <param name="typeName">The type name to look for</param>
        public Patch(string? typeName = null) 
        {
            TypeName = typeName;
        }
    }
}
