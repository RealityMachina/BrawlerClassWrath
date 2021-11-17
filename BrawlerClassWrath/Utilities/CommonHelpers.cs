using BrawlerClassWrath.Extensions;
using BrawlerClassWrath.NewMechanics;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

namespace BrawlerClassWrath.Utilities
{
    static class CommonHelpers
    {
        public static BlueprintCharacterClass animal_class = Resources.GetBlueprint<BlueprintCharacterClass>("4cd1757a0eea7694ba5c933729a53920");
        public static BlueprintCharacterClass dragon_class = Resources.GetBlueprint<BlueprintCharacterClass>("01a754e7c1b7c5946ba895a5ff0faffc");
        public static BlueprintFeature undead = Resources.GetBlueprint<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33");
        public static BlueprintFeature dragon = Resources.GetBlueprint<BlueprintFeature>("455ac88e22f55804ab87c2467deff1d6");
        public static BlueprintFeature fey = Resources.GetBlueprint<BlueprintFeature>("018af8005220ac94a9a4f47b3e9c2b4e");
        public static BlueprintFeature construct = Resources.GetBlueprint<BlueprintFeature>("fd389783027d63343b4a5634bd81645f");
        public static BlueprintFeature elemental = Resources.GetBlueprint<BlueprintFeature>("198fd8924dabcb5478d0f78bd453c586");
        public static BlueprintFeature outsider = Resources.GetBlueprint<BlueprintFeature>("9054d3988d491d944ac144e27b6bc318");
        public static BlueprintFeature plant = Resources.GetBlueprint<BlueprintFeature>("706e61781d692a042b35941f14bc41c5");
        public static BlueprintFeature animal = Resources.GetBlueprint<BlueprintFeature>("a95311b3dc996964cbaa30ff9965aaf6");
        public static BlueprintFeature magical_beast = Resources.GetBlueprint<BlueprintFeature>("625827490ea69d84d8e599a33929fdc6");
        public static BlueprintFeature monstrous_humanoid = Resources.GetBlueprint<BlueprintFeature>("57614b50e8d86b24395931fffc5e409b");
        public static BlueprintFeature giant_humanoid = Resources.GetBlueprint<BlueprintFeature>("f9c388137f4faa74aac9065a68b56880");
        public static BlueprintFeature aberration = Resources.GetBlueprint<BlueprintFeature>("3bec99efd9a363242a6c8d9957b75e91");
        public static BlueprintFeature vermin = Resources.GetBlueprint<BlueprintFeature>("09478937695300944a179530664e42ec");
        public static BlueprintFeature incorporeal = Resources.GetBlueprint<BlueprintFeature>("c4a7f98d743bc784c9d4cf2105852c39");

        public static LocalizedString tenMinPerLevelDuration = Resources.GetBlueprint<BlueprintAbility>("5b77d7cc65b8ab74688e74a37fc2f553").LocalizedDuration; // barkskin
        public static LocalizedString minutesPerLevelDuration = Resources.GetBlueprint<BlueprintAbility>("ef768022b0785eb43a18969903c537c4").LocalizedDuration; // shield
        public static LocalizedString hourPerLevelDuration = Resources.GetBlueprint<BlueprintAbility>("9e1ad5d6f87d19e4d8883d63a6e35568").LocalizedDuration; // mage armor
        public static LocalizedString roundsPerLevelDuration = Resources.GetBlueprint<BlueprintAbility>("486eaff58293f6441a5c2759c4872f98").LocalizedDuration; // haste
        public static LocalizedString oneRoundDuration = Resources.GetBlueprint<BlueprintAbility>("2c38da66e5a599347ac95b3294acbe00").LocalizedDuration; // true strike
        public static LocalizedString oneMinuteDuration = Resources.GetBlueprint<BlueprintAbility>("93f391b0c5a99e04e83bbfbe3bb6db64").LocalizedDuration; // protection from evil communal
        public static LocalizedString reflexHalfDamage = Resources.GetBlueprint<BlueprintAbility>("2d81362af43aeac4387a3d4fced489c3").LocalizedSavingThrow; // fireball
        public static LocalizedString savingThrowNone = Resources.GetBlueprint<BlueprintAbility>("b6010dda6333bcf4093ce20f0063cd41").LocalizedSavingThrow; // frigid touch
        public static LocalizedString willNegates = Resources.GetBlueprint<BlueprintAbility>("8bc64d869456b004b9db255cdd1ea734").LocalizedSavingThrow; //bane
        public static LocalizedString fortNegates = Resources.GetBlueprint<BlueprintAbility>("48e2744846ed04b4580be1a3343a5d3d").LocalizedSavingThrow; //contagion



