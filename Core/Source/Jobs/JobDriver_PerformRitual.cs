using System.Collections.Generic;
using OldWorldGods.Buildings;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OldWorldGods.Jobs
{
    public class JobDriver_PerformRitual : JobDriver
    {
        public JobDriver_PerformRitual()
        {
            globalFinishActions.Add(() => ((Building_Rune) TargetA.Thing).Casters.Remove(pawn));
        }
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return MoveToRunePosition();
            yield return WaitToComplete();
        }

        private static Toil MoveToRunePosition()
        {
            Toil toil = new Toil();
            toil.initAction = () =>
            {
                Pawn actor = toil.actor;
                IntVec3 position = actor.jobs.curJob.GetTarget(TargetIndex.A).Thing.Position;
                Building_Rune buildingRune = (Building_Rune) actor.jobs.curJob.GetTarget(TargetIndex.A).Thing;
                LocalTargetInfo info = new LocalTargetInfo(new IntVec3((int) Mathf.Sin(Mathf.PI * buildingRune.Casters.Count / 4) * 2,
                    1, (int) -Mathf.Cos(Mathf.PI * buildingRune.Casters.Count / 4) * 2) + position);
                actor.pather.StartPath(info, PathEndMode.Touch);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            return toil;
        }
        
        private static Toil WaitToComplete()
        {
            Toil toil = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Never
            };
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            return toil;
        }
    }
}
