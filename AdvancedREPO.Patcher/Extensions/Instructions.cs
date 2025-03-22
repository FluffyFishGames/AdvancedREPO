using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace AdvancedREPO.Patcher.Extensions
{
    /// <summary>
    /// Offers extensions for instruction sets
    /// </summary>
    public static class Instructions
    {
        /// <summary>
        /// Dictionary to simplify OpCodes
        /// </summary>
        private static Dictionary<OpCode, OpCode> SimplifyDict = new Dictionary<OpCode, OpCode>() {
            { OpCodes.Beq_S, OpCodes.Beq },
            { OpCodes.Bge_S, OpCodes.Bge },
            { OpCodes.Bge_Un_S, OpCodes.Bge_Un },
            { OpCodes.Bgt_S, OpCodes.Bgt },
            { OpCodes.Bgt_Un_S, OpCodes.Bgt_Un },
            { OpCodes.Ble_S, OpCodes.Ble },
            { OpCodes.Ble_Un_S, OpCodes.Ble_Un },
            { OpCodes.Blt_S, OpCodes.Blt },
            { OpCodes.Blt_Un_S, OpCodes.Blt_Un },
            { OpCodes.Bne_Un_S, OpCodes.Bne_Un },
            { OpCodes.Brfalse_S, OpCodes.Brfalse },
            { OpCodes.Brtrue_S, OpCodes.Brtrue },
            { OpCodes.Br_S, OpCodes.Br },
            { OpCodes.Ldarg_S, OpCodes.Ldarg },
            { OpCodes.Ldarga_S, OpCodes.Ldarga },
            { OpCodes.Ldloc_S, OpCodes.Ldloc },
            { OpCodes.Ldloca_S, OpCodes.Ldloca },
            { OpCodes.Leave_S, OpCodes.Leave },
            { OpCodes.Starg_S, OpCodes.Starg },
            { OpCodes.Stloc_S, OpCodes.Stloc }
        };

        /// <summary>
        /// Dictionary to ultra simplify opcodes
        /// </summary>
        private static Dictionary<OpCode, OpCode> UltraSimplifyDict = new Dictionary<OpCode, OpCode>()
        {
            { OpCodes.Beq_S, OpCodes.Beq },
            { OpCodes.Bge_S, OpCodes.Bge },
            { OpCodes.Bge_Un_S, OpCodes.Bge_Un },
            { OpCodes.Bgt_S, OpCodes.Bgt },
            { OpCodes.Bgt_Un_S, OpCodes.Bgt_Un },
            { OpCodes.Ble_S, OpCodes.Ble },
            { OpCodes.Ble_Un_S, OpCodes.Ble_Un },
            { OpCodes.Blt_S, OpCodes.Blt },
            { OpCodes.Blt_Un_S, OpCodes.Blt_Un },
            { OpCodes.Bne_Un_S, OpCodes.Bne_Un },
            { OpCodes.Brfalse_S, OpCodes.Brfalse },
            { OpCodes.Brtrue_S, OpCodes.Brtrue },
            { OpCodes.Br_S, OpCodes.Br },
            { OpCodes.Ldarga_S, OpCodes.Ldarga },
            { OpCodes.Ldarg_0, OpCodes.Ldarg },
            { OpCodes.Ldarg_1, OpCodes.Ldarg },
            { OpCodes.Ldarg_2, OpCodes.Ldarg },
            { OpCodes.Ldarg_3, OpCodes.Ldarg },
            { OpCodes.Ldloca_S, OpCodes.Ldloca },
            { OpCodes.Ldloc_S, OpCodes.Ldloc },
            { OpCodes.Ldloc_0, OpCodes.Ldloc },
            { OpCodes.Ldloc_1, OpCodes.Ldloc },
            { OpCodes.Ldloc_2, OpCodes.Ldloc },
            { OpCodes.Ldloc_3, OpCodes.Ldloc },
            { OpCodes.Leave_S, OpCodes.Leave },
            { OpCodes.Starg_S, OpCodes.Starg },
            { OpCodes.Stloc_S, OpCodes.Stloc },
            { OpCodes.Stloc_0, OpCodes.Stloc },
            { OpCodes.Stloc_1, OpCodes.Stloc },
            { OpCodes.Stloc_2, OpCodes.Stloc },
            { OpCodes.Stloc_3, OpCodes.Stloc },
            { OpCodes.Ldc_I4_0, OpCodes.Ldc_I4 },
            { OpCodes.Ldc_I4_1, OpCodes.Ldc_I4 },
            { OpCodes.Ldc_I4_2, OpCodes.Ldc_I4 },
            { OpCodes.Ldc_I4_3, OpCodes.Ldc_I4 },
            { OpCodes.Ldc_I4_4, OpCodes.Ldc_I4 },
            { OpCodes.Ldc_I4_5, OpCodes.Ldc_I4 },
            { OpCodes.Ldc_I4_6, OpCodes.Ldc_I4 },
            { OpCodes.Ldc_I4_7, OpCodes.Ldc_I4 },
            { OpCodes.Ldc_I4_8, OpCodes.Ldc_I4 }
        };

        /// <summary>
        /// Simplifies an op code to its elemental op code.
        /// </summary>
        /// <param name="op">The op code to simplify</param>
        /// <param name="ultra">If it should be ultra simplified</param>
        /// <returns>The simplified op code</returns>
        private static OpCode Simplify(OpCode op, bool ultra = false)
        {
            if (ultra && UltraSimplifyDict.ContainsKey(op))
                return UltraSimplifyDict[op];
            else if (SimplifyDict.ContainsKey(op))
                return SimplifyDict[op];
            return op;
        }

        /// <summary>
        /// Loosely compares two op codes
        /// </summary>
        /// <param name="op1">The op code to compare</param>
        /// <param name="op2">The op code to compare with</param>
        /// <param name="ultra">If the check should be done ultra loosely</param>
        /// <returns>If the op codes match</returns>
        public static bool LooseEquals(this OpCode op1, OpCode op2, bool ultra = false)
        {
            return Simplify(op1, ultra) == Simplify(op2, ultra);
        }

        /// <summary>
        /// Searches for an occurrence of a specific op code pattern.
        /// </summary>
        /// <param name="collection">The instructions to search in</param>
        /// <param name="opCodes">The op codes to look for</param>
        /// <param name="startIndex">The index to start looking from</param>
        /// <param name="ultraSimplify">If the comparison should be ultra loose</param>
        /// <returns>The position of the first occurrence if found otherwise -1</returns>
        public static int SearchFor(this Mono.Collections.Generic.Collection<Instruction> collection, OpCode[] opCodes, int startIndex = 0, bool ultraSimplify = false)
        {
            var c = 0;
            for (var i = startIndex; i < collection.Count - opCodes.Length; i++)
            { 
                var inst = collection[i];
                if (inst.OpCode.LooseEquals(opCodes[c]))
                {
                    c++;
                    if (c == opCodes.Length)
                        return i - opCodes.Length + 1;
                }
                else
                    c = 0;
            }
            return -1;
        }
    }
}
