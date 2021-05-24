using RimWorld;
using Verse;

//using System.Reflection;
//using HarmonyLib;

namespace OldWorldGods
{
    [DefOf]
    public class TemplateDefOf
    {
        public static LetterDef success_letter;
    }

    public class MyMapComponent : MapComponent
    {
        public MyMapComponent(Map map) : base(map){}
        public override void FinalizeInit()
        {
            Find.World.ConstructComponents();
            Messages.Message("Success", null, MessageTypeDefOf.PositiveEvent);
            Find.LetterStack.ReceiveLetter("Success", TemplateDefOf.success_letter.description, TemplateDefOf.success_letter, null);
        }
    }

    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start()
        {
            Log.Message("Mod template loaded successfully!");
        }
    }
}
