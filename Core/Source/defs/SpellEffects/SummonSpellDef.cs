using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class SummonSpellDef : SpellEffectDef
    {
        [Description("Summon (nullable)")]
        public ThingDef summon;

        [Description("Amount to summon, if not focus it's per target")]
        public SpellNumber count;

        [Description("Locations: enemy, ritual, friendly")]
        public string location;

        [Description("Should all spawn on a single spot (enemy/friendly location only)")]
        public bool focusSpawning;

        public override void Execute(Thing ritual, GodDef god, float strength, int casters,
            List<LocalTargetInfo> targets)
        {
            if (focusSpawning)
            {
                Spawn(ritual, targets.First(), count.CalculateNumber(casters, 1));
                return;
            }

            List<LocalTargetInfo> targetInfos = targets.ToList();
            float totalCount = count.CalculateNumber(casters, targetInfos.Count);
            foreach (LocalTargetInfo thing in targetInfos)
            {
                Spawn(ritual, thing, totalCount);
            }
        }

        private void Spawn(Thing ritual, LocalTargetInfo target, float count)
        {
            for (int i = 0; i < count; i++)
            {
                ritual.Map.thingGrid.Register(new Thing {def = summon, Position = target.Cell});
            }
        }

        public override List<LocalTargetInfo> GetTargets(Thing ritual)
        {
            switch (location.ToLower())
            {
                case "enemy":
                    return ritual.Map.mapPawns.AllPawnsSpawned
                        .Where(pawn => !pawn.IsPrisoner
                                       && pawn.Faction.PlayerRelationKind ==
                                       FactionRelationKind.Hostile).ToList().ConvertAll(pawn => (LocalTargetInfo)pawn);
                case "ritual":
                    return new List<LocalTargetInfo> {(LocalTargetInfo)ritual};
                case "friendly":
                    return ritual.Map.mapPawns.FreeColonistsSpawned.ConvertAll(pawn => (LocalTargetInfo)pawn);
                default:
                    Log.Error("Unknown summon spell location: " + location);
                    return new List<LocalTargetInfo>();
            }
        }
    }
}
