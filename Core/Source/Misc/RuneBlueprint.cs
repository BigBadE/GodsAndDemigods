using System.Collections.Generic;
using System.Text;
using OldWorldGods.Base;
using OldWorldGods.Comps;
using OldWorldGods.Defs.DefOfs;
using OldWorldGods.Needs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OldWorldGods.Misc
{
    [StaticConstructorOnStartup]
    public class RuneBlueprint : Blueprint, IThingHolder
    {
        private static readonly ThingDef runeBuilding;
        
        private Thing thing;
        private CompRune runeComp;
        private Rune rune;
        private Vector3? drawPos;
        private ThingOwner resourceContainer;
        
        protected override float WorkTotal => 10;
        
        public override string Label => "UndrawnRune".Translate();

        public override Vector3 DrawPos => rune.DrawPosition(Position, 2f);

        public override Graphic Graphic => rune.Graphic;
        
        public ThingOwner GetDirectlyHeldThings() => this.resourceContainer;
        
        public void GetChildHolders(List<IThingHolder> outChildren) => 
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());


        public RuneBlueprint()
        {
        }

        static RuneBlueprint()
        {
            runeBuilding = DefDatabase<ThingDef>.GetNamed("Spell_Rune_Building");
        }
        
        public RuneBlueprint(Thing thing, Rune rune)
        {
            this.resourceContainer = new ThingOwner<Thing>(this, false);
            this.thing = thing;
            this.rune = rune;
            this.def = runeBuilding;
            runeComp = thing.TryGetComp<CompRune>();
        }

        public override void Draw()
        {
            Graphic.Draw(DrawPos, this.Rotation, this);
            Comps_PostDraw();
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length > 0)
                stringBuilder.AppendLine();
            stringBuilder.AppendLine("ContainedResources".Translate() + ":");
            ThingDefCountClass thingDefCountClass = MaterialsNeeded()[0];
            stringBuilder.Append(thingDefCountClass.thingDef.LabelCap + ": " + GetDirectlyHeldThings().Count + " / " + thingDefCountClass.count);
            return stringBuilder.ToString().Trim();
        }

        protected override Thing MakeSolidThing()
        {
            runeComp.AddRune(rune);
            Destroy();
            //Just in case any mods check
            return this;
        }

        public override bool TryReplaceWithSolidThing(
            Pawn workerPawn,
            out Thing createdThing,
            out bool jobEnded)
        {
            jobEnded = false;
            if (GenConstruct.FirstBlockingThing(this, workerPawn) != null)
            {
                workerPawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                jobEnded = true;
                createdThing = null;
                return false;
            }

            if (workerPawn.needs.TryGetNeed<Need_Cult>().CurLevel <= .001f)
            {
                workerPawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                jobEnded = true;
                createdThing = null;
                return false;
            }
            
            if (resourceContainer.Count < 1)
            {
                jobEnded = false;
                createdThing = null;
                return false;
            }

            createdThing = MakeSolidThing();
            return true;
        }

        public override List<ThingDefCountClass> MaterialsNeeded() => 
            new List<ThingDefCountClass> {new ThingDefCountClass(ThingsDefOf.Chalk, 1)};

        public override ThingDef EntityToBuildStuff() => ThingsDefOf.Chalk;

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref resourceContainer, "resourceContainer");
            Scribe_Deep.Look(ref rune, "rune");
            Scribe_References.Look(ref thing, "thing");
            if (runeComp == null)
            {
                runeComp = thing?.TryGetComp<CompRune>();
            }
        }
    }
}
