using System.Collections.Generic;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public abstract class SpellEffectDef
    {
        [Description("Total targets")]
        public SpellNumber targets;
        
        [Description("Strength")]
        public SpellNumber strength;

        public abstract void Execute(Thing ritual, GodDef god, float strength, int casters, List<LocalTargetInfo> targets);

        public abstract List<LocalTargetInfo> GetTargets(Thing ritual);
    }
}
