using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Source.Defs;
using RimWorld.Planet;
using Verse;

namespace OldWorldGods.Base
{
    public class God : IExposable
    {
        private GodDef def;
        
        private List<WorldObject> controlled = new List<WorldObject>();
        
        public bool IsPlayerGod { get; set; }

        public GodDef GetDef => def;

        public List<WorldObject> ControlledObjects => controlled;
        
        public God(GodDef def)
        {
            this.def = def;
        }

        public void LoseControl(WorldObject target)
        {
            controlled.Remove(target);
        }

        public void GainControl(WorldObject target)
        {
            controlled.Add(target);
        }
        
        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Collections.Look(ref controlled, "controlled", LookMode.Reference);
        }

        public static bool CanCorrupt(WorldObject worldObject)
        {
            //If any modded worldobjects can be corrupted, add them here.
            return worldObject is Settlement || 
                   worldObject.IncidentTargetTags().Any(tag => tag.defName.Equals("Corruptible"));
        }
    }
}
