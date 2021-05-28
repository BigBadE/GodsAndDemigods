using System;
using RimWorld;
using Verse;

namespace OldWorldGods.Thoughts
{
    public class RitualThought : Thought
    {
        private int stageIndex;
        
        public override int CurStageIndex => stageIndex;

        public int SetStageIndex
        {
            get => stageIndex;
            set => stageIndex = Math.Min(value, def.stages.Count);
        }
    }
    
    public class ThoughtWorker_RitualThought : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p) => true;
    }
}
