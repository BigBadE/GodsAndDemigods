using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class SpellNumber
    {
        [Description("Base value with one caster")]
        public float baseValue;

        [Description("Increase per caster")]
        public float casterMultiplier;
        
        [Description("Increase per target")]
        public float targetMultiplier;
        
        public float CalculateNumber(int casters, int targets)
        {
            return baseValue * (1 + casterMultiplier * casters) * (1 + targetMultiplier * targets);
        }
    }
}
