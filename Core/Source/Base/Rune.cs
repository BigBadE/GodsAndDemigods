using OldWorldGods.Defs;
using UnityEngine;
using Verse;

namespace OldWorldGods.Base
{
    public class Rune : IExposable
    {
        private Graphic graphic;
        private Vector3? drawPosition;
        private IntVec3? drawOffset;
        private RuneDef type;
        private int position;

        public int Position => position;
        public RuneDef Type
        {
            get => type;
            set
            {
                type = value;
                graphic = null;
            }
        }

        public Graphic Graphic => graphic ?? (graphic = RuneManager.GetRuneGraphic(this));

        public Vector3 DrawPosition(IntVec3 offset, float scale)
        {
            if (drawOffset == offset && drawPosition != null) return drawPosition.Value;
            
            //Sin/Cos with a period of 8 (simplified)
            drawPosition = new Vector3(Mathf.Sin(Mathf.PI * Position / 4) * scale,
                AltitudeLayer.MoteBelowThings.AltitudeFor(),
                -Mathf.Cos(Mathf.PI * Position / 4) * scale) + new Vector3(offset.x + .5f, 0, offset.z + .5f);
            drawOffset = offset;
            
            return drawPosition.Value;
        }
        
        //For saving/loading
        public Rune()
        {
        }
        
        public Rune(int position, RuneDef type)
        {
            this.position = position;
            this.type = type;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref position, "position");
            Scribe_Values.Look(ref type, "type");
        }
    }
}
