using System;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Parts;
using static BrawlerClassWrath.BrawlerClassPatcher;

namespace BrawlerClassWrath.NewMechanics 
{
    [AllowMultipleComponents]
    [AllowedOn(typeof(BlueprintFeature), false)]
    [TypeId("14120eda625e429180a29752fc1aacca")]
    class AddBrawlerFlurryExtraAttacks : UnitFactComponentDelegate
    {
        public class AddBrawlerFlurryOnActivation : UnitFactComponentDelegate
        {

            public override void OnActivate()
            {

                Owner.Get<UnitPartBrawler>()?.IncreaseExtraAttacks();

            }

            public override void OnDeactivate()
            {
                Owner.Get<UnitPartBrawler>()?.DecreaseExtraAttacks();
            }


        }
    }
}
