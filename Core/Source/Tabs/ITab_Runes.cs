using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OldWorldGods.Base;
using OldWorldGods.Comps;
using OldWorldGods.Defs;
using RimWorld;
using UnityEngine;
using Verse;

namespace OldWorldGods.Tabs
{
    public class ITab_Runes : ITab
    {
        private List<Rune> runes;
        
        [CanBeNull] 
        private Texture2D rune;
        [CanBeNull] 
        private Texture2D cast;
        
        private Vector2 WinSize = new Vector2(432, 467);
        
        public override bool IsVisible => true;

        private Texture2D RuneTexture => rune ? rune : (rune = ContentFinder<Texture2D>.Get("Buildings/Runes/Main_Rune"));
        private Texture2D CastTexture => cast ? cast : (cast = ContentFinder<Texture2D>.Get("Buildings/Runes/Rune_Cast"));
        
        public ITab_Runes()
        {
            this.size = WinSize;
            this.labelKey = "TabRunes";
            this.tutorTag = "Runes";
        }

        private float CalculateOffset(float input)
        {
            return input * (1 - 20f / 128) * (WinSize.x / 2 - 20) + (WinSize.x - 10) / 2;
        }
        
        protected override void FillTab()
        {
            Rect rect1 = new Rect(0.0f, 0.0f, WinSize.x, WinSize.y).ContractedBy(10f);
            Text.Font = GameFont.Medium;
            Widgets.Label(rect1, "TabRunesTitle".Translate());
            Rect rect2 = rect1;
            rect2.yMin += 35f;
            Widgets.DrawTextureFitted(rect2, RuneTexture, 1);
            if (runes == null)
            {
                runes = SelThing.TryGetComp<CompRune>().Runes.ListFullCopy();
            }

            for (int i = 0; i < 8; i++)
            {
                Rune found = runes.Find(testing => testing.Position == i);
                if (!Widgets.ButtonImage(new Rect(
                        new Vector2(CalculateOffset(Mathf.Sin(Mathf.PI * i / 4))-20,
                            CalculateOffset(Mathf.Cos(Mathf.PI * i / 4))+5),
                        new Vector2(50, 50)),
                    found == null ? Texture2D.normalTexture : 
                        (Texture2D) RuneManager.GetRuneGraphic(found).MatNorth.mainTexture)) continue;
                if (found == null)
                {
                    runes.Add(new Rune(i, 0));
                } else if (found.Type == 9)
                {
                    runes.Remove(found);
                }
                else
                {
                    found.Type++;
                }
            }

            List<SpellDef> foundSpells = Current.Game.GetComponent<Gods>().FoundSpells;
            if (foundSpells.Any() && 
                Widgets.ButtonText(new Rect(new Vector2(0, 0), new Vector2(125, 50)), "FoundRunes".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(foundSpells.Select(spell => 
                    new FloatMenuOption(spell.label, () => runes = RuneManager.GetRunes(spell))).ToList()));
            }

            if (Widgets.ButtonImage(new Rect(rect2.size / 2, new Vector2(50, 50)), CastTexture))
            {
                SelThing.TryGetComp<CompRune>().SetRunes(runes);
            }
        }

        protected override void CloseTab()
        {
            runes = null;
        }
    }
}
