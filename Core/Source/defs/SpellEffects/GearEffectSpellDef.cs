using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Base;
using RimWorld;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public class GearEffectSpellDef : SpellEffectDef
    {
        [System.ComponentModel.Description("Item effect")]
        public ItemEffect effect;
        
        [System.ComponentModel.Description("Target apparel layers (nullable)")]
        public List<ApparelLayerDef> apparelLayers;

        [System.ComponentModel.Description("Should effect every item on the pawn")]
        public bool effectAll;

        public override void Execute(GodDef god, float strength, int casters, List<Thing> targets)
        {
            foreach (Thing thing in targets)
            {
                if (!(thing is Pawn pawn)) continue;
                List<Apparel> apparels;
                if (effectAll)
                {
                    apparels = pawn.apparel.WornApparel;
                }
                else
                {
                    apparels = pawn.apparel.WornApparel.Where(apparel => 
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
