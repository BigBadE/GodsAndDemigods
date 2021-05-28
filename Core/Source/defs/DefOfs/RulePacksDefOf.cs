using RimWorld;
using Verse;

namespace OldWorldGods.Defs
{
    [DefOf]
    public class RulePacksDefOf
    {
        public static RulePackDef DreamDescriptionRoot;
        
        static RulePacksDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(RulePacksDefOf));
    }
}