        public static AddFeatureOnClassLevel CreateAddFeatureOnClassLevel(this BlueprintFeature feat, int level, BlueprintCharacterClass[] classes, BlueprintArchetype[] archetypes = null, bool before = false)
        {
            var a = Helpers.Create<AddFeatureOnClassLevel>();
            a.name = $"AddFeatureOnClassLevel${feat.name}";
            a.Level = level;
            a.BeforeThisLevel = before;
            a.m_Feature = feat.ToReference<BlueprintFeatureReference>();
            a.m_Class = classes[0].ToReference<BlueprintCharacterClassReference>();
            BlueprintCharacterClassReference[] addClasses = new BlueprintCharacterClassReference[0];

            for(var i = 1; i < addClasses.Count(); i++)
            {
                addClasses[i - 1] = classes[i].ToReference<BlueprintCharacterClassReference>();
            }
            a.m_AdditionalClasses = addClasses;
            BlueprintArchetypeReference[] addArchs = new BlueprintArchetypeReference[0];
            if (archetypes != null)
            {
               addArchs = new BlueprintArchetypeReference[archetypes.Count() - 1];
            }
            for (var i = 1; i < addArchs.Count(); i++)
            {
                addArchs[i] = archetypes[i].ToReference<BlueprintArchetypeReference>();
            }
            a.m_Archetypes = archetypes == null ? new BlueprintArchetypeReference[0] : addArchs;
            return a;
        }
        public static AbilityTargetsAround CreateAbilityTargetsAround(Feet radius, TargetType targetType, ConditionsChecker conditions = null, Feet spreadSpeed = default(Feet), bool includeDead = false)
        {
            var around = Helpers.Create<AbilityTargetsAround>();
            around.m_Radius = radius;
            around.m_TargetType = targetType;
            around.m_IncludeDead = includeDead;
            around.m_Condition = conditions ?? new ConditionsChecker() { Conditions = Array.Empty<Condition>() };
            around.m_SpreadSpeed = spreadSpeed;
            return around;
        }

        public static void setMiscAbilityParametersSingleTargetRangedHarmful(this BlueprintAbility ability, bool works_on_allies = false,
                                                               Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Point,
                                                               Kingmaker.View.Animation.CastAnimationStyle animation_style = Kingmaker.View.Animation.CastAnimationStyle.CastActionPoint)
        {
            ability.CanTargetFriends = works_on_allies;
            ability.CanTargetEnemies = true;
            ability.CanTargetSelf = false;
            ability.CanTargetPoint = false;
            ability.EffectOnEnemy = AbilityEffectOnUnit.Harmful;
            ability.EffectOnAlly = works_on_allies ? AbilityEffectOnUnit.Harmful : AbilityEffectOnUnit.None;
            ability.Animation = animation;
            ability.AnimationStyle = animation_style;
        }

        public static AbilitySpawnFx createAbilitySpawnFx(string asset_id, AbilitySpawnFxAnchor position_anchor = AbilitySpawnFxAnchor.None,
                                                                           AbilitySpawnFxAnchor orientation_anchor = AbilitySpawnFxAnchor.None,
                                                                           AbilitySpawnFxAnchor anchor = AbilitySpawnFxAnchor.None)
        {
            var a = Helpers.Create<AbilitySpawnFx>();
            a.PrefabLink = createPrefabLink(asset_id);
            a.PositionAnchor = position_anchor;
            a.OrientationAnchor = orientation_anchor;
            a.Anchor = anchor;

            return a;
        }

        static public ContextActionRemoveBuffFromCaster createContextActionRemoveBuffFromCaster(BlueprintBuff buff, int delay = 0)
        {
            var c = Helpers.Create<ContextActionRemoveBuffFromCaster>();
            c.Buff = buff;
            c.remove_delay_seconds = delay;
            return c;
        }
        static public ContextConditionCasterHasFact createContextConditionCasterHasFact(BlueprintUnitFact fact, bool has = true)
        {
            var c = Helpers.Create<ContextConditionCasterHasFact>();
            c.m_Fact = fact.ToReference<BlueprintUnitFactReference>();
            c.Not = !has;
            return c;
        }
        static public AddInitiatorAttackWithWeaponTrigger createAddInitiatorAttackWithWeaponTriggerWithCategory(ActionList action, bool only_hit = true, bool critical_hit = false,
                                                                                              bool check_weapon_range_type = false, bool reduce_hp_to_zero = false,
                                                                                              bool on_initiator = false,
                                                                                              WeaponRangeType range_type = WeaponRangeType.Melee,
                                                                                              bool wait_for_attack_to_resolve = false, bool only_first_hit = false,
                                                                                              WeaponCategory weapon_category = WeaponCategory.UnarmedStrike)
        {
            var t = Helpers.Create<AddInitiatorAttackWithWeaponTrigger>();
            t.Action = action;
            t.OnlyHit = only_hit;
            t.CriticalHit = critical_hit;
            t.CheckWeaponRangeType = check_weapon_range_type;
            t.RangeType = range_type;
            t.ReduceHPToZero = reduce_hp_to_zero;
            t.ActionsOnInitiator = on_initiator;
            t.WaitForAttackResolve = wait_for_attack_to_resolve;
            t.OnlyOnFirstAttack = only_first_hit;
            t.CheckWeaponCategory = true;
            t.Category = weapon_category;
            return t;
        }

