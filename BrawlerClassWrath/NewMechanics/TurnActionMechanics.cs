using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

namespace BrawlerClassWrath.NewMechanics
{

    public class UseAbilitiesAsSwiftAction : SwiftActionAbilityUseBase
    {
        public BlueprintAbility[] abilities;

        public override bool canUseOnAbility(AbilityData ability, CommandType actual_action_type)
        {
            if (ability == null)
            {
                return false;
            }

            return abilities.HasItem(ability.Blueprint) || (ability.Blueprint.Parent == null ? false : abilities.HasItem(ability.Blueprint.Parent));
        }
    }


    public class UseAbilitiesAsFreeAction : FreeActionAbilityUseBase
    {
        public BlueprintAbility[] abilities;

        public override bool canUseOnAbility(AbilityData ability, CommandType actual_action_type)
        {
            if (ability == null)
            {
                return false;
            }

            return abilities.HasItem(ability.Blueprint) || (ability.Blueprint.Parent == null ? false : abilities.HasItem(ability.Blueprint.Parent));
        }
    }

    static public class Aux
    {
        static public void removePart(UnitPart part)
        {

            var owner = part.Owner;
            Type part_type = part.GetType();
            
            var remove_method = owner.GetType().GetMethod(nameof(UnitDescriptor.Remove));
            remove_method.MakeGenericMethod(part_type).Invoke(owner, null);
        }
    }


    public class AdditiveUnitPart : UnitPart
    {
        [JsonProperty]
        protected List<UnitFact> buffs = new List<UnitFact>();

        public virtual void addBuff(UnitFact buff)
        {
            if (!buffs.Contains(buff))
            {
                buffs.Add(buff);
            }
        }

        public virtual void removeBuff(UnitFact buff)
        {
            buffs.Remove(buff);
            if (buffs.Empty())
            {
                Aux.removePart(this);
            }
        }
    }
    public class AdditiveUnitPartWithCheckLock : AdditiveUnitPart
    {
        Dictionary<UnitFact, bool> lock_map = new Dictionary<UnitFact, bool>();

        public override void addBuff(UnitFact buff)
        {
            if (!buffs.Contains(buff))
            {
                buffs.Add(buff);
                lock_map[buff] = false;
            }
        }

        public override void removeBuff(UnitFact buff)
        {
            buffs.Remove(buff);
            lock_map.Remove(buff);
            if (buffs.Empty())
            {
                Aux.removePart(this);
            }
        }


