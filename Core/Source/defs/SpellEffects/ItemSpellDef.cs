using System;
using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Buildings;
using RimWorld;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class ItemSpellDef : SpellEffectDef
    {
        [Description("Effect on item")]
        public ItemEffect effect;

        public override void Execute(Thing ritual, GodDef god, float strength, int casters, List<LocalTargetInfo> targets)
        {
            foreach (LocalTargetInfo thing in targets.Where(thing => thing.Thing != null))
            {
                Thing found = thing.Thing;
                
                if (effect.itemHealthChange && found.HitPoints < found.MaxHitPoints)
                {
                    found.HitPoints = (int) Math.Min(found.MaxHitPoints, found.HitPoints + strength*5);
                }

                if (effect.untaint && found is Apparel apparel)
                {
                    apparel.Notify_PawnResurrected();
                }
            }
        }

        public override List<LocalTargetInfo> GetTargets(Thing ritual)
        {
            return ritual.Map.thingGrid.ThingsListAt(ritual.Position).Where(thing => !(thing is Building_Rune)).ToList()
                .ConvertAll(thing => (LocalTargetInfo) thing);
        }
    }
}
