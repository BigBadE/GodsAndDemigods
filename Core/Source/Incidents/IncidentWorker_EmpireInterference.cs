using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using OldWorldGods.Base;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OldWorldGods.Incidents
{
    public class IncidentWorker_EmpireInterference : IncidentWorker
    {
        private IEnumerable<DiaOption> getChoices(AdditionalParms parms)
        {
            yield return new DiaOption("EmbraceCorruption".Translate())
            {
                action = () => Find.LetterStack.ReceiveLetter("CorruptionEmbraced".Translate(),
                        "CorruptionEmbracedText".Translate(), LetterDefOf.PositiveEvent),
                resolveTree = true
            };

            if (ModLister.RoyaltyInstalled)
            {
                yield return new DiaOption("EmpireImplants".Translate(Faction.Empire.Name))
                {
                    action = () =>
                    {
                        Slate slate = new Slate();
                        slate.Set("colonistsToLendCount", parms.pawns.Count);
                        slate.Set("map", parms.target);
                        slate.Set("asker", Faction.Empire.leader);
                        slate.Set("requiredPawns", parms.pawns);
                        QuestUtility.GenerateQuestAndMakeAvailable(
                            DefDatabase<QuestScriptDef>.GetNamed("EmpireImplants"), slate).Accept(null);
                    },
                    resolveTree = true
                };
            }
            else
            {
                yield return new DiaOption("RoyaltyRequired".Translate()) {disabled = true};
            }
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!(parms is AdditionalParms corruptParms))
            {
                return false;
            }

            DiaNode nodeRoot = ModLister.RoyaltyInstalled ? 
                new DiaNode("EmpireImplantsText".Translate(Faction.Empire.Name)) : 
                new DiaNode("CorruptWhispersInterventionText".Translate());

            nodeRoot.options.AddRange(getChoices(corruptParms));
            if (ModLister.RoyaltyInstalled)
            {
                Find.WindowStack.Add(new Dialog_NodeTreeWithFactionInfo(nodeRoot, Faction.Empire,
                    radioMode: false, title: "EmpireImplantsTitle".Translate()));
            }
            else
            {
                Find.WindowStack.Add(new Dialog_NodeTree(nodeRoot,
                    radioMode: false, title: "CorruptWhispersIntervention".Translate()));
            }

            return true;
        }
    }

    public class ImplantArrivalAction : CaravanArrivalAction
    {
        public override void Arrived(Caravan caravan)
        {
            IncidentParms parms = new IncidentParms
            {
                target = caravan,
                forced = true
            };
            
            Current.Game.storyteller.incidentQueue.Add(DefDatabase<IncidentDef>.GetNamed("ImplantsAdded"), 
                Current.Game.tickManager.TicksGame + 2500, parms);
            Find.WorldObjects.Remove(caravan);
            Find.LetterStack.ReceiveLetter("ImplantsRecieved".Translate(), 
                "ImplantsRecievedText".Translate(Faction.Empire.Name), LetterDefOf.PositiveEvent);
        }

        public override string Label => "VisitImplants".Translate();
        public override string ReportString => "VisitingImplants".Translate();
    }

    [HarmonyPatch(typeof(Settlement), "GetFloatMenuOptions")]
    class EmpireImplantPatch
    {
        public static void Postfix(ref Settlement __instance, Caravan caravan, ref IEnumerable<FloatMenuOption> __result)
        {
            if (!ModLister.RoyaltyInstalled || !__instance.Faction.Equals(Faction.Empire)) return;
            List<FloatMenuOption> output = __result.ToList();
            output.AddRange(CaravanArrivalActionUtility.GetFloatMenuOptions(
                () => true, () => new ImplantArrivalAction(), 
                "VisitImplants".Translate(), caravan, __instance.Tile, __instance));
            __result = output;
        }
    }
}
