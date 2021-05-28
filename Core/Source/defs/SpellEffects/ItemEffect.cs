using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class ItemEffect : Def
    {
        [System.ComponentModel.Description("Untaint the item")]
        public bool untaint;

        [System.ComponentModel.Description("Change the item health, uses strength for amount")]
        public bool itemHealthChange;
    }
}
