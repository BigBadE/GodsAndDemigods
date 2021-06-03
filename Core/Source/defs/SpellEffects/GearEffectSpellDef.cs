using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Base;
using RimWorld;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class GearEffectSpellDef : PawnEffectingSpell
    {
        [Description("Item effect")]
        public ItemEffect effect;
        
        [Description("Target apparel layers (nullable)")]
        public List<ApparelLayerDef> apparelLayers;

        [Description("Should effect every item on the pawn")]
        public bool effectAll;

        public override void Execute(Thing ritual, GodDef god, float strength, int casters, List<LocalTargetInfo> targets)
        {
            foreach (LocalTargetInfo thing in targets.Where(thing => thing.Pawn != null))
            {
                List<Apparel> apparels;
                if (effectAll)
                {
                    apparels = thing.Pawn.apparel.WornApparel;
                }
                else
                {
                    apparels = thing.Pawn.apparel.WornApparel.Where(apparel => 
                        apparel.def.apparel.layers.Any(layer => apparelLayers.Contains(layer))).ToList();
                }

                foreach (Apparel apparel in apparels)
                {
                    SpellManager.ApplyItemEffect(effect, strength, apparel);
                }
            }
        }
    }
}