        public static AddFactContextActions CreateAddFactContextActions(GameAction[] activated = null, GameAction[] deactivated = null, GameAction[] newRound = null)
        {
            var a = Helpers.Create<AddFactContextActions>();
            a.Activated = CreateActionList(activated);
            a.Deactivated = CreateActionList(deactivated);
            a.NewRound = CreateActionList(newRound);
            return a;
        }

        static public ContextActionApplyBuff createContextActionApplyChildBuff(BlueprintBuff buff)
        {
            return createContextActionApplyBuff(buff, CreateContextDuration(), is_child: true, is_permanent: true, dispellable: false);
        }


        public static PrefabLink createPrefabLink(string asset_id)
        {
            var link = new PrefabLink();
            link.AssetId = asset_id;
            return link;
        }
        public static AddCondition createAddCondition(UnitCondition condition)
        {
            var a = Helpers.Create<AddCondition>();
            a.Condition = condition;
            return a;
        }
        public static SpellDescriptorComponent CreateSpellDescriptor(this SpellDescriptor descriptor)
        {
            var s = Helpers.Create<SpellDescriptorComponent>();
            s.Descriptor = descriptor;
            return s;
        }
        public static SpellDescriptorComponent CreateSpellDescriptor()
        {
            var s = Helpers.Create<SpellDescriptorComponent>();
            s.Descriptor = SpellDescriptor.None;
            return s;
        }
        public static BlueprintBuff dazed_non_mind_affecting = Helpers.CreateBuff("DazedNonMindAffectingBuff", bp => {
            bp.SetName("Dazed");
            bp.SetDescription("The creature is unable to act normally. A dazed creature can take no actions, but has no penalty to AC.\nA dazed condition typically lasts 1 round.");
            bp.m_Icon = Helpers.GetIcon("9934fedff1b14994ea90205d189c8759");
            bp.FxOnStart = createPrefabLink("396af91a93f6e2b468f5fa1a944fae8a");
            bp.AddComponent(createAddCondition(UnitCondition.Dazed));
            bp.AddComponent(CreateSpellDescriptor(SpellDescriptor.Daze));
           });
        static public PrerequisiteAlignment createPrerequisiteAlignment(Kingmaker.UnitLogic.Alignments.AlignmentMaskType alignment)
        {
            var p = Helpers.Create<PrerequisiteAlignment>();
            p.Alignment = alignment;
            return p;
        }
        public static AddAbilityResources CreateAddAbilityResource(this BlueprintAbilityResource resource)
        {
            var a = Helpers.Create<AddAbilityResources>();
            a.m_Resource = resource.ToReference<BlueprintAbilityResourceReference>();
            a.RestoreAmount = true;
            return a;
        }


        static public BlueprintFeature ActivatableAbilityToFeature(BlueprintActivatableAbility ability, bool hide = true, string guid = "")
        {
            var feature = CreateFeature(ability.name + "Feature",
                                                     ability.Name,
                                                     ability.Description,
                                                     ability.Icon,
                                                     FeatureGroup.None,
                                                     CreateAddFact(ability)
                                                     );
            if (hide)
            {
                feature.HideInCharacterSheetAndLevelUp = true;
                feature.HideInUI = true;
            }
            return feature;
        }
        static public BlueprintFeature AbilityToFeature(BlueprintAbility ability, bool hide = true)
        {
            var feature = CreateFeature(ability.name + "Feature",
                                                     ability.Name,
                                                     ability.Description,
                                                     ability.Icon,
                                                     FeatureGroup.None
                                                     );
            feature.AddComponent(createAddFeatureIfHasFact(ability, ability, not: true));
            if (hide)
            {
                feature.HideInCharacterSheetAndLevelUp = true;
                feature.HideInUI = true;
            }
            return feature;
        }
        static public AddFeatureIfHasFact createAddFeatureIfHasFact(BlueprintUnitFact fact, BlueprintUnitFact feature, bool not = false)
        {
            var a = Helpers.Create<AddFeatureIfHasFact>();
            a.m_CheckedFact = fact.ToReference<BlueprintUnitFactReference>();
            a.m_Feature = feature.ToReference<BlueprintUnitFactReference>();
            a.Not = not;
            return a;
        }

