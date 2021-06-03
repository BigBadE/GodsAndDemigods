using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class EffectSpellDef : PawnEffectingSpell
    {
        [Description("Hediff to add")]
        public HediffDef hediff;

        [Description("Target body part (nullable)")]
        public BodyPartDef bodyPart;

        public override void Execute(Thing ritual, GodDef god, float strength, int casters, List<LocalTargetInfo> targets)
        {
            foreach (LocalTargetInfo thing in targets.Where(thing => thing.Pawn != null))
            {
                BodyPartRecord record = thing.Pawn.RaceProps.body.AllParts.Find(part => part.def.Equals(bodyPart));
                thing.Pawn.health.AddHediff(hediff, record);
            }
        }
    }
}
