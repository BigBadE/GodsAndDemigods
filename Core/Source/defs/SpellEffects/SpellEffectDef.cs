using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OldWorldGods.Defs.SpellEffects
{
    public abstract class SpellEffectDef
    {
        [System.ComponentModel.Description("Total targets")]
        public SpellNumber targets;
        
        [System.ComponentModel.Description("Strength")]
        public SpellNumber strength;

        [System.ComponentModel.Description("Max casters")] public int maxCasters;
        [System.ComponentModel.Description("Min casters")] public int minCasters;
        
        [System.ComponentModel.Description("Cast particles (nullable)")]
        public Texture2D particle;

        public abstract void Execute(GodDef god, float strength, int casters, List<Thing> targets);
    }
}
