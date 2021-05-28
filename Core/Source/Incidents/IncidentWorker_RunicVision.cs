using OldWorldGods.Conditions;
using OldWorldGods.Needs;
using RimWorld;
using Verse;

namespace OldWorldGods.Incidents
{
    public class IncidentWorker_RunicVision : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms) {
            return Find.World.gameConditionManager.GetActiveCondition<GameCondition_Corruption>() != null;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Pawn target = IncidentWorker_CorruptWhispers.CorruptableVictims(parms.target).RandomElement();
            target.needs.TryGetNeed<Need_Cult>().EventTrigger(.15f);
            
            //TODO Symbol
            Find.LetterStack.ReceiveLetter("RunicVision".Translate(), "RunicVisionText".Translate(
                target.Name.ToStringShort, "TODO"), LetterDefOf.NeutralEvent);
            return true;
        }
        
    }
}