        protected bool check<T>(UnitFact buff, Predicate<T> pred) where T : BlueprintComponent
        {
            if (!buffs.Contains(buff))
            {
                return false;
            }
            if (lock_map[buff])
            {
                return false;
            }
            lock_map[buff] = true;

            bool res = false;
            buff.CallComponents<T>(c => res = pred(c));
            lock_map[buff] = false;
            return res;
        }
    }
    public class UnitPartFreeAbilityUse : AdditiveUnitPartWithCheckLock
    {
        public bool canBeUsedOnAbility(AbilityData ability, CommandType actual_action_type)
        {
            if (buffs.Empty())
            {
                return false;
            }

            foreach (var b in buffs)
            {
                bool result = check<FreeActionAbilityUseBase>(b, c => c.canUseOnAbility(ability, actual_action_type));
                if (!result)
                {
                    var sticky_touch = ability?.StickyTouch;
                    if (sticky_touch != null)
                    {
                        result = check<FreeActionAbilityUseBase>(b, c => c.canUseOnAbility(sticky_touch, actual_action_type));
                    }
                }
                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }


    [TypeId("04d49dd76437413bac022ef3b288c1df")]
    public abstract class FreeActionAbilityUseBase : UnitFactComponentDelegate
    {
        public abstract bool canUseOnAbility(AbilityData ability, CommandType actual_action_type);

        public override void OnTurnOn()
        {
            this.Owner.Ensure<UnitPartFreeAbilityUse>().addBuff(this.Fact);
        }


        public override void OnTurnOff()
        {
            this.Owner.Ensure<UnitPartFreeAbilityUse>().removeBuff(this.Fact);
        }
    }


    public class UnitPartSwiftAbilityUse : AdditiveUnitPartWithCheckLock
    {
        public bool canBeUsedOnAbility(AbilityData ability, CommandType actual_action_type)
        {
            if (buffs.Empty())
            {
                return false;
            }

            foreach (var b in buffs)
            {
                bool result = check<SwiftActionAbilityUseBase>(b, c => c.canUseOnAbility(ability, actual_action_type));
                if (!result)
                {
                    var sticky_touch = ability?.StickyTouch;
                    if (sticky_touch != null)
                    {
                        result = check<SwiftActionAbilityUseBase>(b, c => c.canUseOnAbility(sticky_touch, actual_action_type));
                    }
                }
                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }


    public class UnitPartMoveAbilityUse : AdditiveUnitPartWithCheckLock
    {
        public bool canBeUsedOnAbility(AbilityData ability, CommandType actual_action_type)
        {
            if (buffs.Empty())
            {
                return false;
            }

            foreach (var b in buffs)
            {
                bool result = check<MoveActionAbilityUseBase>(b, c => c.canUseOnAbility(ability, actual_action_type));
                if (!result)
                {
                    var sticky_touch = ability?.StickyTouch;
                    if (sticky_touch != null)
                    {
                        result = check<MoveActionAbilityUseBase>(b, c => c.canUseOnAbility(sticky_touch, actual_action_type));
                    }
                }
                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }


    public class UnitPartStandardActionAbilityUse : AdditiveUnitPartWithCheckLock
    {
        public bool canBeUsedOnAbility(AbilityData ability, CommandType actual_action_type)
        {
            if (buffs.Empty())
            {
                return false;
            }

            foreach (var b in buffs)
            {
                bool result = check<StandardActionAbilityUseBase>(b, c => c.canUseOnAbility(ability, actual_action_type));
                if (!result)
                {
                    var sticky_touch = ability?.StickyTouch;
                    if (sticky_touch != null)
                    {
                        result = check<StandardActionAbilityUseBase>(b, c => c.canUseOnAbility(sticky_touch, actual_action_type));
                    }
                }
                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [TypeId("c103f83a3228440985891fb62e7bf830")]
    public abstract class MoveActionAbilityUseBase : UnitFactComponentDelegate
    {
        public abstract bool canUseOnAbility(AbilityData ability, CommandType actual_action_type);

        public override void OnTurnOn()
        {
            this.Owner.Ensure<UnitPartMoveAbilityUse>().addBuff(this.Fact);
        }


        public override void OnTurnOff()
        {
            this.Owner.Ensure<UnitPartMoveAbilityUse>().removeBuff(this.Fact);
        }
    }




    [TypeId("b9f63ab06c474711831d592d693fb9ed")]
    public abstract class StandardActionAbilityUseBase : UnitFactComponentDelegate
    {
        public abstract bool canUseOnAbility(AbilityData ability, CommandType actual_action_type);

        public override void OnTurnOn()
        {
            this.Owner.Ensure<UnitPartStandardActionAbilityUse>().addBuff(this.Fact);
        }


        public override void OnTurnOff()
        {
            this.Owner.Ensure<UnitPartStandardActionAbilityUse>().removeBuff(this.Fact);
        }
    }


    [TypeId("75c236929094446d8fb49c079a4b6ed4")]
    public abstract class SwiftActionAbilityUseBase : UnitFactComponentDelegate
    {
        public abstract bool canUseOnAbility(AbilityData ability, CommandType actual_action_type);

        public override void OnTurnOn()
        {
            this.Owner.Ensure<UnitPartSwiftAbilityUse>().addBuff(this.Fact);
        }


        public override void OnTurnOff()
        {
            this.Owner.Ensure<UnitPartSwiftAbilityUse>().removeBuff(this.Fact);
        }
    }

}
