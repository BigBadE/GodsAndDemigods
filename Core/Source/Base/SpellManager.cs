using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Defs;
using OldWorldGods.Defs.DefOfs;
using OldWorldGods.Defs.SpellEffects;
using OldWorldGods.Thoughts;
using RimWorld;
using Verse;

namespace OldWorldGods.Base
{
    public class SpellManager
    {
        public static void CastSpell(SpellEffectDef effectDef, Thing rune, GodDef god, List<Pawn> casters, List<LocalTargetInfo> targets)
        {
            if (targets.Count == 0)
            {
                Messages.Message("SpellNoTargets".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }
            int totalTargets =
                (int)(effectDef.targets.baseValue * (1 + effectDef.targets.casterMultiplier * casters.Count));
            if (targets.Count > totalTargets) targets.RemoveRange(totalTargets, targets.Count);
            float strength = effectDef.strength.CalculateNumber(casters.Count, targets.Count);
            ApplyRecoil(casters, god, strength);
            effectDef.Execute(rune, god, strength, casters.Count, targets);
        }


        private static void ApplyRecoil(List<Pawn> casters, GodDef god, float strength)
        {
            foreach (Pawn caster in casters)
            {
                ApplyGodEffect(god, caster, casters.Count, 1, strength);
            }
        }

        public static void ApplyGodEffect(GodDef god, Pawn pawn, int targets, int casters, float strength,
            DamageSpellDef damageSpell = null)
        {
            switch (god.godEffect)
            {
                case "blood":
                {
                    if (damageSpell != null)
                    {
                        Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Cut, pawn);
                        hediff.CurStage.deathMtbDays =
                            damageSpell.hoursLeft.CalculateNumber(casters, targets);
                        if (!damageSpell.kill) hediff.Severity = 0;
                        pawn.health.AddHediff(hediff, pawn.RaceProps.body.corePart,
                            new DamageInfo(DamageDefOf.Cut, strength, 9999,
                                hitPart: pawn.RaceProps.body.corePart));
                    }
                    else
                    {
                        pawn.health.AddHediff(HediffDefOf.Cut, pawn.RaceProps.body.corePart,
                            new DamageInfo(DamageDefOf.Cut, strength, 9999,
                                hitPart: pawn.RaceProps.body.corePart));
                    }

                    break;
                }
                case "poison":
                {
                    if (damageSpell != null)
                    {
                        Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.ToxicBuildup, pawn);
                        hediff.CurStage.deathMtbDays =
                            damageSpell.hoursLeft.CalculateNumber(casters, targets);
                        if (!damageSpell.kill) hediff.Severity = 0;
                        pawn.health.AddHediff(hediff, pawn.RaceProps.body.corePart,
                            new DamageInfo(DamageDefOf.Rotting, strength, 9999,
                                hitPart: pawn.RaceProps.body.corePart));
                    }
                    else
                    {
                        pawn.health.AddHediff(HediffDefOf.ToxicBuildup, pawn.RaceProps.body.corePart,
                            new DamageInfo(DamageDefOf.Rotting, strength, 9999,
                                hitPart: pawn.RaceProps.body.corePart));
                    }

                    break;
                }
                case "corruption":
                {
                    if (pawn.Faction.Equals(Faction.OfPlayer))
                    {
                        foreach (Pawn other in pawn.Map.mapPawns.FreeColonists.Where(other => other != pawn &&
                            pawn.relations.OpinionOf(other) > 0 && strength-- > 0))
                        {
                            other.needs.mood.thoughts.memories.TryGainMemory(RitualThoughtDefOf.RitualThought, pawn);
                        }
                    }

                    List<Thought> thoughts = new List<Thought>();
                    pawn.needs.mood.thoughts.GetAllMoodThoughts(thoughts);
                    RitualThought target = (RitualThought)thoughts.Find(thought => thought is RitualThought);
                    if (target == null)
                    {
                        target = new RitualThought();
                        pawn.needs.mood.thoughts.situational.AppendMoodThoughts(new List<Thought> {target});
                    }

                    while (strength < 1)
                    {
                        target.SetStageIndex++;
                        strength /= 5;
                    }

                    if (target.CurStageIndex > target.def.stages.Count)
                    {
                        pawn.SetFaction(Find.FactionManager.FirstFactionOfDef(CorruptFactionDefOf.Corrupt));
                    }

                    break;
                }
                case "time":
                {
                    pawn.ageTracker.AgeTickMothballed((int)strength * 300000);
                    break;
                }
            }
        }

        public static void ApplyItemEffect(ItemEffect itemEffect, float strength, Apparel apparel)
        {
            if (itemEffect.untaint)
            {
                apparel.Notify_PawnResurrected();
            }

            if (itemEffect.itemHealthChange)
            {
                apparel.HitPoints -= (int)strength;
            }
        }
    }
}
