using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Controllers.Projectiles;
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
    [ComponentName("Add feature if owner has no armor or shield")]
    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    [TypeId("1f062a8356fc4927893f2d44a9135769")]
    public class SpecificWeaponGroupOrFeralCombatFeatureUnlock : UnitFactComponentDelegate<HasArmorFeatureUnlockData>, 
        IUnitActiveEquipmentSetHandler, IGlobalSubscriber, ISubscriber, IUnitEquipmentHandler, IUnitBuffHandler, ITeleportHandler
    {
        public BlueprintUnitFact NewFact
        {
            get
            {
                BlueprintUnitFactReference newFact = this.m_NewFact;
                if (newFact == null)
                {
                    Main.Log("No new fact, only returning null.");
                    return null;
                }
                return newFact.Get();
            }
        }
        [SerializeField]
        public WeaponFighterGroup[] weapon_groups;

        [SerializeField]
        [FormerlySerializedAs("NewFact")]
        public BlueprintUnitFactReference m_NewFact;

        [SerializeField]
        public BlueprintBuffReference m_RapidshotBuff;


        public BlueprintBuff RapidShotBuff
        {
            get
            {
                BlueprintBuffReference rapidshotBuff = this.m_RapidshotBuff;
                if (rapidshotBuff == null)
                {
                    return null;
                }
                return rapidshotBuff.Get();
            }
        }
        public override void OnActivate()
        {
            this.CheckEligibility();
        }

        // Token: 0x0600C4AE RID: 50350 RVA: 0x00314537 File Offset: 0x00312737
        public void HandleBuffDidAdded(Buff buff)
        {
            if (buff.Blueprint == RapidShotBuff)
            {
                CheckEligibility();
            }
        }

        // Token: 0x0600C4AF RID: 50351 RVA: 0x00314537 File Offset: 0x00312737
        public void HandleBuffDidRemoved(Buff buff)
        {
            if (buff.Blueprint == RapidShotBuff)
            {
                CheckEligibility();
            }
        }
        public override void OnDeactivate()
        {

            RemoveFact();
        }

        public void HandleUnitChangeActiveEquipmentSet(UnitDescriptor unit)
        {

            CheckEligibility();
        }

        public void CheckEligibility()
        {


            if (weapon_groups == null )
            {

            }
            else if (NewFact == null)
            {

            }
            bool FoundGroup = false;
            foreach (var group in weapon_groups)
            {
                if (this.Owner.Body.PrimaryHand.Weapon.Blueprint.FighterGroup.Contains(group) 
                    && (this.Owner.Body?.SecondaryHand?.MaybeWeapon == null
                       || (this.Owner.Body.SecondaryHand.MaybeWeapon.Blueprint.IsNatural
                           && (this.Owner.Body.SecondaryHand.MaybeWeapon.Blueprint.IsUnarmed))
                          ) //ensure that off-hand is empty since flurry normally does not allow off-hand attacks
                    )
                {
                    FoundGroup = true;

                    AddFact();
                    break;
                }
                else if (this.Owner.Body.PrimaryHand.Weapon.Blueprint.FighterGroup.Contains(group)
                    && (this.Owner.Body.SecondaryHand.Weapon.Blueprint.FighterGroup.Contains(group) || this.Owner.Body?.SecondaryHand?.MaybeWeapon == null))
                {
                    FoundGroup = true;

                    AddFact();
                    break;
                }

            }
            if(!FoundGroup)
            {

                RemoveFact();
            }
        }

        public void AddFact()
        {
            if (Data.AppliedFact != null)
            {

                return;
            }
            Data.AppliedFact = Owner.AddFact(NewFact, null, null);
        }

        public void RemoveFact()
        {
            if (Data.AppliedFact == null)
            {
 
                return;
            }
            Owner.RemoveFact(Data.AppliedFact);
            Data.AppliedFact = null;
        }

        public void HandleEquipmentSlotUpdated(ItemSlot slot, ItemEntity previousItem)
        {
            if (slot.Owner != Owner) // INVESTIGATION: the original slot.owner != Owner gave false negatives. Why? 
            {
                //Main.Log("Item being removed is " + previousItem?.Name);
                //Main.Log("Is slot owner main char" + slot.Owner.IsMainCharacter.ToString());
                //Main.Log("Slot owner is " + slot.Owner.CharacterName);

                //Main.Log("Is fact owner main char" + Owner.IsMainCharacter.ToString());
                //Main.Log("fact owner is " + Owner.CharacterName);
                return;
            }

            //if (!slot.Active)
            //{
            //    return;
            //}
            this.CheckEligibility();                    
        }


        public void HandlePartyTeleport(AreaEnterPoint enterPoint)
        {
            //we know the party moved so check
            this.CheckEligibility();
        }
        //public new void OnTurnOn()
        //{
        //    this.CheckEligibility();
        //}
    }
}
