using Verse;

namespace OldWorldGods.Defs
{
    public class GodDef : Def
    {
        [Description("If the god is a Chaos god or not")]
        public bool chaos = true;

        [Description("Easter egg name for Warhammer seed")]
        public string easterEggName;
        [Description("Words used to describe the god")]
        public string words;
        [Description("Name set for the god")]
        public RulePackDef names;
        [Description("Effect of the god: blood, poison, time, corruption")]
        public string godEffect;
    }
}
