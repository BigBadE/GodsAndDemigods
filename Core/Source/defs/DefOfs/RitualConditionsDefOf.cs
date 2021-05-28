using RimWorld;
using Verse;

namespace OldWorldGods.Defs
{
    [DefOf]
    public class CorruptConditionsDefOf
    {
        public static GameConditionDef ColonyCorruption;
        
        static CorruptConditionsDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(CorruptConditionsDefOf));
    }
}