        public static AbilityResourceLogic CreateResourceLogic(this BlueprintAbilityResource resource, bool spend = true, int amount = 1, bool cost_is_custom = false)
        {
            var a = Helpers.Create<AbilityResourceLogic>();
            a.m_IsSpendResource = spend;
            a.m_RequiredResource = resource.ToReference<BlueprintAbilityResourceReference>();
            a.Amount = amount;
            a.CostIsCustom = cost_is_custom;
            return a;
        }

        public static void setMiscAbilityParametersSelfOnly(this BlueprintAbility ability,
                                                               Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Self,
                                                               Kingmaker.View.Animation.CastAnimationStyle animation_style = Kingmaker.View.Animation.CastAnimationStyle.CastActionSelf)
        {
            ability.CanTargetFriends = false;
            ability.CanTargetEnemies = false;
            ability.CanTargetSelf = true;
            ability.CanTargetPoint = false;
            ability.EffectOnEnemy = AbilityEffectOnUnit.None;
            ability.EffectOnAlly = AbilityEffectOnUnit.Helpful;
            ability.Animation = animation;
            ability.AnimationStyle = animation_style;
        }
        public static AbilityVariants CreateAbilityVariants(this BlueprintAbility parent, IEnumerable<BlueprintAbility> variants) => CreateAbilityVariants(parent, variants.ToArray());

        public static AbilityVariants CreateAbilityVariants(this BlueprintAbility parent, params BlueprintAbility[] variants)
        {
            var a = Helpers.Create<AbilityVariants>();

            BlueprintAbilityReference[] newVar = new BlueprintAbilityReference[variants.Count()];
            var i = 0;
            foreach(var variant in variants)
            {
                newVar[i] = variant.ToReference<BlueprintAbilityReference>();
                i++;
            }
            a.m_Variants = newVar;
            foreach (var v in variants)
            {
                v.Parent = parent;
            }
            return a;
        }

        public static AddFacts CreateAddFact(this BlueprintUnitFact fact)
        {
            var result = Helpers.Create<AddFacts>();
            result.name = $"AddFacts${fact.name}";
            result.m_Facts = new BlueprintUnitFactReference[] { fact.ToReference<BlueprintUnitFactReference>() };
            return result;
        }
        public static AddFacts CreateAddFacts(params BlueprintUnitFact[] facts)
        {
            BlueprintUnitFactReference[] newVar = new BlueprintUnitFactReference[facts.Count()];
            var i = 0;
            foreach (var fact in facts)
            {
                newVar[i] = fact.ToReference<BlueprintUnitFactReference>();
                i++;
            }
            var result = Helpers.Create<AddFacts>();
            result.m_Facts = newVar;
            return result;
        }

        public static BlueprintAbility createVariantWrapper(string name, string guid, params BlueprintAbility[] variants)
        {
           // var wrapper = library.CopyAndAdd<BlueprintAbility>(variants[0].AssetGuid, name, guid);
            var wrapper = CreateAbility(name, variants[0].m_DisplayName, variants[0].m_Description, guid, variants[0].m_Icon, variants[0].Type, variants[0].ActionType, variants[0].Range,
                variants[0].LocalizedDuration, variants[0].LocalizedSavingThrow);

            //wrapper.SetDescription("");
            List<BlueprintComponent> components = new List<BlueprintComponent>();
            components.Add(CreateAbilityVariants(wrapper, variants));
            wrapper.ComponentsArray = components.ToArray();

            return wrapper;
        }
        static public void addContextActionApplyBuffOnConditionToActivatedAbilityBuffNoRemove(BlueprintBuff target_buff, Conditional conditional_action)
        {
            if (target_buff.GetComponent<AddFactContextActions>() == null)
            {
                target_buff.AddComponent(Helpers.CreateEmptyAddFactContextActions());
            }

            var activated = target_buff.GetComponent<AddFactContextActions>().Activated;
            activated.Actions = activated.Actions.AppendToArray(conditional_action);
        }
        static public AbilityShowIfCasterHasFact createAbilityShowIfCasterHasFact(BlueprintUnitFact fact)
        {
            var a = Helpers.Create<AbilityShowIfCasterHasFact>();
            a.m_UnitFact = fact.ToReference<BlueprintUnitFactReference>();
            return a;
        }
        public static BlueprintAbility CreateAbility(String name, String displayName,
            String description, String guid, UnityEngine.Sprite icon,
            AbilityType type, CommandType actionType, AbilityRange range,
            String duration, String savingThrow,
            params BlueprintComponent[] components)
        {
            var ability = Helpers.CreateBlueprint<BlueprintAbility>(name);
            ability.name = name;
            ability.SetComponents(components);
            ability.SetNameDescription(displayName, description);
            ability.m_Icon = icon;
            ability.ResourceAssetIds = Array.Empty<string>();

            ability.Type = type;
            ability.ActionType = actionType;
            ability.Range = range;
            ability.LocalizedDuration = Main.MakeLocalizedString($"{name}.Duration", duration);
            ability.LocalizedSavingThrow = Main.MakeLocalizedString($"{name}.SavingThrow", savingThrow);
           
            return ability;
        }

