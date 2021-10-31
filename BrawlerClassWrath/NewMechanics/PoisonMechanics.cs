using System;
using BrawlerClassWrath.Utilities;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers.Units;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;

namespace BrawlerClassWrath.NewMechanics
{
    [ComponentName("BuffMechanics/Add Random Stat Penalty or Damage")]
    [AllowedOn(typeof(BlueprintBuff), false)]
    [AllowMultipleComponents]
    [TypeId("18b350e0e1634c1f8e9328ff12fd723e")]
    public class BuffPoisonDamage : UnitBuffComponentDelegate<BuffPoisonStatDamageData>, ITickEachRound, IPoisonStack
    {
        // Token: 0x0600C42F RID: 50223 RVA: 0x00314120 File Offset: 0x00312320
        void ITickEachRound.OnNewRound()
        {
            if (this.Ticks > base.Data.TicksPassed && this.SuccesfullSaves > base.Data.SavesSucceeded)
            {
                BuffPoisonStatDamageData data = base.Data;
                ContextDiceValue contextValue;
                if ((contextValue = base.Data.ContextValue) == null)
                {
                    ContextDiceValue contextDiceValue = new ContextDiceValue();
                    contextDiceValue.BonusValue = this.Bonus;
                    contextDiceValue.DiceCountValue = this.Value.Rolls;
                    contextValue = contextDiceValue;
                    contextDiceValue.DiceType = this.Value.Dice;
                }
                data.ContextValue = contextValue;
                int bonus = base.Data.ContextValue.Calculate(base.Context);
                ModifiableValue stat = base.Owner.Stats.GetStat(this.Stat);
                if (this.SaveType != SavingThrowType.Unknown)
                {
                    RuleSavingThrow ruleSavingThrow = new RuleSavingThrow(base.Owner, this.SaveType, base.Context.Params.DC + base.Data.BonusDC)
                    {
                        Reason = base.Fact
                    };
                    Game.Instance.Rulebook.TriggerEvent<RuleSavingThrow>(ruleSavingThrow);
                    if (!ruleSavingThrow.IsPassed)
                    {
                        TriggerEffect();
                    }
                    if (ruleSavingThrow.IsPassed)
                    {
                        (Buff as IFactContextOwner).RunActionInContext(on_successful_save_action, Owner);
                        base.Data.SavesSucceeded++;
                        if (base.Data.SavesSucceeded >= this.SuccesfullSaves)
                        {
                            base.Buff.Remove();
                        }
                    }
                }
                base.Data.TicksPassed++;
                return;
            }
            base.Buff.Remove();
        }

        public override void OnActivate()
        {
            //BuffPoisonStatDamageData data = base.Data;
            //ContextDiceValue contextValue;
            //if ((contextValue = base.Data.ContextValue) == null)
            //{
            //    contextValue = ContextValue;
            //}
            //data.ContextValue = contextValue;
            //int bonus = base.Data.ContextValue.Calculate(base.Context);
            //if (base.Owner.Stats.GetStat(this.Stat) != null && Stat.)
            //{
            //    RuleDealStatDamage rule = new RuleDealStatDamage(base.Context.MaybeCaster, base.Owner, this.Stat, DiceFormula.Zero, bonus)
            //    {
            //        Reason = base.Fact
            //    };
            //    base.Context.TriggerRule<RuleDealStatDamage>(rule);
            //}
            //base.Data.TicksPassed++;
            SuccesfullSaves = contextSuccesfullSaves.Calculate(Fact.MaybeContext);
            Ticks = contextTicks.Calculate(Fact.MaybeContext);
            TriggerEffect();
        }


        public void TriggerEffect()
        {
            BuffPoisonStatDamageData data = base.Data;
            ContextDiceValue contextValue;
            if ((contextValue = base.Data.ContextValue) == null)
            {
                contextValue = ContextValue;
            }
            data.ContextValue = contextValue;
            int bonus = base.Data.ContextValue.Calculate(base.Context);
            if (base.Owner.Stats.GetStat(this.Stat) != null && Stat != StatType.Unknown)
            {
                RuleDealStatDamage rule = new RuleDealStatDamage(base.Context.MaybeCaster, base.Owner, this.Stat, DiceFormula.Zero, bonus)
                {
                    Reason = base.Fact
                };
                base.Context.TriggerRule<RuleDealStatDamage>(rule);
            }
            else
            {
                var hp_damage = new DamageBundle(new BaseDamage[1]
                                                {
                                                    (BaseDamage) new DirectDamage(new DiceFormula(0, DiceType.Zero), bonus)
                                                }
                                                );
                RuleDealDamage newRule = new RuleDealDamage(Context.MaybeCaster, Owner, hp_damage)
                {
                    Reason = Fact
                };
                 Context.TriggerRule(newRule);
            }
            base.Data.TicksPassed++;
        }

        private void GetStacked()
        {
            base.Data.TicksPassed = base.Data.TicksPassed - this.Ticks / 2;
            base.Data.SavesSucceeded = Math.Max(0, base.Data.SavesSucceeded - 1);
            base.Data.BonusDC += 2;
        }

        void IPoisonStack.Stack()
        {
            this.GetStacked();
        }

        public ModifierDescriptor Descriptor;

        public StatType Stat = StatType.Unknown; // we're doing HP damage by default

        public DiceFormula Value;

        public ContextDiceValue ContextValue;

        public int Bonus;

        public int Ticks;

        public int SuccesfullSaves;

        public SavingThrowType SaveType;

        public ActionList on_successful_save_action = Helpers.CreateActionList();

        public ContextValue contextTicks;
        public ContextValue contextSuccesfullSaves;
    }
}
