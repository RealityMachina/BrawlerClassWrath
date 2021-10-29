using System;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Parts;
using static BrawlerClassWrath.BrawlerClassPatcher;

namespace BrawlerClassWrath.NewMechanics
{
    [AllowMultipleComponents]
    [AllowedOn(typeof(BlueprintBuff), false)]
    [AllowedOn(typeof(BlueprintActivatableAbility), false)]
    [TypeId("eaaf147d84df4d57813da4e6b702d1c4")]
    class AddBrawlerPartUnitFact : UnitFactComponentDelegate
    {
        public override void OnActivate()
        {
            UnitPartBrawler flurrypart = Owner.Ensure<UnitPartBrawler>();
            flurrypart.Initialize(groups);
        }

        public override void OnDeactivate()
        {
            Owner.Remove<UnitPartBrawler>();
        }

        public WeaponFighterGroup[] groups;
    }
}
