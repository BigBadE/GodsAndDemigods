using System.Collections.Generic;
using OldWorldGods.Conditions;
using OldWorldGods.Defs.DefOfs;
using OldWorldGods.Needs;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OldWorldGods.Quests
{
    public class QuestNode_GiveImplants : QuestNode
    {
        [NoTranslate] public SlateRef<string> inSignal;
        public SlateRef<Thing> shuttle;

        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            if (this.shuttle.GetValue(slate) == null)
                return;
            QuestGen.quest.AddPart(new QuestPart_GiveImplants
            {
                inSignal =
                    (QuestGenUtility.HardcodedSignalWithQuestID(this.inSignal.GetValue(slate)) ??
                     QuestGen.slate.Get<string>("inSignal")),
                shuttle = this.shuttle.GetValue(slate),
            });
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }

    public class QuestPart_GiveImplants : QuestPart
    {
        public string inSignal;
        public Thing shuttle;

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            if (signal.tag != this.inSignal || this.shuttle == null)
                return;

            AddImplants(shuttle.TryGetComp<CompShuttle>().requiredPawns);
            
            Find.LetterStack.ReceiveLetter("ImplantsRecieved".Translate(),
                "ImplantsRecievedText".Translate(Faction.Empire.Name), LetterDefOf.PositiveEvent);
            Find.World.gameConditionManager.GetActiveCondition<GameCondition_Corruption>()?.CheckPawns();
            Faction.Empire.allowRoyalFavorRewards = true;
        }

        public static void AddImplants(IEnumerable<Pawn> pawns)
        {
            foreach (Pawn pawn in pawns)
            {
                pawn.health.AddHediff(ImplantsDefOf.CorruptionProtector, pawn.RaceProps.body.AllParts.Find(
                    part => part.def.Equals(BodyPartDefOf.Brain)));
                pawn.needs.TryGetNeed<Need_Cult>()?.Notify_ImplantInstalled();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.inSignal, "inSignal");
            Scribe_References.Look(ref this.shuttle, "shuttle");
        }

        public override void AssignDebugData()
        {
            base.AssignDebugData();
            this.inSignal = "DebugSignal" + Rand.Int;
            if (Find.AnyPlayerHomeMap == null)
                return;
            Map randomPlayerHomeMap = Find.RandomPlayerHomeMap;
            IntVec3 center = DropCellFinder.RandomDropSpot(randomPlayerHomeMap);
            this.shuttle = ThingMaker.MakeThing(ThingDefOf.Shuttle);
            GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(ThingDefOf.ShuttleIncoming, this.shuttle), center,
                randomPlayerHomeMap, ThingPlaceMode.Near);
        }

        public override void ReplacePawnReferences(Pawn replace, Pawn with)
        {
            shuttle?.TryGetComp<CompShuttle>().requiredPawns.Replace(replace, with);
        }
    }
}
