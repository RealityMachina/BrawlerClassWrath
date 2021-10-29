using BrawlerClassWrath.Utilities;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
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
        public BlueprintArchetype[] archetypes = new BlueprintArchetype[0];
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
                var class_archetypes = archetypes.Where(a => a.GetParentClass() == c);

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
            return (groups.Contains(caster.Body.PrimaryHand.Weapon.Blueprint.Type.FighterGroup) || extra_categories.Contains(caster.Body.PrimaryHand.Weapon.Blueprint.Category));

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
                    if (base.Owner.Body.Armor.HasArmor) // care about forbidden armor if it comes up later
                    {
                        this.AddFact();
                        return;
                    }
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
