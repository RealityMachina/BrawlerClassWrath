using BrawlerClassWrath.Extensions;
using BrawlerClassWrath.Utilities;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Area;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using Kingmaker.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrawlerClassWrath.NewMechanics
{
    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [AllowMultipleComponents]
    [TypeId("ac7d45194cb349c787c8aa6119b1db43")]
    public class TerrainMastery : UnitFactComponentDelegate, ITeleportHandler, IGlobalSubscriber, ISubscriber
    {
        // Token: 0x17001948 RID: 6472
        // (get) Token: 0x0600A141 RID: 41281 RVA: 0x00298F3F File Offset: 0x0029713F
        public bool CurrentAreaPartIsFavoredTerrain
        {
            get
            {
                return Owner.Get<UnitPartFavoredTerrain>().CurrentAreaPartIsFavoredTerrain;
            }
        }

        // Token: 0x0600A142 RID: 41282 RVA: 0x00298F53 File Offset: 0x00297153
        public override void OnTurnOn()
        {
            base.OnTurnOn();
            this.UpdateModifiers();
        }

        // Token: 0x0600A143 RID: 41283 RVA: 0x00298F7D File Offset: 0x0029717D
        public override void OnTurnOff()
        {
            base.OnTurnOff();
            this.DeactivateModifier();
        }

        // Token: 0x0600A144 RID: 41284 RVA: 0x00298FA1 File Offset: 0x002971A1
        private void UpdateModifiers()
        {
            if (this.CurrentAreaPartIsFavoredTerrain)
            {
                this.CheckEligibility();
                return;
            }
            this.DeactivateModifier();
        }

        private int GetRank()
        {
            int i = 10;

            i = Math.Max(10, 10 * Fact.GetRank());
            return i;
        }
        private void CheckEligibility()
        {

            if (!Owner.Body.IsPolymorphed) // can't be polymorphed
            {

                bool armor_ok = false;
                bool load_ok = false;

                var load = EncumbranceHelper.GetCarryingCapacity(Owner.Descriptor).GetEncumbrance();
                
                if(load <= maxLoad)
                {
                    load_ok = true;
                }
                var body_armor = this.Owner.Body?.Armor?.MaybeArmor;
                armor_ok = body_armor != null && required_armor.Contains(body_armor.Blueprint.ProficiencyGroup);
                armor_ok = armor_ok || (body_armor == null && required_armor.Contains(ArmorProficiencyGroup.None));
                if (!armor_ok) // armor doesn't meet requirements but does shield?
                {
                    var shield = this.Owner.Body?.SecondaryHand?.MaybeShield?.ArmorComponent;
                    armor_ok = shield != null && required_armor.Contains(shield.Blueprint.ProficiencyGroup);
                }

                if (armor_ok && load_ok) // requirements met...
                {


                    ActivateModifier();
                    return;

                }

            }
        }


        // Token: 0x0600A145 RID: 41285 RVA: 0x00298FB8 File Offset: 0x002971B8
        private void ActivateModifier()
        {
            int value = GetRank();
            base.Owner.Stats.Speed.AddModifierUnique(value, base.Runtime, ModifierDescriptor.Enhancement);

        }

        // Token: 0x0600A146 RID: 41286 RVA: 0x00299048 File Offset: 0x00297248
        private void DeactivateModifier()
        {
            base.Owner.Stats.Speed.RemoveModifiersFrom(base.Runtime);
        }

        // Token: 0x0600A147 RID: 41287 RVA: 0x002990C1 File Offset: 0x002972C1
        public void HandlePartyTeleport(AreaEnterPoint enterPoint)
        {
            this.UpdateModifiers();
        }


        public ArmorProficiencyGroup[] required_armor = new ArmorProficiencyGroup[0];

        public Encumbrance maxLoad = Encumbrance.Medium;
    }


    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [AllowMultipleComponents]
    [TypeId("6289f4fe3d6d414182ac383eb2cda9fe")]
    public class FavoredTurf : UnitFactComponentDelegate, ITeleportHandler, IGlobalSubscriber, ISubscriber
    {
        // Token: 0x17001948 RID: 6472
        // (get) Token: 0x0600A141 RID: 41281 RVA: 0x00298F3F File Offset: 0x0029713F
        public bool CurrentAreaPartIsFavoredTerrain
        {
            get
            {
                return AreaService.Instance.CurrentAreaSetting == this.Setting;
            }
        }

        // Token: 0x0600A142 RID: 41282 RVA: 0x00298F53 File Offset: 0x00297153
        public override void OnTurnOn()
        {
            base.OnTurnOn();
            base.Owner.Ensure<UnitPartFavoredTerrain>().AddEntry(this.Setting, base.Fact);
            this.UpdateModifiers();
        }

        // Token: 0x0600A143 RID: 41283 RVA: 0x00298F7D File Offset: 0x0029717D
        public override void OnTurnOff()
        {
            base.OnTurnOff();
            base.Owner.Ensure<UnitPartFavoredTerrain>().RemoveEntry(base.Fact);
            this.DeactivateModifier();
        }

        // Token: 0x0600A144 RID: 41284 RVA: 0x00298FA1 File Offset: 0x002971A1
        private void UpdateModifiers()
        {
            if (this.CurrentAreaPartIsFavoredTerrain)
            {
                this.ActivateModifier();
                return;
            }
            this.DeactivateModifier();
        }

        private int GetRank()
        {
            int i = 1;
            foreach (Feature feature in base.Owner.Progression.Features) // this SHOULDN'T include feature selections
            {
                if (feature.Blueprint.name != Fact.Blueprint.name && feature.Blueprint.Groups.Contains(FeatureGroup.FavoriteTerrain))
                {

                    i++;
                }
            }
            return i;
        }
        // Token: 0x0600A145 RID: 41285 RVA: 0x00298FB8 File Offset: 0x002971B8
        private void ActivateModifier()
        {
            int value = GetRank();
            int Bonusvalue = 2 * GetRank();
            base.Owner.Stats.Initiative.AddModifierUnique(Bonusvalue, base.Runtime, ModifierDescriptor.UntypedStackable);
            base.Owner.Stats.AdditionalCMB.AddModifierUnique(value, base.Runtime, ModifierDescriptor.UntypedStackable);
            base.Owner.Stats.AdditionalCMD.AddModifierUnique(value, base.Runtime, ModifierDescriptor.UntypedStackable);
        }

        // Token: 0x0600A146 RID: 41286 RVA: 0x00299048 File Offset: 0x00297248
        private void DeactivateModifier()
        {
            base.Owner.Stats.Initiative.RemoveModifiersFrom(base.Runtime);
            base.Owner.Stats.AdditionalCMB.RemoveModifiersFrom(base.Runtime);
            base.Owner.Stats.AdditionalCMD.RemoveModifiersFrom(base.Runtime);
        }

        // Token: 0x0600A147 RID: 41287 RVA: 0x002990C1 File Offset: 0x002972C1
        public void HandlePartyTeleport(AreaEnterPoint enterPoint)
        {
            this.UpdateModifiers();
        }

        // Token: 0x04006C50 RID: 27728
        public AreaSetting Setting;
    }

    [ComponentName("FactMechanics/Add caster level for buff")]
    [AllowMultipleComponents]
    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [TypeId("df54612927934178a42d5dbc3372cede")]
    public class AddCasterLevelForBuff : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAbilityParams>, IRulebookHandler<RuleCalculateAbilityParams>, ISubscriber, IInitiatorRulebookSubscriber
    {
        // Token: 0x17001FAC RID: 8108
        // (get) Token: 0x0600BE38 RID: 48696 RVA: 0x0030277F File Offset: 0x0030097F
        public BlueprintBuff Spell
        {
            get
            {
                BlueprintBuffReference spell = this.m_Spell;
                if (spell == null)
                {
                    return null;
                }
                return spell.Get();
            }
        }

        // Token: 0x0600BE39 RID: 48697 RVA: 0x00302792 File Offset: 0x00300992
        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {
            if(evt.Blueprint as BlueprintBuff != null)
            {
                if (evt.Blueprint as BlueprintBuff == Spell)
                {
                    evt.AddBonusCasterLevel(this.Bonus, this.Descriptor);
                }
            }

        }

        // Token: 0x0600BE3A RID: 48698 RVA: 0x00003AE3 File Offset: 0x00001CE3
        public void OnEventDidTrigger(RuleCalculateAbilityParams evt)
        {
        }

        // Token: 0x04007CA1 RID: 31905
        [SerializeField]
        [FormerlySerializedAs("Spell")]
        public BlueprintBuffReference m_Spell;

        // Token: 0x04007CA2 RID: 31906
        public int Bonus;

        // Token: 0x04007CA3 RID: 31907
        public ModifierDescriptor Descriptor = ModifierDescriptor.UntypedStackable;
    }

    [TypeId("d05e64da5b1e4fcaaabcd134ed868257")]
    public class IncreaseResourcesByClassWithArchetype : UnitFactComponentDelegate, IResourceAmountBonusHandler, IUnitSubscriber, ISubscriber
    {
        public BlueprintAbilityResource Resource;
        public BlueprintCharacterClass CharacterClass;
        public BlueprintArchetype Archetype = null;
        public int base_value = 0;

        public void CalculateMaxResourceAmount(BlueprintAbilityResource resource, ref int bonus)
        {
            if (!this.Fact.Active || (resource != this.Resource))
                return;
            int classLevel = this.Owner.Progression.GetClassLevel(this.CharacterClass);
            if (Archetype == null || this.Owner.Progression.IsArchetype(Archetype))
            {
                bonus += classLevel + base_value;
            }
        }
    }

    [TypeId("b4951c6e7af543ecbe4bde7d75d5bcab")]
    public class ContextActionCasterSkillCheck : ContextAction
    {
        public StatType Stat;
        public bool UseCustomDC;//if false will be used against main target hd
        public ContextValue CustomDC = 0;
        public ActionList Success = Helpers.CreateActionList();
        public ActionList Failure = Helpers.CreateActionList();
        public ContextValue bonus = 0;

        public override void RunAction()
        {
            if (this.Context.MaybeCaster == null)
            {
                Main.Log("Caster unit is missing.");
            }
            else
            {
                int num = bonus.Calculate(this.Context);
                var rule_skill_check = new RuleSkillCheck(this.Context.MaybeCaster, this.Stat, this.UseCustomDC ? this.CustomDC.Calculate(this.Context) : ((this.Target.Unit?.Descriptor?.Progression.CharacterLevel).GetValueOrDefault() + 10));
                rule_skill_check.ShowAnyway = true;
                if (num != 0)
                {
                    rule_skill_check.Bonus.AddModifier(num, ModifierDescriptor.UntypedStackable);
                }
                if (this.Context.TriggerRule<RuleSkillCheck>(rule_skill_check).Success)
                    this.Success.Run();
                else
                    this.Failure.Run();
            }
        }

        public override string GetCaption()
        {
            return string.Format("Caster skill check {0} {1}", (object)this.Stat, this.UseCustomDC ? (object)string.Format("(DC: {0})", (object)this.CustomDC) : (object)"");
        }
    }

    [AllowMultipleComponents]
    [ComponentName("Saving throw bonus against fact from caster")]
    [AllowedOn(typeof(BlueprintUnitFact))]
    [TypeId("2c5f80fd923145e29aa0ca30d6d4661d")]
    public class SavingThrowBonusAgainstFactFromCaster : UnitFactComponentDelegate, IGlobalRulebookHandler<RuleSavingThrow>, IRulebookHandler<RuleSavingThrow>, ISubscriber, IGlobalRulebookSubscriber
    {
        public BlueprintUnitFact CheckedFact;
        public ModifierDescriptor Descriptor;
        public ContextValue Value;
        public bool will = true;
        public bool reflex = true;
        public bool fortitude = true;

        public void OnEventAboutToTrigger(RuleSavingThrow evt)
        {
            UnitDescriptor descriptor = evt.Reason.Caster?.Descriptor;
            if (descriptor == null)
                return;
            int bonus = Value.Calculate(this.Fact.MaybeContext);
            foreach (var b in descriptor.Buffs)
            {
                if (b.Blueprint == CheckedFact && b.Context.MaybeCaster == evt.Initiator)
                {
                    if (will)
                    {
                        evt.AddTemporaryModifier(evt.Initiator.Stats.SaveWill.AddModifier(bonus, Fact, this.Descriptor));
                    }
                    if (reflex)
                    {
                        evt.AddTemporaryModifier(evt.Initiator.Stats.SaveReflex.AddModifier(bonus, Fact, this.Descriptor));
                    }
                    if (fortitude)
                    {
                        evt.AddTemporaryModifier(evt.Initiator.Stats.SaveFortitude.AddModifier(bonus, Fact, this.Descriptor));
                    }
                    return;
                }
            }

        }

        public void OnEventDidTrigger(RuleSavingThrow evt)
        {
        }
    }

    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [AllowMultipleComponents]
    [TypeId("0d2ae35019c84d03a26f6e04e84d5dd4")]
    public class OpportunistMultipleAttacks : UnitFactComponentDelegate<OpportunistData>, IGlobalRulebookHandler<RuleDealDamage>, IRulebookHandler<RuleDealDamage>, ISubscriber, IGlobalRulebookSubscriber
    {
        [SerializeField]
        public int extra_attacks_used = 0;

        [SerializeField]
        public ContextValue num_extra_attacks;

        public void OnEventAboutToTrigger(RuleDealDamage evt)
        {

        }

        public void OnEventDidTrigger(RuleDealDamage evt)
        {
            ItemEntityWeapon weapon = evt.DamageBundle.Weapon;
            if (evt.Initiator == this.Owner || weapon == null || (!weapon.Blueprint.IsMelee || !this.Owner.CombatState.EngagedUnits.Contains(evt.Target)))
                return;
            int max_extra_attacks = num_extra_attacks.Calculate(this.Fact.MaybeContext);
            if (Data.LastUseTime + 1.Rounds().Seconds > Kingmaker.Game.Instance.TimeController.GameTime)
            {
                if (extra_attacks_used < max_extra_attacks)
                {
                    extra_attacks_used++;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Data.LastUseTime = Kingmaker.Game.Instance.TimeController.GameTime;
                extra_attacks_used = 0;
            }

            Kingmaker.Game.Instance.CombatEngagementController.ForceAttackOfOpportunity(this.Owner, evt.Target);
        }
    }


    [TypeId("913ff1e0bfe64103bfd8804550f77498")]
    public class ContextActionRemoveBuffFromCaster : ContextAction
    {
        public BlueprintBuff Buff;
        public int remove_delay_seconds = 0;

        public override string GetCaption()
        {
            return "Remove Buff From Caster: " + this.Buff.Name;
        }

        public override void RunAction()
        {
            MechanicsContext context = Context;
            if (context == null)
                return;
            UnitEntityData maybeCaster = this.Context.MaybeCaster;
            foreach (var b in this.Target.Unit.Buffs)
            {
                if (b.Blueprint == Buff && b.Context.MaybeCaster == maybeCaster)
                {
                    if (remove_delay_seconds > 0)
                        b.RemoveAfterDelay(new TimeSpan(0, 0, remove_delay_seconds));
                    else
                        b.Remove();
                }
            }
        }
    }


    [TypeId("f35d232b7a6840ed8052b3e7d0dfc389")]
    public class ContextActionRemoveBuffs : ContextAction
    {
        [SerializeField]
        [FormerlySerializedAs("TargetBuffs")]
        public BlueprintBuff[] Buffs;

        public bool ToCaster;

        public override string GetCaption()
        {
            return "Remove Buffs";
        }

        public override void RunAction()
        {

                foreach (var b in Buffs)
                {
                      Buff buff = null;
                    if (!ToCaster)
                    {
                        buff = base.Target.Unit.Buffs.GetBuff(b);
                    }
                    else
                    {
                         buff = Context.MaybeCaster?.Buffs.GetBuff(b);
                     }
                    if(buff != null)
                    {
                        buff.Remove();
                    }
                    
                }
            
        }
    }


    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [AllowMultipleComponents]
    [TypeId("0447ada95b824716a633b5027cf3b7b1")]
    public class ContextWeaponDamageDiceReplacementWeaponCategory : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateWeaponStats>, IRulebookHandler<RuleCalculateWeaponStats>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public WeaponCategory[] categories;
        public DiceFormula[] dice_formulas;
        public ContextValue value;


        public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            if (!categories.Contains(evt.Weapon.Blueprint.Category))
            {
                return;
            }

            int dice_id = value.Calculate(this.Context);
            if (dice_id < 0)
            {
                dice_id = 0;
            }
            if (dice_id >= dice_formulas.Length)
            {
                dice_id = dice_formulas.Length - 1;
            }

            var wielder_size = evt.Initiator.Descriptor.State.Size;
            //scale weapon to the wielder size if need (note polymorphs do not change their size, so their weapon dice is not supposed to scale)
            var base_damage = evt.WeaponDamageDiceOverride.HasValue ? evt.WeaponDamageDiceOverride.Value : evt.Weapon.Blueprint.BaseDamage;
            var base_dice = evt.Initiator.Body.IsPolymorphed ? base_damage : WeaponDamageScaleTable.Scale(base_damage, wielder_size);

            var new_dice = WeaponDamageScaleTable.Scale(dice_formulas[dice_id], wielder_size);

            var new_dmg_avg = new_dice.MinValue(0) + new_dice.MaxValue(0);
            int current_dmg_avg = (base_dice.MaxValue(0) + base_dice.MinValue(0));
            if (new_dmg_avg > current_dmg_avg)
            {
                evt.WeaponDamageDiceOverride = dice_formulas[dice_id];
            }
        }

        public void OnEventDidTrigger(RuleCalculateWeaponStats evt)
        {

        }
    }


    [TypeId("cdcbf53886114eaf95aa5823aa605593")]
    public class ContextCalculateAbilityParamsBasedOnClasses : ContextAbilityParamsCalculator
    {
        public bool use_kineticist_main_stat;
        public StatType StatType = StatType.Charisma;
        public BlueprintCharacterClass[] CharacterClasses = new BlueprintCharacterClass[0];
        public BlueprintArchetype[] archetypes = null;
        public BlueprintUnitProperty property = null;

        public override AbilityParams Calculate(MechanicsContext context)
        {
            UnitEntityData maybeCaster = context?.MaybeCaster;
            if (maybeCaster == null)
            {

                return context?.Params;
            }
            StatType statType = this.StatType;
            if (this.use_kineticist_main_stat)
            {
                UnitPartKineticist unitPartKineticist = context.MaybeCaster?.Get<UnitPartKineticist>();
               StatType? mainStatType = unitPartKineticist?.MainStatType;
                statType = !mainStatType.HasValue ? this.StatType : mainStatType.Value;
            }
     
            var stat_property_getter = property?.GetComponent<StatPropertyValueGetter>();
            if (stat_property_getter != null)
            {

                statType = stat_property_getter.GetStat(maybeCaster);
     
            }

            AbilityData ability = context.SourceAbilityContext?.Ability;
            RuleCalculateAbilityParams rule = !(ability != (AbilityData)null) ? new RuleCalculateAbilityParams(maybeCaster, context.AssociatedBlueprint, (Spellbook)null) : new RuleCalculateAbilityParams(maybeCaster, ability);
            rule.ReplaceStat = new StatType?(statType);

            int class_level = 0;
            foreach (var c in this.CharacterClasses)
            {

                var class_archetypes = archetypes?.Where(a => a.GetParentClass() == c);

                if (class_archetypes == null || class_archetypes.Any(a => maybeCaster.Descriptor.Progression.IsArchetype(a)))
                {

                    class_level += maybeCaster.Descriptor.Progression.GetClassLevel(c);
                }

            }

            rule.ReplaceCasterLevel = new int?(class_level);
            rule.ReplaceSpellLevel = new int?(class_level / 2);
            return context.TriggerRule<RuleCalculateAbilityParams>(rule).Result;
        }

        public override void ApplyValidation(ValidationContext context)
        {
            base.ApplyValidation(context);
            if (this.StatType.IsAttribute() || this.StatType == StatType.BaseAttackBonus)
                return;
            string str = string.Join(", ", ((IEnumerable<StatType>)StatTypeHelper.Attributes).Select<StatType, string>((Func<StatType, string>)(s => s.ToString())));
            context.AddError("StatType must be Base Attack Bonus or an attribute: {0}", (object)str);
        }
    }


    [AllowedOn(typeof(BlueprintUnitProperty), false)]
    [AllowMultipleComponents]
    [TypeId("9db0e32620524087b65fd4f65614804f")]
    class StatPropertyValueGetter : PropertyValueGetter
    {
        public override int GetBaseValue(UnitEntityData unit)
        {
            return 0;
        }

        public virtual StatType GetStat(UnitEntityData unit)
        {
            return StatType.Charisma;
        }
    }

    [AllowedOn(typeof(BlueprintUnitProperty), false)]
    [AllowMultipleComponents]
    [TypeId("1b169c5395314b50955ecb14e383bb0d")]
    class HighestStatPropertyGetter : StatPropertyValueGetter
    {
        public StatType[] stats;
        public static BlueprintUnitProperty createProperty(string name, string guid, params StatType[] stats)
        {
            var p = Helpers.Create<BlueprintUnitProperty>();
            p.name = name;
            p.SetComponents(Helpers.Create<HighestStatPropertyGetter>(a => a.stats = stats));
            return p;
        }

        public override int GetBaseValue(UnitEntityData unit)
        {
            int val = -100;
            foreach (var s in stats)
            {
                int bonus = unit.Stats.GetStat<ModifiableValueAttributeStat>(s).Bonus;
                if (bonus > val)
                {

                    val = bonus;
                }
            }
            return val;
        }


        public override StatType GetStat(UnitEntityData unit)
        {
            int val = -100;
            var stat = StatType.Charisma;
            foreach (var s in stats)
            {
                int bonus = unit.Stats.GetStat<ModifiableValueAttributeStat>(s).Bonus;
                if (bonus > val)
                {

                    val = bonus;
                    stat = s;
                }
            }
            return stat;
        }
    }


    [TypeId("31bbab3364604b3b8374f6d9aac6377a")]
    public class ContextConditionHasFacts : ContextCondition
    {
        public BlueprintUnitFact[] Facts;
        public bool all = false;

        public override string GetConditionCaption()
        {
            return string.Empty;
        }

        public override bool CheckCondition()
        {
            foreach (var f in Facts)
            {
                if (this.Target.Unit.Descriptor.HasFact(f) && !all)
                {
                    return true;
                }
                else if (!this.Target.Unit.Descriptor.HasFact(f) && all)
                {
                    return false;
                }
            }
            return all;
        }
    }

    [AllowedOn(typeof(BlueprintAbility))]
    [AllowMultipleComponents]
    [TypeId("428eac4711f14fb183df04573b48ad6c")]
    public class AbilityCasterMainWeaponGroupCheck : BlueprintComponent, IAbilityCasterRestriction
    {
        public WeaponFighterGroup[] groups;
        public WeaponCategory[] extra_categories = new WeaponCategory[0];
        public bool is_2h = false;
        readonly BlueprintParametrizedFeature weapon_focus = Resources.GetBlueprint<BlueprintParametrizedFeature>("1e1f627d26ad36f43bbd26cc2bf8ac7e");

        public bool IsCasterRestrictionPassed(UnitEntityData caster)
        {
            if (!caster.Body.PrimaryHand.HasWeapon)
            {
                return false;
            }

            if (is_2h)
            {
                return caster.Body.PrimaryHand.Weapon.Blueprint.IsTwoHanded && caster.Body.PrimaryHand.Weapon.Blueprint.IsMelee;
            }

            foreach (var group in groups)
            {
                if(caster.Body.PrimaryHand.Weapon.Blueprint.Type.FighterGroup.Contains(group) || extra_categories.Contains(caster.Body.PrimaryHand.Weapon.Blueprint.Category))
                {
                    return true;
                }
            }
            return false;

        }

        public string GetAbilityCasterRestrictionUIText()
        {
            return LocalizedTexts.Instance.Reasons.SpecificWeaponRequired;
        }

   
    }

    [ComponentName("Add feature if owner has no armor or shield")]
    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [AllowMultipleComponents]
    [TypeId("0e7242853566456a90dfebc05bc166c6")]
    public class AddFeatureOnArmor : UnitFactComponentDelegate<HasArmorFeatureUnlockData>, IUnitActiveEquipmentSetHandler, IGlobalSubscriber, ISubscriber, IUnitEquipmentHandler
    {
        // Token: 0x17002091 RID: 8337
        // (get) Token: 0x0600C4AD RID: 50349 RVA: 0x00315BF7 File Offset: 0x00313DF7
        public BlueprintUnitFact NewFact
        {
            get
            {
                BlueprintUnitFactReference newFact = this.m_NewFact;
                if (newFact == null)
                {
                    return null;
                }
                return newFact.Get();
            }
        }

        // Token: 0x0600C4AE RID: 50350 RVA: 0x00315C0A File Offset: 0x00313E0A
        public override void OnActivate()
        {
            this.CheckEligibility();
        }

        // Token: 0x0600C4AF RID: 50351 RVA: 0x00315C12 File Offset: 0x00313E12
        public override void OnDeactivate()
        {
            this.RemoveFact();
        }

        // Token: 0x0600C4B0 RID: 50352 RVA: 0x00315C0A File Offset: 0x00313E0A
        public void HandleUnitChangeActiveEquipmentSet(UnitDescriptor unit)
        {
            this.CheckEligibility();
        }

        // Token: 0x0600C4B1 RID: 50353 RVA: 0x00315C1A File Offset: 0x00313E1A
        private void CheckEligibility()
        {

            if (!Owner.Body.IsPolymorphed) // can't be polymorphed
            {

                bool armor_ok = false;
                var body_armor = this.Owner.Body?.Armor?.MaybeArmor;
                armor_ok = body_armor != null && required_armor.Contains(body_armor.Blueprint.ProficiencyGroup);
                armor_ok = armor_ok || (body_armor == null && required_armor.Contains(ArmorProficiencyGroup.None));
                if (!armor_ok) // armor doesn't meet requirements but does shield?
                {
                    var shield = this.Owner.Body?.SecondaryHand?.MaybeShield?.ArmorComponent;
                    armor_ok = shield != null && required_armor.Contains(shield.Blueprint.ProficiencyGroup);
                }

                if (armor_ok) // requirements met...
                {
                 

                        this.AddFact();
                        return;
                    
                }

            }
 
            this.RemoveFact();
        }
                                                                                                    
        // Token: 0x0600C4B2 RID: 50354 RVA: 0x00315C40 File Offset: 0x00313E40
        private void AddFact()
        {
            if (base.Data.AppliedFact == null)
            {
                base.Data.AppliedFact = base.Owner.AddFact(this.NewFact, null, null);
            }
        }

        // Token: 0x0600C4B3 RID: 50355 RVA: 0x00315C6D File Offset: 0x00313E6D
        private void RemoveFact()
        {
            if (base.Data.AppliedFact != null)
            {
                base.Owner.RemoveFact(base.Data.AppliedFact);
                base.Data.AppliedFact = null;
            }
        }

        // Token: 0x0600C4B4 RID: 50356 RVA: 0x00315C9E File Offset: 0x00313E9E
        public void HandleEquipmentSlotUpdated(ItemSlot slot, ItemEntity previousItem)
        {
            if (slot.Owner != base.Owner)
            {
                return;
            }
            this.CheckEligibility();
        }

        // Token: 0x040080AC RID: 32940
        [SerializeField]
        [FormerlySerializedAs("NewFact")]
        public BlueprintUnitFactReference m_NewFact;

        public ArmorProficiencyGroup[] required_armor = new ArmorProficiencyGroup[0];
        public ArmorProficiencyGroup[] forbidden_armor = new ArmorProficiencyGroup[0];
    }
}
