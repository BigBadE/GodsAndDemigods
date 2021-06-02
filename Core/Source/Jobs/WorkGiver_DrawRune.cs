using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Misc;
using RimWorld;
using Verse;
using Verse.AI;

namespace OldWorldGods.Jobs
{
    [StaticConstructorOnStartup]
    public class WorkGiver_DrawRune : WorkGiver_ConstructDeliverResources
    {
        private static JobDef drawRuneJob;

        static WorkGiver_DrawRune()
        {
            drawRuneJob = DefDatabase<JobDef>.GetNamed("DrawRune");
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.blueprintGrid.InnerArray
                .Where(blueprint => blueprint != null)
                .SelectMany(blueprints => blueprints?.OfType<RuneBlueprint>());
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            RuneBlueprint blue = (RuneBlueprint) t;
            if (!GenConstruct.CanConstruct(blue, pawn, false, forced))
                return null;
            Job job2 = this.ResourceDeliverJobFor(pawn, blue);
            if (job2 != null)
                return job2;
            Job job3 = this.NoCostFrameMakeJobFor(blue);
            return job3;
        }

        private Job NoCostFrameMakeJobFor(IConstructible c)
        {
            if (c.MaterialsNeeded().Count != 0) return null;
            Job job = JobMaker.MakeJob(drawRuneJob);
            job.targetA = (LocalTargetInfo)(Thing)c;
            return job;
        }
    }
}
