using System;
using GodsAndDemigods.Def;
using HarmonyLib;
using RimWorld;
using Verse;

namespace GodsAndDemigods.Base
{
    public class CustomDeity : IdeoFoundation_Deity.Deity
    {
        public bool major;
        public DeityDef def;
    }

    [HarmonyPatch(typeof(IdeoFoundation_Deity.Deity), "ExposeData")]
    public class DeityExposableSaver
    {
        public void Postfix(ref IdeoFoundation_Deity.Deity __instance)
        {
            if (!(__instance is CustomDeity deity))
            {
                throw new InvalidCastException("Tried saving vanilla deity instead of overwritten one!");
            }
            
            Scribe_Values.Look(ref deity.major, "major");
            Scribe_Defs.Look(ref deity.def, "def");
        }
    }
}
