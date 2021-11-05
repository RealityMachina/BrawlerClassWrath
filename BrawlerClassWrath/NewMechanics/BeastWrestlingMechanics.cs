using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrawlerClassWrath.NewMechanics
{

    // Token: 0x02001CED RID: 7405
    [ComponentName("Armor check penalty increase")]
    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [TypeId("d2e28e1344544411a7809e508f26b918")]
    public class BeastDefences : UnitFactComponentDelegate, ITargetRulebookHandler<RuleAttackRoll>, IRulebookHandler<RuleAttackRoll>, ISubscriber, ITargetRulebookSubscriber
    {
        // Token: 0x17001FA3 RID: 8099
        // (get) Token: 0x0600BDF5 RID: 48629 RVA: 0x00301E34 File Offset: 0x00300034
        public ReferenceArrayProxy<BlueprintUnitFact, BlueprintUnitFactReference> Facts
        {
            get
            {
                return this.m_Facts;
            }
        }

        // Token: 0x0600BDF6 RID: 48630 RVA: 0x00301E44 File Offset: 0x00300044
        public void OnEventAboutToTrigger(RuleAttackRoll evt)
        {
            bool flag = false;
            if(!evt.Target.Descriptor.HasFact(requiredFact)) //ignore if we don't have required feat
            {
                return;
            }

            foreach (BlueprintUnitFact blueprintUnitFact in this.Facts)
            {
                if (blueprintUnitFact != null)
                {
                    if (evt.Initiator.Descriptor.HasFact(blueprintUnitFact) || evt.Initiator.Descriptor.Alignment.ValueRaw.HasComponent(this.Alignment))
                    {
                        flag = true;
                        break;
                    }
                   
                }
            }
            if (flag)
            {
                evt.AddTemporaryModifier(evt.Target.Stats.AC.AddModifier(GetRank(), base.Runtime, this.Descriptor));
            }
        }

        // Token: 0x0600BDF7 RID: 48631 RVA: 0x00003AE3 File Offset: 0x00001CE3
        public void OnEventDidTrigger(RuleAttackRoll evt)
        {
        }


        private int GetRank()
        {
            int i = 1;
            foreach (Feature feature in base.Owner.Progression.Features) // this SHOULDN'T include feature selections
            {
                if (feature.Blueprint.name != Fact.Blueprint.name && feature.Blueprint.Groups.Contains(FeatureGroup.FavoriteEnemy))
                {

                    i++;
                }
            }
            return i;
        }
        // Token: 0x04007C64 RID: 31844
        [NotNull]
        [SerializeField]
        [FormerlySerializedAs("Facts")]
        public BlueprintUnitFactReference[] m_Facts;

        public BlueprintUnitFactReference requiredFact;

        // Token: 0x04007C66 RID: 31846
        public ModifierDescriptor Descriptor;

        // Token: 0x04007C67 RID: 31847
        public AlignmentComponent Alignment;
    }

    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [AllowMultipleComponents]
    [TypeId("20aa3b19e57c421e9aebbfa5c5eb3022")]
    public class BeastTraining : UnitFactComponentDelegate
    {
        // Token: 0x17001948 RID: 6472
        // (get) Token: 0x0600A141 RID: 41281 RVA: 0x00298F3F File Offset: 0x0029713F
        public ReferenceArrayProxy<BlueprintUnitFact, BlueprintUnitFactReference> CheckedFacts
        {
            get
            {
                return this.m_CheckedFacts;
            }
        }
        private int GetRank()
        {
            int i = 1;
            foreach (Feature feature in base.Owner.Progression.Features) // this SHOULDN'T include feature selections
            {
                if (feature.Blueprint.name != Fact.Blueprint.name && feature.Blueprint.Groups.Contains(FeatureGroup.FavoriteEnemy))
                {

                    i++;
                }
            }
            return i;
        }
        // Token: 0x0600A13E RID: 41278 RVA: 0x00298EF2 File Offset: 0x002970F2
        public override void OnTurnOn()
        {
            base.Owner.Ensure<UnitPartBeastTraining>().AddEntry(this.CheckedFacts.ToArray<BlueprintUnitFact>(), base.Fact, 2 * GetRank());
        }

        // Token: 0x0600A13F RID: 41279 RVA: 0x00298F27 File Offset: 0x00297127
        public override void OnTurnOff()
        {
            base.Owner.Ensure<UnitPartBeastTraining>().RemoveEntry(base.Fact);
        }

        // Token: 0x04006C4F RID: 27727
        [SerializeField]
        [FormerlySerializedAs("CheckedFacts")]
        public BlueprintUnitFactReference[] m_CheckedFacts;
    }


    public class UnitPartBeastTraining : UnitPart, IInitiatorRulebookHandler<RuleCalculateCMD>, IRulebookHandler<RuleCalculateCMD>, ISubscriber, IInitiatorRulebookSubscriber, IInitiatorRulebookHandler<RuleCalculateCMB>, IRulebookHandler<RuleCalculateCMB>
    {
        // Token: 0x06009C73 RID: 40051 RVA: 0x00287B84 File Offset: 0x00285D84
        public void AddEntry(BlueprintUnitFact[] feature, EntityFact source, int bonus)
        {
            FavoredEntry item = new UnitPartBeastTraining.FavoredEntry
            {
                CheckedFeatures = feature,
                Source = source,
                Bonus = bonus
            };
            this.Entries.Add(item);
        }

        // Token: 0x06009C74 RID: 40052 RVA: 0x00287BB8 File Offset: 0x00285DB8
        public void RemoveEntry(EntityFact source)
        {
            this.Entries.RemoveAll((UnitPartBeastTraining.FavoredEntry p) => p.Source == source);
        }

        // Token: 0x06009C75 RID: 40053 RVA: 0x00287BEC File Offset: 0x00285DEC
        public bool HasEntry(BlueprintUnitFact feature)
        {
            return this.Entries.Any((UnitPartBeastTraining.FavoredEntry p) => p.CheckedFeatures.Contains(feature));
        }

        // Token: 0x06009C76 RID: 40054 RVA: 0x00287C20 File Offset: 0x00285E20
        public void OnEventAboutToTrigger(RuleCalculateCMD evt)
        {
            int num = 0;
            EntityFact entityFact = null;
            foreach (UnitPartBeastTraining.FavoredEntry favoredEntry in this.Entries)
            {
                IEnumerable<BlueprintUnitFact> checkedFeatures = favoredEntry.CheckedFeatures;

                foreach (var curFact in checkedFeatures)
                {
                    if (evt.Target.Facts.Get(curFact) != null)
                    {
                        if (favoredEntry.Bonus > num)
                        {
                            num = favoredEntry.Bonus;
                            entityFact = favoredEntry.Source;
                            break;
                        }
                    }
                }
            }
            if (entityFact != null)
            {
                evt.AddModifier(num, entityFact, ModifierDescriptor.FavoredEnemy);
            }
        }

        // Token: 0x06009C77 RID: 40055 RVA: 0x00003AE3 File Offset: 0x00001CE3
        public void OnEventDidTrigger(RuleCalculateCMD evt)
        {
        }

        // Token: 0x06009C78 RID: 40056 RVA: 0x00287CDC File Offset: 0x00285EDC
        public void OnEventAboutToTrigger(RuleCalculateCMB evt)
        {
            int num = 0;
            EntityFact entityFact = null;
            foreach (UnitPartBeastTraining.FavoredEntry favoredEntry in this.Entries)
            {
                IEnumerable<BlueprintUnitFact> checkedFeatures = favoredEntry.CheckedFeatures;
         
                foreach (var curFact in checkedFeatures)
                {
                    if (evt.Target.Facts.Get(curFact) != null)
                    {
                        if(favoredEntry.Bonus > num)
                        {
                            num = favoredEntry.Bonus;
                            entityFact = favoredEntry.Source;
                            break;
                        }
                    }
                }
            }
            if (entityFact != null)
            {
                evt.AddModifier(num, entityFact, ModifierDescriptor.FavoredEnemy);
            }
        }

        // Token: 0x06009C79 RID: 40057 RVA: 0x00003AE3 File Offset: 0x00001CE3
        public void OnEventDidTrigger(RuleCalculateCMB evt)
        {
        }

        // Token: 0x040069D0 RID: 27088
        public List<UnitPartBeastTraining.FavoredEntry> Entries = new List<UnitPartBeastTraining.FavoredEntry>();

        // Token: 0x02002E7A RID: 11898
        public class FavoredEntry
        {
            // Token: 0x0400AD9E RID: 44446
            public BlueprintUnitFact[] CheckedFeatures;

            // Token: 0x0400AD9F RID: 44447
            public EntityFact Source;

            // Token: 0x0400ADA0 RID: 44448
            public int Bonus;
        }
    }
}
