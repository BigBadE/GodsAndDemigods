using System.Collections.Generic;
using OldWorldGods.Base;
using RimWorld;
using Verse;

namespace OldWorldGods.Buildings
{
    public class Building_Rune : Building
    {
        
        private List<Rune> runes = new List<Rune>();
        private bool midRitual = false;

        public List<Rune> Runes => runes ?? (runes = new List<Rune>());

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

        public override void DeSpawn(DestroyMode mode)
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
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref runes, "runes", LookMode.Deep);
        }
    }
}
