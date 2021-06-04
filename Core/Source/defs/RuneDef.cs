using Verse;

namespace OldWorldGods.Defs
{
    public class RuneDef : Def
    {
        [Description("Description of the rune")]
        public string descripton;

        [Description("Index of rune, starting with 0")]
        public int index;
        
        
        [Description("Image name")]
        public int image;
    }
}
