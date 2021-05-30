using System;
using System.Collections.Generic;
using OldWorldGods.Needs;
using RimWorld;
using Verse;

namespace OldWorldGods.Interactions
{
    [StaticConstructorOnStartup]
    public class InteractionWorker_CultFight : InteractionWorker
    {
        private static MentalStateDef cultFight;

        static InteractionWorker_CultFight()
        {
            cultFight = DefDatabase<MentalStateDef>.GetNamed("CultFight");
        }
        
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            float first = initiator.needs.TryGetNeed<Need_Cult>().CurLevel;
            float second = recipient.needs.TryGetNeed<Need_Cult>().CurLevel;
            return Math.Max(Math.Abs(first - second) - .25f, 0);
        }

        public override void Interacted(
            Pawn initiator,
            Pawn recipient,
            List<RulePackDef> extraSentencePacks,
            out string letterText,
            out string letterLabel,
            out LetterDef letterDef,
            out LookTargets lookTargets)
        {
            Pawn fighter = initiator.needs.TryGetNeed<Need_Cult>().CurLevel == 0 ? initiator : recipient;
            
            letterText = "CultFightText".Translate(fighter.Name.ToStringShort, 
                fighter == initiator ? recipient.Name.ToStringShort : initiator.Name.ToStringShort);
            letterLabel = "CultFight".Translate();
            letterDef = LetterDefOf.ThreatSmall;
            lookTargets = new LookTargets(initiator, recipient);
            initiator.mindState.mentalStateHandler.TryStartMentalState(cultFight, otherPawn: recipient);
            recipient.mindState.mentalStateHandler.TryStartMentalState(cultFight, otherPawn: initiator);
        }
    }
}
