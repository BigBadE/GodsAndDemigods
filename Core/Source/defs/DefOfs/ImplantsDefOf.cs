using RimWorld;
using Verse;

namespace OldWorldGods.Defs.DefOfs
{
    [DefOf]
    public class ImplantsDefOf
    {
        public static HediffDef CorruptionProtector;
        
        static ImplantsDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ImplantsDefOf));
    }
}
