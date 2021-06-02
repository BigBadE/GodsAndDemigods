using Verse;

namespace OldWorldGods.Base
{
    public class Rune : IExposable
    {
        private int position;
        private int type;

        public int Position => position;
        public int Type { get => type; set => type = value; }

        //For saving/loading
        public Rune()
        {
        }
        
        public Rune(int position, int type)
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
