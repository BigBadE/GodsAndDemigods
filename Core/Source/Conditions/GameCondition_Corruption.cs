using System.Linq;
using System.Reflection;
using OldWorldGods.Base;
using OldWorldGods.Defs.DefOfs;
using RimWorld;
using Verse;

namespace OldWorldGods.Conditions
{
    public class GameCondition_Corruption : GameCondition
    {
        private static readonly FieldInfo categoryField = typeof(StorytellerCompProperties_OnOffCycle).GetField(
            "category",
            BindingFlags.NonPublic | BindingFlags.Instance);

        public override bool Expired => false;

        public override void Init()
        {
            StorytellerCompProperties_OnOffCycle compProperties =
                new StorytellerCompProperties_OnOffCycle
                {
                    onDays = 1, minSpacingDays = 0.5f, numIncidentsRange = {min = 1, max = 1}
                };

            categoryField.SetValue(compProperties,
                DefDatabase<IncidentCategoryDef>.GetNamed("Colonist_Corruption"));

            StorytellerComp comp = new StorytellerComp_OnOffCycle();
            comp.props = compProperties;
            Find.Storyteller.storytellerComps.Add(comp);

            Gods gods = Current.Game.GetComponent<Gods>();
            gods.playerGod = gods.AllGods.RandomElement();
            if (gods.playerGod != null)
            {
                gods.playerGod.IsPlayerGod = true;
            }
        }

        public override void End()
        {
            base.End();
            StorytellerComp storytellerComp = Find.Storyteller?.storytellerComps?.Find(
                comp => comp is StorytellerComp_OnOffCycle onOffCycle &&
                        (((StorytellerCompProperties_OnOffCycle) onOffCycle.props)?.IncidentCategory?.defName
                        .Equals("Colonist_Corruption") ?? false));
            if (storytellerComp != null)
            {
                Find.Storyteller?.storytellerComps?.Remove(storytellerComp);
            }
            Gods gods = Current.Game.GetComponent<Gods>();
            gods.playerGod.IsPlayerGod = false;
            gods.playerGod = null;
        }

        public void CheckPawns()
        {
            if (!IsValid())
            {
                End();
            }
        }

        private bool IsValid()
        {
            return Find.World.worldObjects.Caravans.Any(caravan => caravan.pawns.InnerListForReading.Any(IsValidPawn)) || 
                   Current.Game.Maps.Any(map => map.PlayerPawnsForStoryteller.Any(IsValidPawn));
        }

        public static bool IsValidPawn(Pawn p)
        {
            return p.Faction.Equals(Faction.OfPlayer) &&
                   !(p.ParentHolder is Building_CryptosleepCasket) &&
                   p.RaceProps.intelligence >= Intelligence.Humanlike &&
                   (ModLister.RoyaltyInstalled && !p.health.hediffSet.HasHediff(ImplantsDefOf.CorruptionProtector));
        }
    }
}
