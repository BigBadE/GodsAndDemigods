using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OldWorldGods.Defs;
using Verse;

namespace OldWorldGods.Base
{
    public class PlayerSpellManager : IExposable
    {
        private List<SpellDef> foundSpells = new List<SpellDef>();
        private SpellDef learningSpell;
        private int tier = -1;
        
        public List<SpellDef> FoundSpells => foundSpells;

        [CanBeNull] public SpellDef LearningSpell => learningSpell ?? (learningSpell = GetLearningSpell());

        private SpellDef GetLearningSpell()
        {
            God playerGod = Current.Game.GetComponent<Gods>().playerGod;
            if (playerGod == null) return null;
            List<SpellDef> newSpells = playerGod.GetDef.spells
                .Where(spellDef => spellDef.tier == tier && !foundSpells.Contains(spellDef)).ToList();
            if (newSpells.Any())
            {
                return newSpells.RandomElement();
            }

            return DefDatabase<SpellDef>.AllDefsListForReading.Where(spellDef => spellDef.tier == tier
                && !foundSpells.Contains(spellDef)).RandomElement();
        }

        public void LearnSpell(SpellDef spellDef)
        {
            if(foundSpells.Contains(spellDef)) return;
            foundSpells.Add(spellDef);
            if (spellDef != learningSpell) return;
            learningSpell = null;
            if (foundSpells.Count(found => found.tier == tier) >=
                Current.Game.GetComponent<Gods>().playerGod?.GetDef.spells.Count(spell => spell.tier == tier) + 2)
            {
                tier++;
            }

        }
        
        public void ExposeData()
        {
            Scribe_Defs.Look(ref learningSpell, "learningSpell");
            Scribe_Collections.Look(ref foundSpells, "foundSpells", LookMode.Def);

            if (tier != -1) return;
            foreach (SpellDef spellDef in foundSpells.Where(spellDef => spellDef.tier > tier))
            {
                tier = spellDef.tier;
            }
        }
    }
}
