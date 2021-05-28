using System.Collections.Generic;
using OldWorldGods.Base;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class DamageSpellDef : SpellEffectDef
    {
        [System.ComponentModel.Description("Should kill or down")]
        public bool kill;
        
        [System.ComponentModel.Description("How many hours until death should be left (nullable)")]
        public SpellNumber hoursLeft;
        
        [System.ComponentModel.Description("Target type: Enemy, friendly, any")]
        public string target;

        public override void Execute(GodDef god, float strength, int casters, List<Thing> targets)
        {
            foreach (Thing thing in targets)
            {
                if (thing is Pawn pawn)
                {
                    SpellManager.ApplyGodEffect(god, pawn, targets.Count, casters, strength, this);
                }
            }
        }
    }
}
