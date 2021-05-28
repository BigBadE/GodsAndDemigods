// unset

using RimWorld;

namespace OldWorldGods.Defs.DefOfs
{
    [DefOf]
    public class RitualThoughtDefOf
    {
        public static ThoughtDef RitualThought;
        
        static RitualThoughtDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(RitualThoughtDefOf));
    }
}
