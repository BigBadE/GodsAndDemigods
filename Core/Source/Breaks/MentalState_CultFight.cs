using RimWorld;
using Verse;
using Verse.AI;

namespace OldWorldGods.Breaks
{
    public class MentalState_CultFight : MentalState
    {
        public Pawn otherPawn;

        private bool ShouldStop => !this.otherPawn.Spawned || this.otherPawn.Dead || !this.IsOtherPawnFightingWithMe;

        private bool IsOtherPawnFightingWithMe => this.otherPawn.InMentalState && this.otherPawn.MentalState is MentalState_CultFight mentalState && mentalState.otherPawn == this.pawn;

        public override void MentalStateTick()
        {
            if (this.ShouldStop)
                this.RecoverFromState();
            else
                base.MentalStateTick();
        }

        public override void PostEnd()
        {
            base.PostEnd();
            this.pawn.jobs.StopAll();
            this.pawn.mindState.meleeThreat = null;
            if (this.IsOtherPawnFightingWithMe)
                this.otherPawn.MentalState.RecoverFromState();
            if ((PawnUtility.ShouldSendNotificationAbout(this.pawn) || PawnUtility.ShouldSendNotificationAbout(this.otherPawn)) && 
                this.pawn.thingIDNumber < this.otherPawn.thingIDNumber)
                Messages.Message("CultFightStop".Translate(this.pawn.Named("PAWN1"), 
                        this.otherPawn.Named("PAWN2")), (Thing) this.pawn, MessageTypeDefOf.SituationResolved);
            if (this.pawn.Dead || this.pawn.needs.mood == null || this.otherPawn.Dead)
                return;
            this.pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.HadAngeringFight, this.otherPawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.otherPawn, "otherPawn");
        }

        public override RandomSocialMode SocialModeMax() => RandomSocialMode.Off;
    }
}