        public static ContextCalculateAbilityParamsBasedOnClasses createContextCalculateAbilityParamsBasedOnClassesWithProperty(BlueprintCharacterClass[] character_classes,
                                                                                                                                    BlueprintUnitProperty property,
                                                                                                                                    StatType stat = StatType.Charisma)
        {
            var c = Helpers.Create<ContextCalculateAbilityParamsBasedOnClasses>();
            c.CharacterClasses = character_classes;
            c.StatType = stat;
            c.property = property;
            return c;
        }
        public static ContextCalculateAbilityParamsBasedOnClass createContextCalculateAbilityParamsBasedOnClass(BlueprintCharacterClass character_class,
                                                                                                                                                    StatType stat, bool use_kineticist_main_stat = false)
        {
            var c = Helpers.Create<ContextCalculateAbilityParamsBasedOnClass>();
            c.m_CharacterClass = character_class.ToReference<BlueprintCharacterClassReference>();
            c.StatType = stat;
            c.UseKineticistMainStat = use_kineticist_main_stat;
            return c;
        }


        public static ContextCalculateAbilityParamsBasedOnClasses createContextCalculateAbilityParamsBasedOnClasses(BlueprintCharacterClass[] character_classes,
                                                                                                                                            StatType stat)
        {
            var c = Helpers.Create<ContextCalculateAbilityParamsBasedOnClasses>();
            c.CharacterClasses = character_classes;
            c.StatType = stat;
            return c;
        }

        static public ContextActionRemoveBuff createContextActionRemoveBuff(BlueprintBuff buff)
        {
            var r = Helpers.Create<ContextActionRemoveBuff>();
            r.m_Buff = buff.ToReference<BlueprintBuffReference>();
            return r;
        }


        static public ContextActionOnContextCaster createContextActionOnContextCaster(params GameAction[] actions)
        {
            var c = Helpers.Create<ContextActionOnContextCaster>();
            c.Actions = Helpers.CreateActionList(actions);
            return c;
        }

        static public AddInitiatorAttackWithWeaponTrigger createAddInitiatorAttackWithWeaponTrigger(Kingmaker.ElementsSystem.ActionList action, bool only_hit = true, bool critical_hit = false,
                                                                                                      bool check_weapon_range_type = false, bool reduce_hp_to_zero = false,
                                                                                                      bool on_initiator = false,
                                                                                                      WeaponRangeType range_type = WeaponRangeType.Melee,
                                                                                                      bool wait_for_attack_to_resolve = false, bool only_first_hit = false)
        {
            var t = Helpers.Create<AddInitiatorAttackWithWeaponTrigger>();
            t.Action = action;
            t.OnlyHit = only_hit;
            t.CriticalHit = critical_hit;
            t.CheckWeaponRangeType = check_weapon_range_type;
            t.RangeType = range_type;
            t.ReduceHPToZero = reduce_hp_to_zero;
            t.ActionsOnInitiator = on_initiator;
            t.WaitForAttackResolve = wait_for_attack_to_resolve;
            t.OnlyOnFirstAttack = only_first_hit;

            return t;
        }


        public static BlueprintBuff CreateBuff(String name, String displayName, String description,  UnityEngine.Sprite icon,
            PrefabLink fxOnStart,
            params BlueprintComponent[] components)
        {
            var buff = Helpers.CreateBuff(name);
            buff.FxOnStart = fxOnStart ?? new PrefabLink();
            buff.FxOnRemove = new PrefabLink();
            buff.SetComponents(components);
            buff.SetNameDescription(displayName, description);
            buff.m_Icon = icon;
            buff.Ranks = 1;
            buff.IsClassFeature = true;
            return buff;
        }

