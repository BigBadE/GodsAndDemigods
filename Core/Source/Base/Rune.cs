namespace OldWorldGods.Base
{
    public class Rune
    {
        private int position;
        private int type;

        public int Position => position;
        public int Type => type;
        
        public Rune(int position, int type)
        {
            this.position = position;
            this.type = type;
        }
    }
}
