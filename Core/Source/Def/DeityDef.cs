using System.Collections.Generic;
using RimWorld;
using Verse;

namespace GodsAndDemigods.Def
{
    public class DeityDef : Verse.Def
    {
        public List<AbilityDef> abilityDefs;

        public List<Gender> genders;
        public List<MemeDef> possibleMemes;
        
        [NoTranslate]
        public string iconFolderPath;

        public string name;
        
        //A special god type for Royalty
        public bool royaltyGod;
    }
}