        public static NewMechanics.ContextConditionHasFacts createContextConditionHasFacts(bool all, params BlueprintUnitFact[] facts)
        {
            var c = Helpers.Create<NewMechanics.ContextConditionHasFacts>();
            c.all = all;
            c.Facts = facts;
            return c;
        }

        static public ContextConditionHasFact createContextConditionHasFact(BlueprintUnitFact fact, bool has = true)
        {
            var c = Helpers.Create<ContextConditionHasFact>();
            c.m_Fact = fact.ToReference<BlueprintUnitFactReference>();
            c.Not = !has;
            return c;
        }

        public static ContextActionConditionalSaved CreateConditionalSaved(GameAction[] success, GameAction[] failed)
        {
            var c = Helpers.Create<ContextActionConditionalSaved>();
            c.Succeed = CreateActionList(success);
            c.Failed = CreateActionList(failed);
            return c;
        }

        public static ContextActionConditionalSaved CreateConditionalSaved(GameAction success, GameAction failed)
        {
            return CreateConditionalSaved(success == null ? new GameAction[0] : new GameAction[] { success }, failed == null ? new GameAction[0] : new GameAction[] { failed });
        }
        public static Conditional CreateConditional(Condition condition, GameAction ifTrue, GameAction ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = CreateConditionsCheckerAnd(condition);
            c.IfTrue = CreateActionList(ifTrue);
            c.IfFalse = CreateActionList(ifFalse);
            return c;
        }

        public static Conditional CreateConditional(Condition[] condition, GameAction ifTrue, GameAction ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = CreateConditionsCheckerAnd(condition);
            c.IfTrue = CreateActionList(ifTrue);
            c.IfFalse = CreateActionList(ifFalse);
            return c;
        }


        public static Conditional CreateConditionalOr(Condition[] condition, GameAction ifTrue, GameAction ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = CreateConditionsCheckerOr(condition);
            c.IfTrue = CreateActionList(ifTrue);
            c.IfFalse = CreateActionList(ifFalse);
            return c;
        }

        public static Conditional CreateConditional(Condition[] condition, GameAction[] ifTrue, GameAction[] ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = CreateConditionsCheckerAnd(condition);
            c.IfTrue = CreateActionList(ifTrue);
            c.IfFalse = CreateActionList(ifFalse);
            return c;
        }



        public static Conditional CreateConditional(ConditionsChecker conditions, GameAction ifTrue, GameAction ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = conditions;
            c.IfTrue = CreateActionList(ifTrue);
            c.IfFalse = CreateActionList(ifFalse);
            return c;
        }

        public static ConditionsChecker CreateConditionsCheckerAnd(params Condition[] conditions)
        {
            return new ConditionsChecker() { Conditions = conditions, Operation = Operation.And };
        }

        public static ConditionsChecker CreateConditionsCheckerOr(params Condition[] conditions)
        {
            return new ConditionsChecker() { Conditions = conditions, Operation = Operation.Or };
        }

        public static ActionList CreateActionList(params GameAction[] actions)
        {
            if (actions == null || actions.Length == 1 && actions[0] == null) actions = Array.Empty<GameAction>();
            return new ActionList() { Actions = actions };
        }
        public static ContextActionSavingThrow CreateActionSavingThrow(this SavingThrowType savingThrow, params GameAction[] actions)
        {
            var c = Helpers.Create<ContextActionSavingThrow>();
            c.Type = savingThrow;
            c.Actions = CreateActionList(actions);
            return c;
        }

        public static ContextValue CreateContextValueRank(AbilityRankType value = AbilityRankType.Default) => value.CreateContextValue();

