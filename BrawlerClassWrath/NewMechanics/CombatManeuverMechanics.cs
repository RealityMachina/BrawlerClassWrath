using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Controllers.Projectiles;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.ContextData;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlerClassWrath.NewMechanics
{
  

    namespace CombatManeuverMechanics
    {

        [AllowMultipleComponents]
        [ComponentName("Maneuver Bonus")]
        [AllowedOn(typeof(BlueprintUnitFact), false)]
        [TypeId("b73d3c480e70494b9fc244d4f180562f")]
        public class SpecificCombatManeuverBonus : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateCMB>, IRulebookHandler<RuleCalculateCMB>, ISubscriber, IInitiatorRulebookSubscriber, ITargetRulebookHandler<RuleCalculateCMD>, IRulebookHandler<RuleCalculateCMD>, ITargetRulebookSubscriber
        {
            public ContextValue Value;
            public CombatManeuver maneuver_type;

            private new MechanicsContext Context
            {
                get
                {
                    MechanicsContext context = (this.Fact as Buff)?.Context;
                    if (context != null)
                        return context;
                    return (this.Fact as Feature)?.Context;
                }
            }

            public void OnEventAboutToTrigger(RuleCalculateCMB evt)
            {
                if (evt.Type == maneuver_type)
                {
                    evt.AddModifier(this.Value.Calculate(this.Context), this.Fact);
                }
            }

            public void OnEventAboutToTrigger(RuleCalculateCMD evt)
            {
                if (evt.Type == maneuver_type)
                {
                    evt.AddModifier(this.Value.Calculate(this.Context), this.Fact);
                }
            }

            public void OnEventDidTrigger(RuleCalculateCMB evt)
            {
            }

            public void OnEventDidTrigger(RuleCalculateCMD evt)
            {
            }
        }

        [HarmonyPatch(typeof(RuleCombatManeuver))]
        [HarmonyPatch("ApplyManeuver", MethodType.Normal)]
        class RuleCombatManeuver__OnTrigger__Patch
        {
            //static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            //{
            //    var codes = instructions.ToList();
            //    var create_exception_string = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Ldstr && x.operand.ToString().Contains("Unsupported"));
            //    var ret = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Ret);

            //    codes.InsertRange(create_exception_string + 1, new CodeInstruction[]
            //        {
            //            //new CodeInstruction(System.Reflection.Emit.OpCodes.Ldarg_0),
            //            new CodeInstruction(System.Reflection.Emit.OpCodes.Call,
            //                                                               new Action<RuleCombatManeuver>(RuleCombatManeuver__OnTrigger__Patch.checkExtraManeuvers).Method
            //                                                               ),
            //           // new CodeInstruction(System.Reflection.Emit.OpCodes.Brfalse_S, codes[ret].labels[0])
            //        }
            //    );
            //    return codes.AsEnumerable();
            //}
            //this was a transpiler before, and then I had the realization it's a lot cleaner to just prefix ApplyManeuver()
            static bool Prefix(RuleCombatManeuver __instance)
            {
                switch (__instance.Type)
                {
                    case CombatManeuver.AwesomeBlow:
                        {
                            if (__instance.Target.CanBeKnockedOff())
                            {
                                __instance.Target.Descriptor.State.Prone.ShouldBeActive = true;
                                EventBus.RaiseEvent<IKnockOffHandler>(delegate (IKnockOffHandler h)
                                {
                                    h.HandleKnockOff(__instance.Initiator, __instance.Target);
                                }, true);
                                return false; // skip, we handled it from here
                            }
                        }
                        break;
                    default:
                        break;
                }
                return true; //otherwise pass through to original
            }
        }

        [TypeId("681af03600f041e3b22c640cdf4f9804")]
        public class ContextActionForceMove : ContextAction
        {
            public bool provoke_aoo = false;
            public ContextDiceValue distance_dice;

            public override string GetCaption()
            {
                return "Force Move";
            }

            public override void RunAction()
            {
                var caster = Context.MaybeCaster;

                var unit = Target.Unit;

                if (unit == null || caster == null)
                {
                    return;
                }
                var distance = distance_dice.Calculate(Context);
                unit.Ensure<UnitPartForceMove>().Push((unit.Position - caster.Position).normalized, distance.Feet().Meters, provoke_aoo);
            }
        }

        [TypeId("bd6aab87fef0410788fc1ff61f595739")]
        public class AbilityTargetNotBiggerUnlessFact : BlueprintComponent, IAbilityTargetRestriction
        {
            public BlueprintUnitFact fact;
            public bool Not;

            public string GetAbilityTargetRestrictionUIText(UnitEntityData caster, TargetWrapper target)
            {
                return "Target too big";
            }
            public bool IsTargetRestrictionPassed(UnitEntityData caster, TargetWrapper target)
            {
                var unit = target.Unit;
                if (unit == null || caster == null)
                {
                    return false;
                }

                return ((unit.Descriptor.State.Size <= caster.Descriptor.State.Size) != Not) || (fact != null && caster.Descriptor.HasFact(fact));
            }
        }


        //fix Success to return false in case of autofailure or immunity to combat maneuver
        [HarmonyPatch(typeof(RuleCombatManeuver))]
        [HarmonyPatch("Success", MethodType.Getter)]
        class RuleCombatManeuver__Success__Patch
        {
            static void Postfix(RuleCombatManeuver __instance, ref bool __result)
            {
                if (__instance.AutoFailure //already includes immunity to specific maneuvers 
                    || __instance.Target.Descriptor.State.HasCondition(UnitCondition.ImmuneToCombatManeuvers)
                    || (__instance.Target.Descriptor.State.HasConditionImmunity(UnitCondition.Prone)
                        && (__instance.Type == CombatManeuver.AwesomeBlow) //__instance.Type == CombatManeuver.Trip || 
                        )
                    )
                {
                    //only need to care about awesome blow
                    __result = false;
                }
            }
        }

        //REDUNDANT, new patch added awesome blow type
 /*       [HarmonyPatch(typeof(ContextActionCombatManeuver))]
        [HarmonyPatch("RunAction", MethodType.Normal)]
        class ContextActionCombatManeuver__RunAction__Patch
        {
            static bool Prefix(ContextActionCombatManeuver __instance)
            {
                var tr = Traverse.Create(__instance);
                var target = tr.Property("Target").GetValue<TargetWrapper>();
                var unit = target?.Unit;
                if (unit == null)
                {
                    return true;
                }

                if (__instance.Type == CombatManeuverTypeExtender.AwesomeBlow.ToCombatManeuverType()
                    && (unit.Descriptor.State.Prone.Active || unit.View.IsGetUp))
                {
                    return false;
                }

                return true;
            }
        }
 */


    }
}