using Verse;

namespace OldWorldGods.Comps
{
    public class CompProperties_CultApparel : CompProperties
    {
        public float apparelValue;
        
        public CompProperties_CultApparel() => this.compClass = typeof (CompCultApparel);
    }
}
