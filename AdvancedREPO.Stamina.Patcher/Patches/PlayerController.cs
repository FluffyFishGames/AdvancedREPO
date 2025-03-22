using Mono.Cecil;
using AdvancedREPO.Patcher;

namespace AdvancedREPO.Stamina.Patches
{
    public class PlayerController
    {
        private static FieldDefinition? IsJumpingField;

        [Patch("PlayerController")]
        public static void AddField(TypeDefinition type)
        {
            Patcher.Log.LogInfo("Adding IsJumping field to PlayerController...");
            IsJumpingField = new FieldDefinition("IsJumping", FieldAttributes.Private, type.Module.TypeSystem.Boolean);
            type.Fields.Add(IsJumpingField);
        }
    }
}
