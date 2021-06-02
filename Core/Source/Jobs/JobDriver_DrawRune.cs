using System.Collections.Generic;
using OldWorldGods.Misc;
using RimWorld;
using Verse.AI;

namespace OldWorldGods.Jobs
{
    public class JobDriver_DrawRune : JobDriver
    {
        private RuneBlueprint Blueprint => (RuneBlueprint) this.job.GetTarget(TargetIndex.A).Thing;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed) => 
            pawn.Reserve(this.job.targetA, this.job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_Goto.MoveOffTargetBlueprint(TargetIndex.A);
            yield return Toils_Construct.MakeSolidThingFromBlueprintIfNecessary(TargetIndex.A);
        }
    }
}
