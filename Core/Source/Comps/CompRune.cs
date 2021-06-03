using System.Collections.Generic;
using OldWorldGods.Base;
using OldWorldGods.Misc;
using RimWorld;
using Verse;

namespace OldWorldGods.Comps
{
    public class CompRune : CompGlower
    {
        private List<Rune> runes = new List<Rune>();
        private bool glowOnInt;
        private bool midRitual = false;

        public List<Rune> Runes => runes ?? (runes = new List<Rune>());
        
        public new CompProperties_Rune Props => (CompProperties_Rune) this.props;
        
        //Needed for initialization
        public CompRune()
        {
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
        
        public override void ReceiveCompSignal(string signal)
        {
        }
        

        public override void CompTick()
        {
            Log.Message("Called!");
            base.CompTick();
            Log.Message("Runes: " + runes.Count);
            foreach (Rune rune in runes)
            {
                Log.Message("Drawing " + rune.Type);
                rune.Graphic.Draw(rune.DrawPosition(parent.Position, 2f), Rot4.North, parent);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Log.Message("Setup!");
            if (midRitual)
            {
                UpdateLit(parent.Map);
                parent.Map.glowGrid.RegisterGlower(this);
            }
            else
            {
                UpdateLit(this.parent.Map);
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
            UpdateLit(map);
        }

        private new void UpdateLit(Map map)
        {
            if (this.glowOnInt == midRitual)
                return;
            this.glowOnInt = midRitual;
            if (!this.glowOnInt)
            {
                map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                map.glowGrid.DeRegisterGlower(this);
            }
            else
            {
                map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                map.glowGrid.RegisterGlower(this);
            }
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref runes, "runes", LookMode.Deep);
        }
    }
}
