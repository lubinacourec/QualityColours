using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace QualityColors
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "DrawThingRow")]
    public static class Patch_ITabPawnGear_DrawThingRow
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Log.Message("[QC] Transpiler for ITab_Pawn_Gear.DrawThingRow triggered.");

            var code = new List<CodeInstruction>(instructions);
            var labelMethod = AccessTools.Method(
                typeof(Widgets),
                nameof(Widgets.Label),
                new[] { typeof(UnityEngine.Rect), typeof(string) }
            );
            var colorizeMethod = AccessTools.Method(typeof(QualityColorsMod), nameof(QualityColorsMod.ColorizeQualityInText));

            for (int i = 0; i < code.Count; i++)
            {
                var instruction = code[i];

                // Look for Widgets.Label(Rect, string) call
                if (instruction.opcode == OpCodes.Call && Equals(instruction.operand, labelMethod))
                {
                    // IL stack before Widgets.Label:
                    // ... (Rect)  (string)
                    // We will transform the string before it hits Widgets.Label
                    // Insert call to ColorizeQualityInText (text, thing)
                    // 'thing' is ldarg.3
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    yield return new CodeInstruction(OpCodes.Call, colorizeMethod);
                }

                yield return instruction;
            }
        }
    }
}
