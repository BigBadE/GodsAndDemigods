using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Defs;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OldWorldGods.Base
{
    public class God : IExposable
    {
        private GodDef def;
        private string name;
        
        private List<WorldObject> controlled = new List<WorldObject>();
        
        public bool IsPlayerGod { get; set; }

        public GodDef GetDef => def;

        public List<WorldObject> ControlledObjects => controlled;

        //For save loading
        public God()
        {
        }
        
        public God(GodDef def)
        {
            this.def = def;
            this.name = (def.easterEggName != null && Find.World.info.seedString.ToLower().Equals("warhammer")) ? 
                def.easterEggName : NameGenerator.GenerateName(def.names, yes => true);
            
        }

        public void LoseControl(WorldObject target)
        {
            controlled.Remove(target);
        }

        public void GainControl(WorldObject target)
        {
            controlled.Add(target);
            target.SetFaction(def.chaos
                ? Find.World.factionManager.FirstFactionOfDef(CorruptFactionDefOf.Corrupt)
                : Find.World.factionManager.AllFactions.Where(faction => !faction.Hidden && !faction.defeated 
                        && !faction.temporary && faction.HasName)
                    .RandomElementByWeight(faction => Find.World.worldObjects
                    .AllWorldObjects.Count(settlement => settlement.Faction == faction)));
        }
        
        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref name, "name");
            Scribe_Collections.Look(ref controlled, "controlled", LookMode.Reference);
        }

        public static bool CanCorrupt(WorldObject worldObject)
        {
            //If any modded worldobjects can be corrupted, add them here.
            return (worldObject is Settlement settlement && settlement.Faction != Faction.OfPlayer
                   && settlement.Faction != Faction.Empire) || 
                   worldObject.IncidentTargetTags().Any(tag => tag.defName.Equals("Corruptible"));
        }
    }
}
