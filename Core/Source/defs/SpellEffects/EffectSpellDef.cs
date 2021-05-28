using System.Collections.Generic;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class EffectSpellDef : SpellEffectDef
    {
        [System.ComponentModel.Description("Hediff to add")]
        public HediffDef hediff;

        [System.ComponentModel.Description("Target body part (nullable)")]
        public BodyPartDef bodyPart;

        [System.ComponentModel.Description("Target type: Enemy, friendly, any")]
        public string target;

        public override void Execute(GodDef god, float strength, int casters, List<Thing> targets)
        {
            foreach (Thing thing in targets)
            {
                if (!(thing is Pawn pawn)) continue;
                BodyPartRecord record = pawn.RaceProps.body.AllParts.Find(part => part.def.Equals(bodyPart));
                pawn.health.AddHediff(hediff, record);
            }
        }
    }
}
