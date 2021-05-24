// unset

using System.Collections.Generic;
using OldWorldGods.Base;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OldWorldGods.Incidents
{
    public abstract class GodChangeIncidentWorkers : IncidentWorker
    {
        protected abstract bool Chaos { get; }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Gods gods = Find.World.GetComponent<Gods>();
            bool player = false;
            bool any = false;
            foreach (God god in gods.AllGods)
            {
                if (god.IsPlayerGod)
                {
                    player = true;
                }
                else if (god.GetDef.chaos == Chaos)
                {
                    any = true;
                }
            }

            return player && any;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            WorldObject target = Find.World.worldObjects.WorldObjectAt<WorldObject>(parms.target.Tile);

            if (!God.CanCorrupt(target)) return false;
            
            List<God> gods = Find.World.GetComponent<Gods>().AllGods;
            gods.ForEach(loser => loser.LoseControl(target));
            Find.World.GetComponent<Gods>().AllGods
                .RandomElementByWeight(god => god.ControlledObjects.Count).GainControl(target);
            return true;
        }
    }

    public class IncidentWorker_SettlementCorruption : GodChangeIncidentWorkers
    {
        protected override bool Chaos => true;
    }
    
    public class IncidentWorker_SettlementCleansing : GodChangeIncidentWorkers
    {
        protected override bool Chaos => false;
    }
}
