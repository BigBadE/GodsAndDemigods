using RimWorld;
using Verse;

namespace OldWorldGods.Defs.DefOfs
{
    [DefOf]
    public class ThingsDefOf
    {
        public static ThingDef Chalk;
        public static TerrainDef Spell_Rune;
        
        static ThingsDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThingsDefOf));
    }
}
