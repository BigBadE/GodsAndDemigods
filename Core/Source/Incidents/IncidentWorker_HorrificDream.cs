using System.Linq;
using OldWorldGods.Conditions;
using OldWorldGods.Defs;
using OldWorldGods.Needs;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace OldWorldGods.Incidents
{
    public class IncidentWorker_HorrificDream : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Find.World.gameConditionManager.GetActiveCondition<GameCondition_Corruption>() != null &&
                IncidentWorker_CorruptWhispers.CorruptableVictims(parms.target).Any(pawn => !pawn.Awake());
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Pawn target = IncidentWorker_CorruptWhispers.CorruptableVictims(parms.target)
                .Where(pawn => !pawn.Awake()).RandomElement();
            target.needs.TryGetNeed<Need_Cult>().EventTrigger(.15f);
            GrammarRequest request = new GrammarRequest();
            request.Includes.Add(RulePacksDefOf.DreamDescriptionRoot);
            request.Rules.Add(new Rule_String("ANYPAWN_nameDef", target.Name.ToStringShort));
            //TODO Symbol
            request.Rules.Add(new Rule_String("ANYSYMBOLUNFOUND", "TODO"));
            request.Rules.Add(new Rule_String("ANYBODYPART", 
                target.RaceProps.body.AllParts.RandomElement().Label));

            Find.LetterStack.ReceiveLetter("HorrificDream".Translate(),
                "HorrificDreamText".Translate(
                    target.Name.ToStringShort,
                    GrammarResolver.Resolve(null, request)), LetterDefOf.NeutralEvent);
            return true;
        }
    }
}
