using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using OldWorldGods.Needs;
using UnityEngine;
using Verse;

namespace OldWorldGods.Gizmos
{
    public class ChalkGizmo : Gizmo
    {
        public override float GetWidth(float maxWidth) => 212;

        public override bool Visible => Find.Selector.SelectedPawns.Count == 1 && 
                                       Find.Selector.SelectedPawns[0].needs.TryGetNeed<Need_Cult>()?.CurInstantLevel > 0;
        
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Widgets.DrawWindowBackground(new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75));
            
            return new GizmoResult(GizmoState.Clear);
        }
    }

    [HarmonyPatch(typeof(Pawn_InventoryTracker), "GetGizmos")]
    public class InventoryTrackerGizmoPatch
    {
        public static void Postfix(ref IEnumerable<Gizmo> __result)
        {
            __result = __result.Append(new ChalkGizmo());
        }
    }
}
