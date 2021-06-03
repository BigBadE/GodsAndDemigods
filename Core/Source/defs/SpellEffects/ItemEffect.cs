using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class ItemEffect : Def
    {
        [Description("Untaint the item")]
        public bool untaint;

        [Description("Change the item health, uses strength for amount")]
        public bool itemHealthChange;
    }
}
