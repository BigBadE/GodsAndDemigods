using System.Collections.Generic;
using OldWorldGods.Source.Defs;
using RimWorld.Planet;
using Verse;

namespace OldWorldGods.Base
{
    public class Gods : WorldComponent
    {
        private List<God> gods = new List<God>();
        
        public List<God> AllGods => gods;
        
        protected Gods(World world) : base(world)
        {
            foreach (GodDef def in DefDatabase<GodDef>.AllDefsListForReading)
            {
                AllGods.Add(new God(def));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref gods, "gods", LookMode.Deep);
        }
    }
}
