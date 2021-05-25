// unset

using RimWorld;

namespace OldWorldGods.Defs
{
    [DefOf]
    public static class CorruptFactionDefOf
    {
        public static FactionDef Corrupt;
        
        static CorruptFactionDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(FactionDefOf));
    }
}
