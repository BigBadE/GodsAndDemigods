using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OldWorldGods.Defs;
using Verse;

namespace OldWorldGods.Base
{
    public class RuneManager
    {
        public static Rune GetRuneAtPosition(SpellDef spell, RunePosition position)
        {
            int seed = Find.World.info.Seed * spell.defName.GetHashCode();
            return new Rune((int) position, (int) (seed / Math.Pow(10, (int) position)) % 10);
        }

        [CanBeNull]
        public static SpellDef Verify(List<Rune> runes) => (from spell in DefDatabase<SpellDef>.AllDefsListForReading 
                let seed = Find.World.info.Seed * spell.defName.GetHashCode() 
                where runes.Count == spell.tier * 2 && 
                      !Enumerable.Any(runes, rune => (int)(seed / Math.Pow(10, rune.Position)) % 10 != rune.Type) 
                select spell).FirstOrDefault();
    }

    public enum RunePosition
    {
        NORTH,
        NORTHEAST,
        EAST,
        SOUTHEAST,
        SOUTH,
        SOUTHWEST,
        WEST,
        NORTHWEST
    }
}
