using System;
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

        public override bool ShowOnNeedList => curLevelInt > 0;

        public override int GUIChangeArrow => (int) Math.Round(GetTargetValue()-curLevelInt*10);
        
        public override float CurInstantLevel => GetTargetValue();
        
        public Need_Cult()
        {
        }
        
        public Need_Cult(Pawn pawn) : base(pawn)
        {
        }

        public override void SetInitialLevel() { }
        
        public void SocialInteraction(bool positive)
        {
            if(ModLister.RoyaltyInstalled && pawn.health.hediffSet.HasHediff(ImplantsDefOf.CorruptionProtector)) return;
            SetWithMax(ref socialModifier, (positive) ? .15f : -.15f);
        }

        public void EventTrigger(float change)
        {
            if(ModLister.RoyaltyInstalled && pawn.health.hediffSet.HasHediff(ImplantsDefOf.CorruptionProtector)) return;
            SetWithMax(ref eventModifier, change);
        }

        public void Notify_ImplantInstalled()
        {
            curLevelInt = 0;
            eventModifier = 0;
            socialModifier = 0;
        }
        
        private static void SetWithMax(ref float value, float change)
        {
            value = Mathf.Max(-1, Mathf.Min(1, value + change));
        }
        
        public override void NeedInterval()
        {
            float target = GetTargetValue();
            if (target > curLevelInt)
            {
                curLevelInt += .005f;
            }
            else if(target < curLevelInt)
            {
                curLevelInt -= .005f;
            }
        }

        private float GetTargetValue()
        {
            return (socialModifier + eventModifier) / 2;
        }
    }
}
