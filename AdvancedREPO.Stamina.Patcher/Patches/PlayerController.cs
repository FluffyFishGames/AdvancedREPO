using Mono.Cecil;
using AdvancedREPO.Patcher;

namespace AdvancedREPO.Stamina.Patches
{
    public class PlayerController
    {
        private static FieldDefinition? SprintJumpField;

        [Patch("PlayerController")]
        public static void AddField(TypeDefinition type)
        {
            Patcher.Log.LogInfo("Adding SprintJump field to PlayerController...");
            SprintJumpField = new FieldDefinition("SprintJump", FieldAttributes.Private, type.Module.TypeSystem.Boolean);
            type.Fields.Add(SprintJumpField);
        }
    }
}
