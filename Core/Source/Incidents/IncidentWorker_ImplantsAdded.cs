using System.Linq;
using OldWorldGods.Quests;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OldWorldGods.Incidents
{
    public class IncidentWorker_ImplantsAdded : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Caravan target = (Caravan) parms.target;
            QuestPart_GiveImplants.AddImplants(target.pawns.InnerListForReading.Where(pawn => pawn.RaceProps.body.AllParts
                    .Any(body => body.def.Equals(BodyPartDefOf.Brain))));
            Find.WorldObjects.Add(target);
            return true;
        }
    }
}
