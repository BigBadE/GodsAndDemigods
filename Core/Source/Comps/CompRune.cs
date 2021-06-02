using System.Collections.Generic;
using OldWorldGods.Base;
using OldWorldGods.Misc;
using RimWorld;
using Verse;

namespace OldWorldGods.Comps
{
    public class CompRune : ThingComp
    {
        private List<Rune> runes = new List<Rune>();

        public List<Rune> Runes => runes ?? (runes = new List<Rune>());

        //Needed for initialization
        public CompRune()
        {
        }

        public void QueueRune(Rune rune)
        {
            Find.Maps[0].blueprintGrid.Register(new RuneBlueprint(parent, rune));
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

                Blueprint blueprint = new RuneBlueprint(parent, newRune);
                blueprint.Position = parent.Position;
                blueprint.Position = new IntVec3(blueprint.DrawPos);
                CellIndices cellIndices = parent.Map.cellIndices;
                CellRect cellRect = blueprint.OccupiedRect();
                List<Blueprint>[] innerArray = parent.Map.blueprintGrid.InnerArray;
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
                blueprint.SpawnSetup(parent.Map, false);
            }
        }

        public override void PostDeSpawn(Map map)
        {
            CellIndices cellIndices = map.cellIndices;
            CellRect cellRect = parent.OccupiedRect();
            List<Blueprint>[] innerArray = map.blueprintGrid.InnerArray;
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
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref runes, "runes", LookMode.Deep);
        }
    }
}
