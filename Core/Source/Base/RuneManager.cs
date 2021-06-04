using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OldWorldGods.Defs;
using RimWorld;
using UnityEngine;
using Verse;

namespace OldWorldGods.Base
{
    public class RuneManager
    {
        private static Dictionary<RuneDef, Graphic> runeGraphics = new Dictionary<RuneDef, Graphic>();

        public static Rune GetRuneAtPosition(SpellDef spell, int position)
        {
            long seed = Find.World.info.Seed * spell.defName.GetHashCode();
            return new Rune(position, DefDatabase<RuneDef>.AllDefsListForReading.First(runeDef => 
                runeDef.index == (int) (seed / Math.Pow(10, position)) % 10));
        }
        
        public static List<Rune> GetRunes(SpellDef spell)
        {
            List<Rune> runes = new List<Rune>();
            long seed = Find.World.info.Seed * spell.defName.GetHashCode();
            for (int i = 0; i < spell.tier * 2; i++)
            {
                runes.Add(new Rune(i, DefDatabase<RuneDef>.AllDefsListForReading.First(runeDef => 
                    runeDef.index == (int) (seed % 10))));
                seed /= 10;
            }
            return runes;
        }

        [CanBeNull]
        public static SpellDef Verify(List<Rune> runes) => (from spell in DefDatabase<SpellDef>.AllDefsListForReading 
                let seed = Find.World.info.Seed * spell.defName.GetHashCode() 
                where runes.Count == spell.tier * 2 && 
                      !Enumerable.Any(runes, rune => (int)(seed / Math.Pow(10, rune.Position)) % 10 != rune.Type.index) 
                select spell).FirstOrDefault();

        public static Graphic GetRuneGraphic(Rune rune)
        {
            Graphic graphic = runeGraphics.TryGetValue(rune.Type);
            if (graphic != null) return graphic;
            graphic = GraphicDatabase.Get(typeof(Graphic_Single), "Buildings/Runes/Spells/Rune_" + rune.Type.image,
                ShaderTypeDefOf.EdgeDetect.Shader, new Vector2(1, 1), Color.red, Color.clear, new GraphicData(),
                new List<ShaderParameter>());
            runeGraphics[rune.Type] = graphic;

            return graphic;
        }
    }
}
