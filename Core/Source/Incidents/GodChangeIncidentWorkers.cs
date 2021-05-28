using System.Collections.Generic;
using System.Linq;
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
            Gods gods = Current.Game.GetComponent<Gods>();
            return PlayerAndEnemySettlement(gods, true, Chaos);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            WorldObject target = Find.World.worldObjects.AllWorldObjects.Where(God.CanCorrupt).RandomElement();

            if (target == null) return false;

            List<God> gods = Current.Game.GetComponent<Gods>().AllGods;
            gods.ForEach(loser => loser.LoseControl(target));
            Current.Game.GetComponent<Gods>().AllGods.Where(god => god.GetDef.chaos == Chaos)
                .RandomElementByWeight(god => god.ControlledObjects.Count).GainControl(target);
            return true;
        }

        private static bool PlayerAndEnemySettlement(Gods gods, bool checkChaos, bool chaos)
        {
            bool player = false;
            bool any = false;
            foreach (God god in gods.AllGods)
            {
                if (god.IsPlayerGod)
                {
                    player = true;
                }
                else if (!checkChaos || god.GetDef.chaos == chaos)
                {
                    any = true;
                }
            }

            return player && any;
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
