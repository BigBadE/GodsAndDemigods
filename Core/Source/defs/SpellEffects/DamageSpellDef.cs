using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Base;
using RimWorld;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class DamageSpellDef : PawnEffectingSpell
    {
        [Description("Should kill or down")]
        public bool kill;
        
        [Description("How many hours until death should be left (nullable)")]
        public SpellNumber hoursLeft;

        public override void Execute(Thing ritual, GodDef god, float strength, int casters, List<LocalTargetInfo> targets)
        {
            foreach (LocalTargetInfo thing in targets.Where(thing => thing.Pawn != null))
            {
                SpellManager.ApplyGodEffect(god, thing.Pawn, targets.Count, casters, strength, this);
            }
        }

    }
}