        public static ContextDurationValue CreateContextDuration(ContextValue bonus = null, DurationRate rate = DurationRate.Rounds, DiceType diceType = DiceType.Zero, ContextValue diceCount = null)
        {
            return new ContextDurationValue()
            {
                BonusValue = bonus ?? CreateContextValueRank(),
                Rate = rate,
                DiceCountValue = diceCount ?? 0,
                DiceType = diceType
            };
        }
        static public ContextActionApplyBuff createContextActionApplyBuff(BlueprintBuff buff, ContextDurationValue duration, bool is_from_spell = false,
                                                                                                                  bool is_child = false, bool is_permanent = false, bool dispellable = true,
                                                                                                                  int duration_seconds = 0)
        {
            var apply_buff = Helpers.Create<ContextActionApplyBuff>();
            apply_buff.IsFromSpell = is_from_spell;
            apply_buff.m_Buff = buff.ToReference<BlueprintBuffReference>();
            apply_buff.Permanent = is_permanent;
            apply_buff.DurationValue = duration;
            apply_buff.IsNotDispelable = !dispellable;
            apply_buff.UseDurationSeconds = duration_seconds > 0;
            apply_buff.DurationSeconds = duration_seconds;
            apply_buff.AsChild = is_child;
            apply_buff.ToCaster = false;
            return apply_buff;
        }
        static public AbilityCasterHasNoFacts createAbilityCasterHasNoFacts(params BlueprintUnitFact[] facts)
        {
            var a = Helpers.Create<AbilityCasterHasNoFacts>();
            var factsArray = new BlueprintUnitFactReference[facts.Count()];
            
            for(var i = 0; i < factsArray.Length; i++)
            {
                factsArray[i] = facts[i].ToReference<BlueprintUnitFactReference>();
            }
            a.m_Facts = factsArray;
            return a;
        }
        public static AbilityEffectRunAction CreateRunActions(params GameAction[] actions)
        {
            var result = Helpers.Create<AbilityEffectRunAction>();
            result.Actions = CreateActionList(actions);
            return result;
        }
        static public Kingmaker.Designers.Mechanics.Buffs.BuffStatusCondition createBuffStatusCondition(UnitCondition condition, SavingThrowType save_type = SavingThrowType.Unknown,
                                                                                                           bool save_each_round = true)
        {
            var c = Helpers.Create<Kingmaker.Designers.Mechanics.Buffs.BuffStatusCondition>();
            c.SaveType = save_type;
            c.SaveEachRound = save_each_round;
            c.Condition = condition;
            return c;
        }
        public static void SetFixedResource(this BlueprintAbilityResource resource, int baseValue)
        {
            BlueprintAbilityResource.Amount amount;
            if (resource.m_MaxAmount.BaseValue <= 0)
                amount = new BlueprintAbilityResource.Amount();
            else
                amount = resource.m_MaxAmount;


            amount.BaseValue = baseValue;

            // Enusre arrays are at least initialized to empty.
            var emptyClasses = Array.Empty<BlueprintCharacterClassReference>();
            var emptyArchetypes = Array.Empty<BlueprintArchetypeReference>();
            var field = "m_Class";
            if (Helpers.GetField(amount, field) == null) amount.m_Class = Array.Empty<BlueprintCharacterClassReference>();
            field = "m_ClassDiv";
            if (Helpers.GetField(amount, field) == null) amount.m_ClassDiv = emptyClasses;
            field = "m_Archetypes";
            if (Helpers.GetField(amount, field) == null) amount.m_Archetypes = emptyArchetypes;
            field = "m_ArchetypesDiv";
            if (Helpers.GetField(amount, field) == null) amount.m_ArchetypesDiv = emptyArchetypes;

            resource.m_MaxAmount = amount;
        }
        public static void SetIncreasedByLevelStartPlusDivStep(this BlueprintAbilityResource resource, int baseValue,
            int startingLevel, int startingIncrease, int levelStep, int perStepIncrease, int minClassLevelIncrease, float otherClassesModifier,
            BlueprintCharacterClass[] classes, BlueprintArchetype[] archetypes = null)
        {
            var amount = new BlueprintAbilityResource.Amount();
            var classRef = new BlueprintCharacterClassReference[classes.Count()];
            for (var i = 0; i < classRef.Length; i++)
            {
                classRef[i] = classes[i].ToReference<BlueprintCharacterClassReference>();
            }
            amount.BaseValue = baseValue;
            amount.IncreasedByLevelStartPlusDivStep = true;
            amount.StartingLevel = startingLevel;
            amount.StartingIncrease = startingIncrease;
            amount.LevelStep = levelStep;
            amount.PerStepIncrease = perStepIncrease;
            amount.MinClassLevelIncrease = minClassLevelIncrease;
            amount.OtherClassesModifier = otherClassesModifier;
            amount.IncreasedByLevel = false;
            amount.LevelIncrease = 0;
            amount.m_ClassDiv = classRef;
            var archRefs = Array.Empty<BlueprintArchetypeReference>();
            var emptyArchetypes = Array.Empty<BlueprintArchetypeReference>();
            if (archetypes != null)
            {
                archRefs = new BlueprintArchetypeReference[archetypes.Count()];
                for (var i = 0; i < archRefs.Length; i++)
                {
                    archRefs[i] = archetypes[i].ToReference<BlueprintArchetypeReference>();
                }
                
            }
            amount.m_ArchetypesDiv = archRefs ?? emptyArchetypes;
 
            // Enusre arrays are at least initialized to empty.
            if(amount.m_Class == null)
            {
                amount.m_Class = Array.Empty<BlueprintCharacterClassReference>();
            }
            if (amount.m_Archetypes == null)
            {
                amount.m_Archetypes = emptyArchetypes;
            }
  
            resource.m_MaxAmount = amount;
        }
        public static BlueprintAbilityResource CreateAbilityResource(string name, string displayName, string description, string guid, UnityEngine.Sprite icon,
            params BlueprintComponent[] components)
        {
            var resource = Helpers.CreateBlueprint<BlueprintAbilityResource>(name);
            resource.SetComponents(components);
            resource.LocalizedName = Main.MakeLocalizedString(name + ".Name", displayName);
            resource.LocalizedDescription = Main.MakeLocalizedString(name + ".Name", description);
            resource.m_Icon = icon;
            return resource;
        }

