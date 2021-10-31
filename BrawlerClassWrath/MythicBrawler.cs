using BrawlerClassWrath.Extensions;
using BrawlerClassWrath.Utilities;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlerClassWrath
{
    [HarmonyPatch(typeof(BlueprintsCache), "Init")]
    static class MythicBrawler
    {

        static bool Initialized;

        [HarmonyPriority(Priority.Low)]
        static void Postfix()
        {
            if (Initialized) return;
            Initialized = true;
            Main.LogHeader("Loading RM Brawler Mythic Feats/Abilities");
            MakeBrawlerMythic();
        }

        static void MakeBrawlerMythic()
        {
            var AbundantBrawler = Helpers.CreateBlueprint<BlueprintFeature>("RMBrawlerAbundantMythic");
            var MythicAbilitySelection = Resources.GetBlueprint<BlueprintFeatureSelection>("ba0e5a900b775be4a99702f1ed08914d");
            var ExtraMythicAbilityMythicFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("8a6a511c55e67d04db328cc49aaad2b8");
            AbundantBrawler.SetNameDescription("Abundant Brawling", "You've learned to apply your most dangerous techniques more often than most mortals can dream of.\nBenefit: Your uses of Knockout per day increases by a number of points equal to your mythic level.");
            AbundantBrawler.Ranks = 1;
            AbundantBrawler.ReapplyOnLevelUp = true;
            AbundantBrawler.IsClassFeature = true;
            AbundantBrawler.Groups = new FeatureGroup[]{ FeatureGroup.MythicAbility};
            AbundantBrawler.m_Icon = Resources.GetBlueprint<BlueprintFeature>("e8752f9126d986748b10d0bdac693264").m_Icon;

            AbundantBrawler.AddComponent(Helpers.Create<IncreaseResourceAmountBySharedValue>(c => {
                c.m_Resource = BrawlerClassPatcher.knockout_resource.ToReference<BlueprintAbilityResourceReference>();
                c.Value = new Kingmaker.UnitLogic.Mechanics.ContextValue() {
                    ValueType = Kingmaker.UnitLogic.Mechanics.ContextValueType.Rank
                };
            }));

            AbundantBrawler.AddComponent(Helpers.CreateContextRankConfig(baseValueType: Kingmaker.UnitLogic.Mechanics.Components.ContextRankBaseValueType.MythicLevel, max: 20));

            AbundantBrawler.AddComponent(Helpers.Create<PrerequisiteFeature>(c =>
            {
                c.Group = Prerequisite.GroupType.Any;
                c.m_Feature = BrawlerClassPatcher.knockout.ToReference<BlueprintFeatureReference>();
            } ));

            MythicAbilitySelection.AddFeatures(AbundantBrawler);
            ExtraMythicAbilityMythicFeat.AddFeatures(AbundantBrawler);
        }
    }
}
