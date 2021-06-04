using System.Collections.Generic;
using OldWorldGods.Defs.SpellEffects;
using Verse;

namespace OldWorldGods.Defs
{
    public class SpellDef : Def
    {
        [Description("Spell tier, from 1 to 4")]
        public int tier;
        
        [Description("Spell effects")]
        public List<SpellEffectDef> effects;
        
    }
}
