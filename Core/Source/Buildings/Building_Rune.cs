using System.Collections.Generic;
using OldWorldGods.Base;
using OldWorldGods.Defs;
using OldWorldGods.Defs.SpellEffects;
using OldWorldGods.Needs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OldWorldGods.Buildings
{
    [StaticConstructorOnStartup]
    public class Building_Rune : Building
    {
        private static JobDef performRitualJob;

        private List<Pawn> casters = new List<Pawn>();
        private List<Rune> runes = new List<Rune>();
        private int ritualEnd;

        public List<Rune> Runes => runes;
        
        public List<Pawn> Casters => casters;
        
        static Building_Rune()
        {
            performRitualJob = DefDatabase<JobDef>.GetNamed("PerformRitual");
        }
        
        public void AddRune(Rune rune)
        {
            runes.RemoveAll(test => test.Position == rune.Position);
            runes.Add(rune);
        }

        public void SetRunes(List<Rune> changing)
        {
            for (int i = 0; i < 8; i++)
            {
                Rune old = runes.Find(rune => rune.Position == i);
                Rune newRune = changing.Find(rune => rune.Position == i);
                if (old == newRune || (old?.Type.Equals(newRune.Type) ?? false)) continue;

                Blueprint blueprint = new RuneBlueprint(this, newRune);
                blueprint.Position = Position;
                blueprint.Position = new IntVec3(blueprint.DrawPos);
                CellIndices cellIndices = Map.cellIndices;
                CellRect cellRect = blueprint.OccupiedRect();
                List<Blueprint>[] innerArray = Map.blueprintGrid.InnerArray;
                for (int minZ = cellRect.minZ; minZ <= cellRect.maxZ; ++minZ)
                {
                    for (int minX = cellRect.minX; minX <= cellRect.maxX; ++minX)
                    {
                        int index = cellIndices.CellToIndex(minX, minZ);
                        if(innerArray[index] == null) continue;
                        innerArray[index].RemoveAll(print => print is RuneBlueprint);
                        if (innerArray[index].Count == 0)
                            innerArray[index] = null;
                    }
                }
                blueprint.SpawnSetup(Map, false);
            }
        }

        public override void Draw()
        {
            base.Draw();
            foreach (Rune rune in runes)
            {
                rune.Graphic.Draw(rune.DrawPosition(Position, 2f), Rot4.North, this);
            }
        }

        public override void Tick()
        {
            if (ritualEnd > 0 && Current.Game.tickManager.TicksGame >= ritualEnd)
            {
                EndRitual();
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Command_Action
            {
                action = StartRitual,
                defaultLabel = "StartRitual".Translate(),
                defaultDesc = "StartRitualText".Translate(),
                icon = Texture2D.blackTexture
            };
        }

        private void StartRitual()
        {
            GetComp<CompGlower>().ReceiveCompSignal("FlickedOn");
            God playerGod = Current.Game.GetComponent<Gods>().playerGod;
            if (playerGod == null)
            {
                Messages.Message("GodlessHeathen".Translate(), MessageTypeDefOf.RejectInput);
                foreach (Pawn caster in casters)
                {
                    caster.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                return;
            }
            SpellDef spellDef = RuneManager.Verify(runes);
            if (spellDef == null)
            {
                foreach (Pawn caster in casters)
                {
                    SpellManager.ApplyGodEffect(playerGod.GetDef, caster, 1, 1, 1);
                    caster.jobs.EndCurrentJob(JobCondition.Incompletable);
                }

                Messages.Message("SpellSizzle".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            ritualEnd = Current.Game.tickManager.TicksGame + 5000;
        }

        private void EndRitual()
        {
            GetComp<CompGlower>().ReceiveCompSignal("FlickedOff");
            foreach (Pawn caster in casters)
            {
                caster.jobs.EndCurrentJob(JobCondition.Incompletable);
            }
            
            God playerGod = Current.Game.GetComponent<Gods>().playerGod;
            if (playerGod == null)
            {
                Messages.Message("GodlessHeathen".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            SpellDef spellDef = RuneManager.Verify(runes);
            if (spellDef == null)
            {
                foreach (Pawn caster in casters)
                {
                    SpellManager.ApplyGodEffect(playerGod.GetDef, caster, 1, 1, 1);
                }

                Messages.Message("SpellSizzle".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            foreach (SpellEffectDef effectDef in spellDef.effects)
            {
                SpellManager.CastSpell(effectDef, this, playerGod.GetDef, casters, effectDef.GetTargets(this));
            }
            
            Current.Game.GetComponent<Gods>().spellManager.LearnSpell(spellDef);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(
            Pawn selPawn)
        {
            if (selPawn == null)
            {
                yield break;
            }
            if (selPawn.needs.TryGetNeed<Need_Cult>().CurLevel < .001f)
            {
                yield return new FloatMenuOption("JoinRitualNotCultist".Translate(), null);
            }
            if (casters.Count == 8)
            {
                yield return new FloatMenuOption("JoinRitualFullRune".Translate(), null);
            }
            else
            {
                yield return new FloatMenuOption("JoinRitual".Translate(), () =>
                {
                    selPawn.jobs.StartJob(new Job(performRitualJob, this),
                        JobCondition.InterruptForced, resumeCurJobAfterwards: true, canReturnCurJobToPool: true);
                });
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            CellIndices cellIndices = Map.cellIndices;
            CellRect cellRect = this.OccupiedRect();
            List<Blueprint>[] innerArray = Map.blueprintGrid.InnerArray;
            for (int minZ = cellRect.minZ; minZ <= cellRect.maxZ; ++minZ)
            {
                for (int minX = cellRect.minX; minX <= cellRect.maxX; ++minX)
                {
                    int index = cellIndices.CellToIndex(minX, minZ);
                    if(innerArray[index] == null) continue;
                    innerArray[index].RemoveAll(print => print is RuneBlueprint);
                    if (innerArray[index].Count == 0)
                        innerArray[index] = null;
                }
            }
            base.DeSpawn(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref runes, "runes", LookMode.Deep);
            Scribe_Values.Look(ref ritualEnd, "ritualEnd");
        }
    }
}
