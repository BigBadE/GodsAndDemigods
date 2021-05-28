using System.Collections.Generic;
using OldWorldGods.Needs;
using RimWorld;
using Verse;

namespace OldWorldGods.Interactions
{
    public class InteractionWorker_CultInteraction : InteractionWorker
    {
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            return (initiator.needs.TryGetNeed<Need_Cult>() != null) && (recipient.needs.TryGetNeed<Need_Cult>() != null)
                ? 10 : 0;
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
            base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef,
                out lookTargets);
            initiator.needs.TryGetNeed<Need_Cult>().SocialInteraction(true);
            recipient.needs.TryGetNeed<Need_Cult>().SocialInteraction(true);
        }
    }
}
