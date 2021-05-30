using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldWorldGods.Base;
using OldWorldGods.Conditions;
using OldWorldGods.Needs;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace OldWorldGods.Incidents
{
    public class IncidentWorker_CorruptWhispers : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            List<Pawn> pawns = ActualVictims(parms).Where(pawn => pawn.RaceProps.body.AllParts
                .Any(body => body.def.Equals(BodyPartDefOf.Brain))).ToList();
            if (!pawns.Any())
                return false;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn pawn in pawns)
            {
                pawn.needs.TryGetNeed<Need_Cult>().EventTrigger(.5f);
                if (stringBuilder.Length != 0)
                    stringBuilder.AppendLine();
                stringBuilder.Append("  - " + pawn.LabelNoCountColored.Resolve());
            }
            
            string str = "CorruptWhispers".Translate(pawns.Count.ToString(), Faction.OfPlayer.def.pawnsPlural,
                    Current.Game.GetComponent<Gods>().PlayerGod?.GetDef.words ?? "DefaultWords".Translate(), 
                    stringBuilder.ToString());

            this.SendStandardLetter("CorruptWhispersTitle".Translate(), str, 
                LetterDefOf.NegativeEvent, parms, pawns, Array.Empty<NamedArgument>());

            AdditionalParms newParms = new AdditionalParms
            {
                pawns = pawns, target = parms.target
            };
            
            Current.Game.storyteller.incidentQueue.Add(DefDatabase<IncidentDef>.GetNamed("EmpireInterference"), 
                Current.Game.tickManager.TicksGame + 15000, newParms);

            GameCondition gameCondition = new GameCondition_Corruption();
            gameCondition.startTick = Find.TickManager.TicksGame;
            gameCondition.def = DefDatabase<GameConditionDef>.GetNamed("ColonyCorruption");
            gameCondition.Duration = -1;
            gameCondition.uniqueID = Find.UniqueIDsManager.GetNextGameConditionID();
            gameCondition.PostMake();
            Find.World.gameConditionManager.RegisterCondition(gameCondition);
            
            return true;
        }
        
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Find.World.gameConditionManager.GetActiveCondition<GameCondition_Corruption>() == null &&
                   CorruptableVictims(parms.target).Any(pawn => pawn.RaceProps.body.AllParts
                       .Any(body => body.def.Equals(BodyPartDefOf.Brain)));
        }

        private static IEnumerable<Pawn> PossibleVictims(
            IIncidentTarget target)
        {
            return target is Map map
                ? map.mapPawns.FreeColonistsSpawned
                : ((Caravan)target).PawnsListForReading.Where(x => x.IsFreeColonist);
        }

        public static IEnumerable<Pawn> CorruptableVictims(IIncidentTarget target) => PossibleVictims(target)
            .Where(p => !(p.ParentHolder is Building_CryptosleepCasket) && GameCondition_Corruption.IsValidPawn(p));

        private static IEnumerable<Pawn> ActualVictims(IncidentParms parms)
        {
            int num = PossibleVictims(parms.target).Count();
            int count = Mathf.Clamp(new IntRange(Mathf.RoundToInt(num * 0.2f), 
                Mathf.RoundToInt(num * 0.4f)).RandomInRange, 1, 3);
            return CorruptableVictims(parms.target).InRandomOrder().Take(count);
        }
    }
}
