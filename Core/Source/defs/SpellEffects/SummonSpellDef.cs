using System.Collections.Generic;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class SummonSpellDef : SpellEffectDef
    {
        [System.ComponentModel.Description("Summon (nullable)")]
        public ThingDef summon;

        [Description("Amount to summon")] public SpellNumber count;
        
        [System.ComponentModel.Description("Locations: enemy, ritual, friendly")]
        public string location;
        
        [System.ComponentModel.Description("Should all spawn on a single spot (enemy/friendly location only)")]
        public bool focusSpawning;

        public override void Execute(GodDef god, float strength, int casters, List<Thing> targets)
        {
            //TODO summon spells
        }
    }
}
