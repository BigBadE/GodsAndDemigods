using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GodsAndDemigods.Base;
using GodsAndDemigods.Def;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GodsAndDemigods.UI
{
    [HarmonyPatch(typeof(Page_ConfigureIdeo))]
    public class IdeologyMenu
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            IEnumerator<CodeInstruction> enumerator = instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CodeInstruction found = enumerator.Current;
                if (found.opcode == OpCodes.Newobj &&
                    ((ConstructorInfo) found.operand).DeclaringType == typeof(Dialog_EditDeity)) break;
                yield return found;
            }

            //Call DrawTitleSelector
            yield return new CodeInstruction(OpCodes.Newobj, typeof(Dialog_EditDeityWithTypes)
                .GetConstructors()[0]);
            yield return enumerator.Current;
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }

            enumerator.Dispose();
        }
    }

    [HarmonyPatch(typeof(Dialog_EditDeity), "DoWindowContects")]
    public class Dialog_EditDeityWithTypes : Dialog_EditDeity
    {
        public static MethodInfo GetXMethod =
            typeof(Rect).GetProperty("x", BindingFlags.Instance | BindingFlags.Public).GetMethod;

        private CustomDeity _deity;
        private DeityDef _newDeityDef;

        public Dialog_EditDeityWithTypes(IdeoFoundation_Deity.Deity deity, Ideo ideo) : base(deity, ideo)
        {
            _deity = (CustomDeity)deity;
            _newDeityDef = _deity.def;
        }

        public override void OnAcceptKeyPressed()
        {
            _deity.def = _newDeityDef;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            IEnumerator<CodeInstruction> enumerator = instructions.GetEnumerator();

            int rects = 0;
            while (enumerator.MoveNext())
            {
                CodeInstruction found = enumerator.Current;
                if (found.Calls(GetXMethod) &&
                    rects++ == 3) break;
                yield return found;
            }

            //Call DrawTitleSelector
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldflda, "newDeityTitle");
            yield return new CodeInstruction(OpCodes.Ldfld, "ideo");
            yield return new CodeInstruction(OpCodes.Call, typeof(Dialog_EditDeityWithTypes)
                .GetMethod(nameof(DrawTitleSelector)));

            yield return enumerator.Current;
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }

            enumerator.Dispose();
        }

        //Line 52-54
        public static void DrawTitleSelector(Rect rect, Dialog_EditDeity instance, ref string newDeityTitle, Ideo ideo)
        {
            float x = rect.x + rect.width / 3f;
            float width = rect.xMax - x;
            float y2 = rect.y + 35 + 10 + 30 + 10;
            Widgets.Label(new Rect(rect.x, y2, width, 30), "DeityTitle".Translate());
            newDeityTitle = Widgets.TextField(new Rect(x, y2, width / 2 - 15, 30), newDeityTitle);

            ref DeityDef deityDef = ref ((Dialog_EditDeityWithTypes)instance)._deity.def;
            DeityDef found = deityDef;
            if (Widgets.ButtonText(new Rect(x + (rect.width / 3f), y2,
                width / 2 - 15, 30), deityDef.name))
            {
                Find.WindowStack.Add(new FloatMenu(DefDatabase<DeityDef>.AllDefsListForReading
                    .Where(def => def.possibleMemes.Any(meme => ideo.memes.Contains(meme)))
                    .Select(def => new FloatMenuOption(def.name, () => found = def)).ToList()));
            }

            deityDef = found;
        }
    }
}
