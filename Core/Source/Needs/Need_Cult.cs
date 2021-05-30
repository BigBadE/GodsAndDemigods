using System;
using System.Collections.Generic;
using System.Linq;
using OldWorldGods.Comps;
using OldWorldGods.Defs.DefOfs;
using RimWorld;
using UnityEngine;
using Verse;

namespace OldWorldGods.Needs
{
    public class Need_Cult : Need
    {
        private float socialModifier;
        private float eventModifier;
        private float practiceModifier;

        private List<Def> cachedApparel;
        private float cachedApparelValue;

        public override bool ShowOnNeedList => curLevelInt > 0;

        public override int GUIChangeArrow
        {
            get
            {
                if (!this.pawn.Awake())
                    return 0;
                float target = GetTargetValue();
                if (Math.Abs(curLevelInt - target) < .001f)
                {
                    return 0;
                }
                if (curLevelInt < target)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public override float CurInstantLevel => GetTargetValue();

        public Need_Cult(Pawn pawn) : base(pawn)
        {
            this.threshPercents = new List<float> {0.25f, 0.5f, 0.65f, 0.80f};
        }

        private float GetApparel()
        {
            if ((pawn.apparel?.WornApparelCount ?? 0) == 0) return 0;
            if (cachedApparel?.Count == pawn.apparel?.WornApparelCount &&
                (pawn.apparel?.WornApparel?.TrueForAll(apparel => cachedApparel.Contains(apparel.def)) ?? true))
            {
                return cachedApparelValue;
            }

            cachedApparel = new List<Def>();
            cachedApparelValue = 0;
            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                cachedApparel.Add(apparel.def);
                cachedApparelValue += apparel.GetComps<CompCultApparel>()?.Sum(comp => comp.Properties.apparelValue) ?? 0;
            }
            
            cachedApparelValue = Math.Max(1, cachedApparelValue);
            return cachedApparelValue;
        }
        
        public override void SetInitialLevel() { }

        public void SocialInteraction(bool positive)
        {
            if (ModLister.RoyaltyInstalled &&
                pawn.health.hediffSet.HasHediff(ImplantsDefOf.CorruptionProtector)) return;
            SetWithMax(ref socialModifier, (positive) ? .15f : -.15f);
        }

        public void EventTrigger(float change)
        {
            if (ModLister.RoyaltyInstalled &&
                pawn.health.hediffSet.HasHediff(ImplantsDefOf.CorruptionProtector)) return;
            SetWithMax(ref eventModifier, change);
        }

        public void Notify_ImplantInstalled()
        {
            curLevelInt = 0;
            eventModifier = 0;
            socialModifier = 0;
        }

        private static void SetWithMax(ref float value, float change, float max = 1)
        {
            value = Mathf.Max(0, Mathf.Min(max, value + change));
        }

        public override void NeedInterval()
        {
            curLevelInt += Mathf.Max(-.01f, Mathf.Min(.01f, GetTargetValue()-curLevelInt));
        }
        
        private float GetTargetValue()
        {
            return (socialModifier + eventModifier + practiceModifier) / 6 + Math.Max(pawn.needs.mood.CurLevel, 0) / 4 + GetApparel() / 4;
        }
    }
}
