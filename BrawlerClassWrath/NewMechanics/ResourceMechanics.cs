using System;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrawlerClassWrath.NewMechanics
{
    [AllowMultipleComponents]
    [TypeId("3f9cc084a6b34d4fa12cc2ad92b471af")]
    public class ContextIncreaseResourceAmount : UnitFactComponentDelegate, IResourceAmountBonusHandler, IUnitSubscriber, ISubscriber
    {
        public ContextValue Value;
        public BlueprintAbilityResource Resource;

        public void CalculateMaxResourceAmount(BlueprintAbilityResource resource, ref int bonus)
        {
            if (!this.Fact.Active || (resource != this.Resource))
                return;
            bonus += this.Value.Calculate(this.Fact.MaybeContext);
        }
    }

}
