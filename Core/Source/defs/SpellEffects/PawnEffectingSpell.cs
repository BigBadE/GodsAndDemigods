using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public abstract class PawnEffectingSpell : SpellEffectDef
    {
        [System.ComponentModel.Description("Target type: enemy, friendly, any")]
        public string target;
        
        public override List<LocalTargetInfo> GetTargets(Thing ritual)
        {
            switch (target.ToLower())
            {
                case "enemy":
                    return ritual.Map.mapPawns.AllPawnsSpawned
                        .Where(pawn => !pawn.IsPrisoner
                                       && pawn.Faction.PlayerRelationKind ==
                                       FactionRelationKind.Hostile).ToList().ConvertAll(pawn => (LocalTargetInfo)pawn);
                case "friendly":
                    return ritual.Map.mapPawns.FreeColonistsSpawned.ConvertAll(pawn => (LocalTargetInfo)pawn);
                case "any":
                    return ritual.Map.mapPawns.AllPawnsSpawned.ConvertAll(pawn => (LocalTargetInfo)pawn);
                default:
                    Log.Error("Unknown damage spell location: " + target);
                    return new List<LocalTargetInfo>();
            }
        }
    }
}
