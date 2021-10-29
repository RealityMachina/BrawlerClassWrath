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
    [TypeId("0385ff13e93e4668880dd7e78980f5ec")]
    public class AddBrawlerFlurryOnActivation : UnitFactComponentDelegate
    {

        public override void OnActivate()
        {

            Owner.Get<UnitPartBrawler>()?.Activate();

        }

        public override void OnDeactivate()
        {
            Owner.Get<UnitPartBrawler>()?.Deactivate();
        }


    }

    
}