        static public ClassLevelsForPrerequisites createClassLevelsForPrerequisites(BlueprintCharacterClass fake_class, BlueprintCharacterClass actual_class, double modifier = 1.0, int summand = 0)
        {
            var c = Helpers.Create<ClassLevelsForPrerequisites>();
            c.m_ActualClass = actual_class.ToReference<BlueprintCharacterClassReference>();
            c.m_FakeClass = fake_class.ToReference<BlueprintCharacterClassReference>();
            c.Modifier = modifier;
            c.Summand = summand;
            return c;
        }

        public static AddContextStatBonus CreateAddContextStatBonus(StatType stat, ModifierDescriptor descriptor, ContextValueType type = ContextValueType.Rank, AbilityRankType rankType = AbilityRankType.Default, int multiplier = 1)
        {
            var addStat = Helpers.Create<AddContextStatBonus>();
            addStat.Stat = stat;
            addStat.Value = new ContextValue() { ValueType = type };
            addStat.Descriptor = descriptor;
            addStat.Value.ValueRank = rankType;
            addStat.Multiplier = multiplier;
            return addStat;
        }

        public static PrerequisiteNoFeature PrerequisiteNoFeature(this BlueprintFeature feat, bool any = false)
        {
            var result = Helpers.Create<PrerequisiteNoFeature>();
            result.m_Feature = feat.ToReference<BlueprintFeatureReference>();
            result.Group = any ? Prerequisite.GroupType.Any : Prerequisite.GroupType.All;

            if(result.m_Feature != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        static public string getNumExtension(int i)
        {
            if (i == 1)
            {
                return "st";
            }
            else if (i == 2)
            {
                return "nd";
            }
            else if (i == 3)
            {
                return "rd";
            }
            else
            {
                return "th";
            }
        }
        public static BlueprintFeatureSelection CreateFeatureSelection( string name, string displayName,
    string description, UnityEngine.Sprite icon, FeatureGroup group, params BlueprintComponent[] components)
        {
            var feat = Helpers.CreateBlueprint<BlueprintFeatureSelection>(name);
            SetFeatureInfo(feat, displayName, description, icon, group, components);
            feat.Group = group;
            return feat;
        }

        public static BlueprintFeature CreateFeature(string name, string displayName, string description, UnityEngine.Sprite icon,
            FeatureGroup group, params BlueprintComponent[] components)
        {
            var feat = Helpers.CreateBlueprint<BlueprintFeature>(name);
            SetFeatureInfo(feat, displayName, description, icon, group, components);
            feat.Ranks = 1;
            feat.IsClassFeature = true;
            return feat;
        }

        public static BlueprintProgression CreateProgression(string name, string displayName, string description, UnityEngine.Sprite icon,
            FeatureGroup group, params BlueprintComponent[] components)
        {
            var feat = Helpers.CreateBlueprint<BlueprintProgression>(name);
            SetFeatureInfo(feat, displayName, description, icon, group, components);
            feat.m_UIDeterminatorsGroup = Array.Empty<BlueprintFeatureBaseReference>();
            feat.UIGroups = Array.Empty<UIGroup>();
            feat.m_Classes = Array.Empty<BlueprintProgression.ClassWithLevel>();
            feat.m_Archetypes = Array.Empty<BlueprintProgression.ArchetypeWithLevel>();
            return feat;
        }


        public static void SetFeatureInfo(BlueprintFeature feat, string displayName, string description, UnityEngine.Sprite icon,
            FeatureGroup group, params BlueprintComponent[] components)
        {
            feat.SetComponents(components);
            feat.Groups = new FeatureGroup[] { group };
            feat.SetNameDescription(displayName, description);
            feat.m_Icon = icon;


        }

    }
}
