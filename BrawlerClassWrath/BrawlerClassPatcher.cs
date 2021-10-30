using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.Localization;
using Kingmaker.UI.UnitSettings;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using BrawlerClassWrath.Config;
using BrawlerClassWrath.Extensions;
using BrawlerClassWrath.Utilities;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Items.Shields;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Designers.Mechanics.Buffs;
using BrawlerClassWrath.NewMechanics;
using Kingmaker.Blueprints.Root;
using Kingmaker.Enums;
using BrawlerClassWrath.NewMechanics.CombatManeuverMechanics;
using Newtonsoft.Json;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Blueprints.Facts;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.RuleSystem;
using Kingmaker.Enums.Damage;
using Kingmaker.UnitLogic.Alignments;

namespace BrawlerClassWrath
{
    [HarmonyPatch(typeof(BlueprintsCache), "Init")]
    public class BrawlerClassPatcher
    {
        static bool Initialized;
        static public BlueprintProgression brawler_progression;
        static public BlueprintFeatureSelection combat_feat;
        static public BlueprintFeature martial_training;
        static public BlueprintFeature brawlers_cunning;
        static public BlueprintFeature unarmed_strike;
        static public BlueprintFeature brawlers_flurry;
        static public BlueprintFeature brawlers_flurry11;
        static public BlueprintFeature ac_bonus;
        static public BlueprintFeature brawler_proficiencies;
        static public BlueprintFeatureSelection[] maneuver_training = new BlueprintFeatureSelection[5];
        static public BlueprintAbilityResource knockout_resource;
        static public BlueprintFeature knockout;
        static public BlueprintFeature brawlers_strike_magic;
        static public BlueprintFeature brawlers_strike_cold_iron_and_silver;
        static public BlueprintFeatureSelection brawlers_strike_alignment;
        static public BlueprintFeature brawlers_strike_adamantine;
        static public BlueprintFeature close_weapon_mastery;
        static public BlueprintFeature awesome_blow;
        static public BlueprintAbility awesome_blow_ability;
        static public BlueprintFeature awesome_blow_improved;
        static public BlueprintFeature perfect_warrior;
        static public BlueprintCharacterClass BrawlerClass;

        
        static void Postfix()
        {
            if (Initialized) return;
            Initialized = true;
            Main.LogHeader("Loading RM Brawler Class");
            //  MakeDemonSpellbookAndProgress();
            MakeBrawler();
        }

        static void MakeBrawler()
        {
            var DemonClass = Resources.GetBlueprint<BlueprintCharacterClass>("8e19495ea576a8641964102d177e34b7");
            var BabFull = Resources.GetBlueprint<BlueprintStatProgression>("b3057560ffff3514299e8b93e7648a9d");
            var SavePrestigeHigh = Resources.GetBlueprint<BlueprintStatProgression>("1f309006cd2855e4e91a6c3707f3f700");
            BrawlerClass = Helpers.CreateBlueprint<BlueprintCharacterClass>("RMBrawlerHybridClass");
            var FighterClass = Resources.GetBlueprint<BlueprintCharacterClass>("48ac8db94d5de7645906c7d0ad3bcfbd");
            var MonkClass = Resources.GetBlueprint<BlueprintCharacterClass>("e8f21e5b58e0569468e420ebea456124");
            

     
            BrawlerClass.m_Spellbook = null;
            BrawlerClass.HitDie = Kingmaker.RuleSystem.DiceType.D10;
            //BrawlerClass.m_SignatureAbilities = DemonClass.m_SignatureAbilities;
            BrawlerClass.IsDivineCaster = true;
            BrawlerClass.PrestigeClass = false;
            BrawlerClass.SkillPoints = MonkClass.SkillPoints;
            
            BrawlerClass.m_BaseAttackBonus = MonkClass.m_BaseAttackBonus;
            BrawlerClass.m_FortitudeSave = MonkClass.m_FortitudeSave;
            BrawlerClass.m_ReflexSave = MonkClass.m_ReflexSave;
            BrawlerClass.m_WillSave = MonkClass.m_WillSave;
            BrawlerClass.ClassSkills = new StatType[] { StatType.SkillAthletics, StatType.SkillMobility, StatType.SkillPersuasion, StatType.SkillPerception, StatType.SkillKnowledgeWorld };
 
            BrawlerClass.LocalizedName = Main.MakeLocalizedString("RMBrawlerName", "Brawler");
            BrawlerClass.LocalizedDescription = Main.MakeLocalizedString("RMBralwerDesc", "Deadly even with nothing in her hands, a brawler eschews using the fighter’s heavy armor and the monk’s mysticism, focusing instead on perfecting many styles of brutal unarmed combat. Versatile, agile, and able to adapt to most enemy attacks, a brawler’s body is a powerful weapon.");
            BrawlerClass.LocalizedDescriptionShort = Main.MakeLocalizedString("RMBralwerShortDesc", "Brawlers are fighters who've chosen to eschew armor and monk mysticism alike to practice the art of unarmed combat, learning powerful techniques like Awesome Blow.");
            BrawlerClass.m_Difficulty = 3;
            BrawlerClass.StartingGold = MonkClass.StartingGold;
            BrawlerClass.PrimaryColor = FighterClass.PrimaryColor;
            BrawlerClass.SecondaryColor = FighterClass.SecondaryColor;
            BrawlerClass.RecommendedAttributes = new StatType[] { StatType.Strength, StatType.Dexterity, StatType.Constitution };
            BrawlerClass.NotRecommendedAttributes = MonkClass.NotRecommendedAttributes;
            BrawlerClass.m_EquipmentEntities = MonkClass.m_EquipmentEntities;
            BrawlerClass.MaleEquipmentEntities = MonkClass.MaleEquipmentEntities;
            BrawlerClass.FemaleEquipmentEntities = MonkClass.FemaleEquipmentEntities;
            BrawlerClass.AddComponent<PrerequisiteNoClassLevel>(c => {

                c.m_CharacterClass = Resources.GetBlueprint<BlueprintCharacterClass>("4cd1757a0eea7694ba5c933729a53920").ToReference<BlueprintCharacterClassReference>(); // no animal class
            });
            BrawlerClass.AddComponent<PrerequisiteIsPet>(c => {

                c.Not = true;
                c.HideInUI = true;
            });
            BrawlerClass.m_StartingItems = new BlueprintItemReference[]
            {

                Resources.GetBlueprint<BlueprintItemWeapon>("43ff56218554d8547840e7659816db5e").ToReference<BlueprintItemReference>(), //punching dagger
                Resources.GetBlueprint<BlueprintItemShield>("f4cef3ba1a15b0f4fa7fd66b602ff32b").ToReference<BlueprintItemReference>(), //heavy shield
                 Resources.GetBlueprint<BlueprintItemWeapon>("ada85dae8d12eda4bbe6747bb8b5883c").ToReference<BlueprintItemReference>(), //quarterstaff
                 Resources.GetBlueprint<BlueprintItemArmor>("afbe88d27a0eb544583e00fa78ffb2c7").ToReference<BlueprintItemReference>(), //studded leather
                 Resources.GetBlueprint<BlueprintItemEquipmentUsable>("d52566ae8cbe8dc4dae977ef51c27d91").ToReference<BlueprintItemReference>(), //cure light wounds potion
                Resources.GetBlueprint<BlueprintItemEquipmentUsable>("d52566ae8cbe8dc4dae977ef51c27d91").ToReference<BlueprintItemReference>(), //cure light wounds potion
            };

            CreateBrawlerProgression();
            BrawlerClass.m_Progression = brawler_progression.ToReference<BlueprintProgressionReference>();
            BrawlerClass.m_SignatureAbilities = new BlueprintFeatureReference[] {
                awesome_blow.ToReference<BlueprintFeatureReference>(),
                brawlers_flurry.ToReference<BlueprintFeatureReference>(),
                perfect_warrior.ToReference<BlueprintFeatureReference>()
            };

            Helpers.RegisterClass(BrawlerClass);
        }

        static void CreateBrawlerProgression()
        {
            var FighterClass = Resources.GetBlueprint<BlueprintCharacterClass>("48ac8db94d5de7645906c7d0ad3bcfbd");
            var MonkClass = Resources.GetBlueprint<BlueprintCharacterClass>("e8f21e5b58e0569468e420ebea456124");
            brawler_proficiencies = Helpers.CreateBlueprint<BlueprintFeature>("RMBrawlerProficiencies");
            brawler_proficiencies.Ranks = 1;
            brawler_proficiencies.IsClassFeature = true;
            brawler_proficiencies.m_Icon = Resources.GetBlueprint<BlueprintFeature>("8c971173613282844888dc20d572cfc9").m_Icon;
            brawler_proficiencies.SetNameDescription("Brawler Proficiencies", "A brawler is proficient with all simple weapons plus the handaxe, short sword, and weapons from the close fighter weapon group. She is proficient with light armor and shields (except tower shields).");
            // library.CopyAndAdd<BlueprintFeature>("8c971173613282844888dc20d572cfc9", "BrawlerProficiencies", ""); //cleric proficiencies

            brawler_proficiencies.AddComponent<AddFacts>(c => {
                c.m_Facts = new BlueprintUnitFactReference[]
                {
                    Resources.GetBlueprint<BlueprintFeature>("6d3728d4e9c9898458fe5e9532951132").ToReference<BlueprintUnitFactReference>(), //light armor
                    Resources.GetBlueprint<BlueprintFeature>("e70ecf1ed95ca2f40b754f1adb22bbdd").ToReference<BlueprintUnitFactReference>(), //simple weapons
                    Resources.GetBlueprint<BlueprintFeature>("cb8686e7357a68c42bdd9d4e65334633").ToReference<BlueprintUnitFactReference>(), //shields
                };
                

            });
            brawler_proficiencies.AddComponent<AddProficiencies>(c => {

                c.WeaponProficiencies = new WeaponCategory[] {
                    WeaponCategory.Handaxe,
                    WeaponCategory.SpikedLightShield,
                    WeaponCategory.SpikedHeavyShield,
                    WeaponCategory.WeaponLightShield,
                    WeaponCategory.WeaponHeavyShield,
                    WeaponCategory.Shortsword
                };
            });



            var fist1d6_monk = Resources.GetBlueprint<BlueprintFeature>("c3fbeb2ffebaaa64aa38ce7a0bb18fb0");
            unarmed_strike = Helpers.CreateBlueprint<BlueprintFeature>("RMBrawlerUnarmedStrikeFeature");
            unarmed_strike.SetNameDescription("Unarmed Strike", "At 1st level, a brawler gains Improved Unarmed Strike as a bonus feat. The damage dealt by a Medium brawler's unarmed strike increases with level: 1d6 at levels 1–3, 1d8 at levels 4–7, 1d10 at levels 8–11, 2d6 at levels 12–15, 2d8 at levels 16–19, and 2d10 at level 20.\nIf the sacred fist is Small, his unarmed strike damage increases as follows: 1d4 at levels 1–3, 1d6 at levels 4–7, 1d8 at levels 8–11, 1d10 at levels 12–15, 2d6 at levels 16–19, and 2d8 at level 20.\nIf the brawler is Large, his unarmed strike damage increases as follows: 1d8 at levels 1–3, 2d6 at levels 4–7, 2d8 at levels 8–11, 3d6 at levels 12–15, 3d8 at levels 16–19, and 4d8 at level 20.");
            unarmed_strike.IsClassFeature = true;
            unarmed_strike.Ranks = 1;
            unarmed_strike.AddComponent<EmptyHandWeaponOverride>(c =>
            {
                c.m_Weapon = Resources.GetBlueprint<BlueprintItemWeapon>("f60c5a820b69fb243a4cce5d1d07d06e").ToReference<BlueprintItemWeaponReference>();
                c.IsPermanent = true;
                c.IsMonkUnarmedStrike = true;
            });

            //we add class to progression after to prevent monk from increasing unarmed damage using brawler class levels for brawler archetypes that remove unarmed strike feature
          //  ClassToProgression.addClassToFeat(brawler_class, new BlueprintArchetype[] { }, ClassToProgression.DomainSpellsType.NoSpells, unarmed_strike, monk_class);
          // RM: checked the original mod and the above appears to done nothing with unarmed strike????
               var improved_unarmed_strike = Resources.GetBlueprint<BlueprintFeature>("7812ad3672a4b9a4fb894ea402095167");

            var fighterbonusfeat = Resources.GetBlueprint<BlueprintFeatureSelection>("41c8486641f7d6d4283ca9dae4147a9f");
            combat_feat = Helpers.CreateBlueprint<BlueprintFeatureSelection>("RMBrawlerCombatFeat");
            combat_feat.m_AllFeatures = fighterbonusfeat.m_AllFeatures;
            combat_feat.Group = FeatureGroup.CombatFeat;
            combat_feat.IsClassFeature = true;
            combat_feat.Ranks = 1;
            combat_feat.SetName("Bonus Combat Feat");
            combat_feat.SetDescription("At 1st level, and at every even level thereafter, a brawler gains a bonus feat in addition to those gained from normal advancement (meaning that the brawler gains a feat at every level). These bonus feats must be selected from those listed as Combat Feats.");

            createMartialTraining();
            createBrawlersCunning();
            createBrawlersFlurry();
            createPerfectWarrior();
            createManeuverTraining();
            createACBonus();
            createKnockout();
            createBrawlersStrike();
            createCloseWeaponMastery();
            CreateAwesomeBlow();

            brawler_progression = CommonHelpers.CreateProgression("RMBrawlerProgression",
                                                              BrawlerClass.Name,
                                                              BrawlerClass.Description,
                                                              BrawlerClass.Icon,
                                                              FeatureGroup.None);

            var classRefs = new BlueprintProgression.ClassWithLevel[getBrawlerArray().Count()];

            for(var i = 0; i < classRefs.Length; i++)
            {
                classRefs[i] = Helpers.ClassToClassWithLevel(getBrawlerArray()[i]);
            }

            brawler_progression.m_Classes = classRefs;



            brawler_progression.LevelEntries = new LevelEntry[] {Helpers.LevelEntry(1, brawler_proficiencies, brawlers_cunning,  improved_unarmed_strike, unarmed_strike,
                                                                                       martial_training, combat_feat,
                                                                                       Resources.GetBlueprint<BlueprintFeature>("d3e6275cfa6e7a04b9213b7b292a011c"), // ray calculate feature
                                                                                       Resources.GetBlueprint<BlueprintFeature>("62ef1cdb90f1d654d996556669caf7fa")), // touch calculate feature                                                                                      
                                                                    Helpers.LevelEntry(2, brawlers_flurry, combat_feat),
                                                                    Helpers.LevelEntry(3, maneuver_training[0]),
                                                                    Helpers.LevelEntry(4, ac_bonus, knockout, combat_feat, Resources.GetBlueprint<BlueprintFeature>("8267a0695a4df3f4ca508499e6164b98")), //1d8 damage with fists
                                                                    Helpers.LevelEntry(5, brawlers_strike_magic, close_weapon_mastery),
                                                                    Helpers.LevelEntry(6, combat_feat),
                                                                    Helpers.LevelEntry(7, maneuver_training[1]),
                                                                    Helpers.LevelEntry(8, combat_feat, Resources.GetBlueprint<BlueprintFeature>("f790a36b5d6f85a45a41244f50b947ca")), //1d10
                                                                    Helpers.LevelEntry(9, brawlers_strike_cold_iron_and_silver),
                                                                    Helpers.LevelEntry(10, combat_feat),
                                                                    Helpers.LevelEntry(11, maneuver_training[2], brawlers_flurry11),
                                                                    Helpers.LevelEntry(12, combat_feat, brawlers_strike_alignment, Resources.GetBlueprint<BlueprintFeature>("b3889f445dbe42948b8bb1ba02e6d949")), //2d6
                                                                    Helpers.LevelEntry(13),
                                                                    Helpers.LevelEntry(14, combat_feat),
                                                                    Helpers.LevelEntry(15, maneuver_training[3]),
                                                                    Helpers.LevelEntry(16, combat_feat, awesome_blow, Resources.GetBlueprint<BlueprintFeature>("078636a2ce835e44394bb49a930da230")), //2d8
                                                                    Helpers.LevelEntry(17, brawlers_strike_adamantine),
                                                                    Helpers.LevelEntry(18, combat_feat),
                                                                    Helpers.LevelEntry(19, maneuver_training[4]),
                                                                    Helpers.LevelEntry(20, combat_feat, awesome_blow_improved, perfect_warrior, Resources.GetBlueprint<BlueprintFeature>("df38e56fa8b3f0f469d55f9aa26b3f5c")) //2d10
                                                                    };

            brawler_progression.m_UIDeterminatorsGroup = new BlueprintFeatureBaseReference[] {
                brawler_proficiencies.ToReference<BlueprintFeatureBaseReference>(),
                unarmed_strike.ToReference<BlueprintFeatureBaseReference>(),
                improved_unarmed_strike.ToReference<BlueprintFeatureBaseReference>(),
                martial_training.ToReference<BlueprintFeatureBaseReference>() };
            brawler_progression.UIGroups = new UIGroup[]  {Helpers.CreateUIGroup(brawlers_flurry, brawlers_flurry11),
                                                                Helpers.CreateUIGroup(combat_feat),
                                                                Helpers.CreateUIGroup(maneuver_training),
                                                                Helpers.CreateUIGroup(brawlers_strike_magic, brawlers_strike_cold_iron_and_silver, brawlers_strike_alignment, brawlers_strike_adamantine),
                                                                Helpers.CreateUIGroup(brawlers_cunning, ac_bonus, close_weapon_mastery, perfect_warrior),
                                                                Helpers.CreateUIGroup(knockout, awesome_blow, awesome_blow_improved)
                                                           };


        }
        public static BlueprintCharacterClass[] getBrawlerArray()
        {
            return new BlueprintCharacterClass[] { BrawlerClass };
        }

        static void createCloseWeaponMastery()
        {
            DiceFormula[] diceFormulas = new DiceFormula[] {new DiceFormula(1, DiceType.D4),
                                                            new DiceFormula(1, DiceType.D6),
                                                            new DiceFormula(1, DiceType.D8),
                                                            new DiceFormula(1, DiceType.D10),
                                                            new DiceFormula(2, DiceType.D6),
                                                            new DiceFormula(2, DiceType.D8)};

            close_weapon_mastery = CommonHelpers.CreateFeature("RMBrawlerCloseWeaponMasteryFeature",
                                              "Close Weapon Mastery",
                                              "At 5th level, a brawler’s damage with close weapons increases. When wielding a close weapon, she uses the unarmed strike damage of a brawler 4 levels lower instead of the base damage for that weapon (for example, a 5th-level Medium brawler wielding a punching dagger deals 1d6 points of damage instead of the weapon’s normal 1d4). If the weapon normally deals more damage than this, its damage is unchanged. This ability does not affect any other aspect of the weapon. The brawler can decide to use the weapon’s base damage instead of her adjusted unarmed strike damage—this must be declared before the attack roll is made.",
                                              Helpers.GetIcon("121811173a614534e8720d7550aae253"), //shield bash
                                              FeatureGroup.None,
                                              Helpers.Create<ContextWeaponDamageDiceReplacementWeaponCategory>(c =>
                                              {
                                                  c.dice_formulas = diceFormulas;
                                                  c.value = Helpers.CreateContextValue(AbilityRankType.Default);
                                                  c.categories = new WeaponCategory[] {WeaponCategory.SpikedHeavyShield, WeaponCategory.SpikedLightShield,
                                                                                       WeaponCategory.WeaponLightShield, WeaponCategory.WeaponHeavyShield,
                                                                                       WeaponCategory.PunchingDagger};
                                              }),
                                              Helpers.CreateContextRankConfig(baseValueType: ContextRankBaseValueType.ClassLevel,
                                                                                type: AbilityRankType.Default,
                                                                                progression: ContextRankProgression.StartPlusDivStep,
                                                                                startLevel: 4,
                                                                                stepLevel: 4,
                                                                                classes: getBrawlerArray())
                                              );
        }


        static void createBrawlersStrike()
        {
            brawlers_strike_magic = Helpers.CreateBlueprint<BlueprintFeature>("RMBrawlerKiStrikeMagic");
            brawlers_strike_magic.SetNameDescription("Brawler's Strike —  Magic",
                                                     "At 5th level, a brawler’s unarmed strikes are treated as magic weapons for the purpose of overcoming damage reduction");
            brawlers_strike_magic.Ranks = 1;
            brawlers_strike_magic.IsClassFeature = true;
            brawlers_strike_magic.AddComponent(Helpers.Create<AddOutgoingPhysicalDamageProperty>(c => {
                c.m_WeaponType = Resources.GetBlueprint<BlueprintWeaponType>("fcca8e6b85d19b14786ba1ab553e23ad").ToReference<BlueprintWeaponTypeReference>();
                c.AddMagic = true;
                c.CheckWeaponType = true;
                c.Material = Kingmaker.Enums.Damage.PhysicalDamageMaterial.Adamantite;
                c.Reality = Kingmaker.Enums.Damage.DamageRealityType.Ghost;
                c.Alignment = Kingmaker.Enums.Damage.DamageAlignment.Good;
            }));
            brawlers_strike_cold_iron_and_silver = Helpers.CreateBlueprint<BlueprintFeature>("RMBrawlerStrikeColdIronSilver"); 
            brawlers_strike_cold_iron_and_silver.SetNameDescription("Brawler's Strike —  Cold Iron and Silver",
                                                                "At 9th level, the brawler's unarmed attacks are treated as cold iron and silver for the purpose of overcoming damage reduction.");


            brawlers_strike_cold_iron_and_silver.Ranks = 1;
            brawlers_strike_cold_iron_and_silver.IsClassFeature = true;
            brawlers_strike_cold_iron_and_silver.AddComponent(Helpers.Create<AddOutgoingPhysicalDamageProperty>(c => {
                c.m_WeaponType = Resources.GetBlueprint<BlueprintWeaponType>("fcca8e6b85d19b14786ba1ab553e23ad").ToReference<BlueprintWeaponTypeReference>();
                c.AddMaterial = true;
                c.CheckWeaponType = true;
                c.Material = Kingmaker.Enums.Damage.PhysicalDamageMaterial.Silver;
                c.Reality = Kingmaker.Enums.Damage.DamageRealityType.Ghost;
                c.Alignment = Kingmaker.Enums.Damage.DamageAlignment.Good;
            }));
            brawlers_strike_cold_iron_and_silver.AddComponent(Helpers.Create<AddOutgoingPhysicalDamageProperty>(c => {
                c.m_WeaponType = Resources.GetBlueprint<BlueprintWeaponType>("fcca8e6b85d19b14786ba1ab553e23ad").ToReference<BlueprintWeaponTypeReference>();
                c.AddMaterial = true;
                c.CheckWeaponType = true;
                c.Material = Kingmaker.Enums.Damage.PhysicalDamageMaterial.ColdIron;
                c.Reality = Kingmaker.Enums.Damage.DamageRealityType.Ghost;
                c.Alignment = Kingmaker.Enums.Damage.DamageAlignment.Good;
            }));
            var damage_alignments = new DamageAlignment[]
            {
                DamageAlignment.Lawful,
                DamageAlignment.Chaotic,
                DamageAlignment.Good,
                DamageAlignment.Evil
            };

            var alignment = new AlignmentMaskType[]
            {
               AlignmentMaskType.Chaotic,
               AlignmentMaskType.Lawful,
               AlignmentMaskType.Evil,
               AlignmentMaskType.Good
            };

            brawlers_strike_alignment = CommonHelpers.CreateFeatureSelection("BrawlersStrikeAlignmentFeatureSelection",
                                                                        "Aligned Brawler's Strike",
                                                                        "At 12th level, the brawler chooses one alignment component: chaotic, evil, good, or lawful; her unarmed strikes also count as this alignment for the purpose of overcoming damage reduction. (This alignment component cannot be the opposite of the brawler’s actual alignment, such as a good brawler choosing evil strikes.)",

                                                                        null,
                                                                        FeatureGroup.None
                                                                        );

            for (int i = 0; i < damage_alignments.Length; i++)
            {
                var strike = Helpers.CreateBlueprint<BlueprintFeature>("RMBrawlersStrike" + damage_alignments[i].ToString());
                strike.SetNameDescription($"Brawler's Strike — {damage_alignments[i].ToString()}",
                                          brawlers_strike_alignment.Description);
                strike.Ranks = 1;
                strike.IsClassFeature = true;
                strike.AddComponent(CommonHelpers.createPrerequisiteAlignment(~alignment[i]));
                brawlers_strike_cold_iron_and_silver.AddComponent(Helpers.Create<AddOutgoingPhysicalDamageProperty>(c => {
                    c.m_WeaponType = Resources.GetBlueprint<BlueprintWeaponType>("fcca8e6b85d19b14786ba1ab553e23ad").ToReference<BlueprintWeaponTypeReference>();
                    c.AddAlignment = true;
                    c.CheckWeaponType = true;
                    c.Material = Kingmaker.Enums.Damage.PhysicalDamageMaterial.ColdIron;
                    c.Reality = Kingmaker.Enums.Damage.DamageRealityType.Ghost;
                    c.Alignment = damage_alignments[i];
                }));
                brawlers_strike_alignment.m_AllFeatures = brawlers_strike_alignment.m_AllFeatures.AddToArray(strike.ToReference<BlueprintFeatureReference>());
            }

            brawlers_strike_adamantine = Helpers.CreateBlueprint<BlueprintFeature>("RMBrawlersStrikeAdamantine");
            brawlers_strike_adamantine.SetNameDescription("Brawler's Strike — Adamantine",
                                                          "At 17th level, the brawler's unarmed attacks are treated as adamantine weapons for the purpose of overcoming damage reduction and bypassing hardness.");


            brawlers_strike_adamantine.Ranks = 1;
            brawlers_strike_adamantine.IsClassFeature = true;
            brawlers_strike_adamantine.AddComponent(Helpers.Create<AddOutgoingPhysicalDamageProperty>(c => {
                c.m_WeaponType = Resources.GetBlueprint<BlueprintWeaponType>("fcca8e6b85d19b14786ba1ab553e23ad").ToReference<BlueprintWeaponTypeReference>();
                c.AddMaterial = true;
                c.CheckWeaponType = true;
                c.Material = Kingmaker.Enums.Damage.PhysicalDamageMaterial.Adamantite;
                c.Reality = Kingmaker.Enums.Damage.DamageRealityType.Ghost;
                c.Alignment = Kingmaker.Enums.Damage.DamageAlignment.Good;
            }));
        }


        static void createKnockout()
        {
            knockout_resource = CommonHelpers.CreateAbilityResource("RMBrawlerKnockoutResource", "", "", "", null);
            knockout_resource.SetIncreasedByLevelStartPlusDivStep(1, 10, 1, 6, 1, 0, 0.0f, getBrawlerArray());


            var effect_buff = Helpers.CreateBuff("RMBrawlerKnockoutBuff", bp =>
            {

                bp.SetName("Knocked Out");
                bp.SetDescription("At 4th level, once per day a brawler can unleash a devastating attack that can instantly knock a target unconscious." +
                    " She must announce this intent before making her attack roll. If the brawler hits and the target takes damage from the blow, " +
                    "the target must succeed at a Fortitude saving throw (DC = 10 + 1/2 the brawler’s level + the higher of the brawler’s Strength or Dexterity modifier) " +
                    "or fall unconscious for 1d6 rounds. Each round on its turn, the unconscious target may attempt a new saving throw to end the effect as a full-round action " +
                    "that does not provoke attacks of opportunity. Creatures immune to critical hits or nonlethal damage are immune to this ability." +
                    " At 10th level, the brawler may use this ability twice per day; at 16th level, she may use it three times per day.");
                bp.m_Icon = Helpers.GetIcon("247a4068296e8be42890143f451b4b45");
                bp.AddComponent(CommonHelpers.createBuffStatusCondition(UnitCondition.Unconscious, SavingThrowType.Fortitude));
            });

            var apply_buff = CommonHelpers.createContextActionApplyBuff(effect_buff, CommonHelpers.CreateContextDuration(0, DurationRate.Rounds, DiceType.D6, 1), dispellable: false);
            var effect = CommonHelpers.CreateActionSavingThrow(SavingThrowType.Fortitude, CommonHelpers.CreateConditionalSaved(null, apply_buff));
            var on_hit = CommonHelpers.CreateConditional(CommonHelpers.createContextConditionHasFacts(false, CommonHelpers.undead, CommonHelpers.construct, CommonHelpers.elemental, CommonHelpers.aberration),
                                                   null,
                                                   effect);
            var buff = CommonHelpers.CreateBuff("RMBrawlerKnockoutOwnerBuff",
                                          "Knockout",
                                          effect_buff.Description,
                                          effect_buff.Icon,
                                          null
                                          );

            var physical_stat_property = HighestStatPropertyGetter.createProperty("RMBrawlerStrOrDexProperty", "", StatType.Strength, StatType.Dexterity);

            buff.AddComponents(CommonHelpers.createAddInitiatorAttackWithWeaponTrigger(Helpers.CreateActionList(effect, CommonHelpers.createContextActionOnContextCaster(CommonHelpers.createContextActionRemoveBuff(buff))),
                                                                               wait_for_attack_to_resolve: true),
                             CommonHelpers.createContextCalculateAbilityParamsBasedOnClassesWithProperty(getBrawlerArray(), physical_stat_property)
                             );

            var ability = CommonHelpers.CreateAbility("RMBrawlerKnockoutAbility",
                                                 buff.Name,
                                                 buff.Description,
                                                 "",
                                                 buff.Icon,
                                                 AbilityType.Extraordinary,
                                                 CommandType.Free,
                                                 AbilityRange.Personal,
                                                 "",
                                                 "",
                                                 CommonHelpers.CreateRunActions(CommonHelpers.createContextActionApplyBuff(buff, CommonHelpers.CreateContextDuration(), is_permanent: true, dispellable: false)),
                                                 CommonHelpers.createAbilityCasterHasNoFacts(buff),
                                                 knockout_resource.CreateResourceLogic(),
                                                 CommonHelpers.createContextCalculateAbilityParamsBasedOnClassesWithProperty(getBrawlerArray(), physical_stat_property)
                                                 );

            knockout = CommonHelpers.AbilityToFeature(ability, false);
            knockout.AddComponent(knockout_resource.CreateAddAbilityResource());
        }


        static void createACBonus()
        {
            var ac_bonus_feature = CommonHelpers.CreateFeature("RMBrawlerACBonusFeature",
                                                         "AC Bonus",
                                                         "At 4th level, when a brawler wears light or no armor, she gains a +1 dodge bonus to AC and CMD. This bonus increases by 1 at 9th, 13th, and 18th levels.\n"
                                                         + "These bonuses to AC apply against touch attacks. She loses these bonuses while immobilized or helpless, wearing medium or heavy armor, or carrying a medium or heavy load.",
                                                         Helpers.GetIcon("97e216dbb46ae3c4faef90cf6bbe6fd5"), //dodge
                                                         FeatureGroup.None,
                                                         CommonHelpers.CreateAddContextStatBonus(StatType.AC, ModifierDescriptor.Dodge),
                                                         CommonHelpers.CreateAddContextStatBonus(StatType.AC, ModifierDescriptor.Dodge, rankType: AbilityRankType.StatBonus),
                                                         Helpers.CreateContextRankConfig(baseValueType: ContextRankBaseValueType.ClassLevel,
                                                                                     classes: getBrawlerArray(),
                                                                                     progression: ContextRankProgression.Custom,
                                                                                     customProgression: new (int, int)[] { (8, 1), (12, 2), (17, 3), (20, 4) }),
                                                         Helpers.CreateContextRankConfig(baseValueType: ContextRankBaseValueType.FeatureList,
                                                                                     type: AbilityRankType.StatBonus,
                                                                                     featureList: new BlueprintFeature[] { perfect_warrior, perfect_warrior }
                                                                                     )
                                                         );

            ac_bonus = CommonHelpers.CreateFeature("RMBrawlerACBonusUnlock",
                                             ac_bonus_feature.Name,
                                             ac_bonus_feature.Description,
                                             ac_bonus_feature.Icon,
                                             FeatureGroup.None
 
                                             );
            ac_bonus.AddComponent(Helpers.Create<AddFeatureOnArmor>(a =>
            {
                a.m_NewFact = ac_bonus_feature.ToReference<BlueprintUnitFactReference>();
                a.required_armor = new ArmorProficiencyGroup[] { ArmorProficiencyGroup.Light, ArmorProficiencyGroup.None };
            }));
            ac_bonus.ReapplyOnLevelUp = true;
        }

        static void createManeuverTraining()
        {
            var maneuvers = new CombatManeuver[][] { new CombatManeuver[] { CombatManeuver.BullRush },
                                                     new CombatManeuver[] {CombatManeuver.Disarm },
                                                     new CombatManeuver[] {CombatManeuver.Trip },
                                                     new CombatManeuver[] {CombatManeuver.SunderArmor },
                                                     new CombatManeuver[] {CombatManeuver.DirtyTrickBlind, CombatManeuver.DirtyTrickEntangle, CombatManeuver.DirtyTrickSickened },
                                                      new CombatManeuver[] {CombatManeuverTypeExtender.AwesomeBlow.ToCombatManeuverType() },
                                                   };

            var names = new string[] { "Bull Rush", "Disarm", "Trip", "Sunder", "Dirty Trick", "Awesome Blow" };
            var icons = new UnityEngine.Sprite[]
            {
                Resources.GetBlueprint<BlueprintFeature>("b3614622866fe7046b787a548bbd7f59").Icon,
                Resources.GetBlueprint<BlueprintFeature>("25bc9c439ac44fd44ac3b1e58890916f").Icon,
                Resources.GetBlueprint<BlueprintFeature>("0f15c6f70d8fb2b49aa6cc24239cc5fa").Icon,
                Resources.GetBlueprint<BlueprintFeature>("9719015edcbf142409592e2cbaab7fe1").Icon,
                Resources.GetBlueprint<BlueprintFeature>("ed699d64870044b43bb5a7fbe3f29494").Icon,
                Resources.GetBlueprint<BlueprintFeature>("bdf58317985383540920c723db07aa3b").Icon, //pummeling bully
            };

            for (int i = 0; i < maneuver_training.Length; i++)
            {
                maneuver_training[i] = 
                CommonHelpers.CreateFeatureSelection($"RMManeuverTraining{i + 1}FeatureSelection",
                                                                   "Maneuver Training ",
                                                                   "At 3rd level, a brawler can select one combat maneuver to receive additional training. She gains a +1 bonus on combat maneuver checks when performing that combat maneuver and a +1 bonus to her CMD when defending against that maneuver.\n"
                                                                   + "At 7th level and every 4 levels thereafter, the brawler becomes further trained in another combat maneuver, gaining the above +1 bonus combat maneuver checks and to CMD. In addition, the bonuses granted by all previous maneuver training increase by 1 each.",
                                                                   null,
                                                                   FeatureGroup.None);
                maneuver_training[i].m_AllFeatures = new BlueprintFeatureReference[0];

                int lvl = 3 + i * 4;
                for (int j = 0; j < maneuvers.Length; j++)
                {
                    var feat = CommonHelpers.CreateFeature($"RMManeuverTraining{i + 1}" + maneuvers[j][0].ToString() + "Feature",
                                                     maneuver_training[i].Name + ": " + names[j] + $" ({lvl}{CommonHelpers.getNumExtension(lvl)} level)",
                                                     maneuver_training[i].Description,
                                                     icons[j],
                                                     FeatureGroup.None,
                                                     Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel,
                                                                                     classes: getBrawlerArray(),
                                                                                     progression: ContextRankProgression.StartPlusDivStep,
                                                                                     startLevel: lvl,
                                                                                     stepLevel: 4),
                                                     Helpers.CreateContextRankConfig(ContextRankBaseValueType.FeatureList,
                                                                                     type: AbilityRankType.StatBonus,
                                                                                     featureList: new BlueprintFeature[] { perfect_warrior, perfect_warrior }
                                                                                     )
                                                     );

                    foreach (var maneuver in maneuvers[j])
                    {
                        feat.AddComponents(Helpers.Create<SpecificCombatManeuverBonus>(s => { s.maneuver_type = maneuver; s.Value = Helpers.CreateContextValue(AbilityRankType.Default); }), // specific now handles both CMB and CMD
                                           Helpers.Create<SpecificCombatManeuverBonus>(s => { s.maneuver_type = maneuver; s.Value = Helpers.CreateContextValue(AbilityRankType.StatBonus); })
                                           );
                    }

                    if (j + 1 != maneuvers.Length)
                    {
                        for (int k = 0; k < i; k++)
                        {
                            var component = CommonHelpers.PrerequisiteNoFeature(maneuver_training[k].AllFeatures[j]);
                            if(component != null)
                            {
                                feat.AddComponent(component);
                            }
                            
                        }
                    }
                    if (j + 1 != maneuvers.Length || i + 1 == maneuver_training.Length)
                    {
                        maneuver_training[i].m_AllFeatures = maneuver_training[i].m_AllFeatures.AddToArray(feat.ToReference<BlueprintFeatureReference>());
                    }
                }
            }
        }


        static void CreateAwesomeBlow()
        {
            var text_entry = new Kingmaker.Blueprints.Root.Strings.CombatManeuverString.MyEntry
            {
                Value = CombatManeuverTypeExtender.AwesomeBlow.ToCombatManeuverType(),
                Text = Helpers.CreateString("RMAwesomeBlow.String", "Combat Maneuver: Awesome Blow")
            };
            BlueprintRoot.Instance.LocalizedTexts.CombatManeuver.Entries = BlueprintRoot.Instance.LocalizedTexts.CombatManeuver.Entries.AddToArray(text_entry);

            awesome_blow_improved = Helpers.CreateBlueprint<BlueprintFeature>("RMAwesomeBlowImprovedFeature");
            awesome_blow_improved.SetNameDescription("Improved Awesome Blow", "At 20th level, the brawler can use her awesome blow ability on creatures of any size.");
            awesome_blow_improved.m_Icon = Helpers.GetIcon("c5a39c8f1a2d6824ca565e6c1e4075a5"); //pummeling charge
            awesome_blow_improved.Groups = new FeatureGroup[] { FeatureGroup.None };

            var awesomeAbility = Helpers.CreateBlueprint<BlueprintAbility>("RMAwesomeBlowAction");
            var tripAction = Resources.GetBlueprint<BlueprintAbility>("6fd05c4ecfebd6f4d873325de442fc17");
            awesomeAbility.SetNameDescription("Awesome Blow",
             "At 16th level, the brawler can as a standard action perform an awesome blow combat maneuver against a corporeal creature of her size or smaller. " +
             "If the combat maneuver check succeeds, the opponent takes damage as if the brawler hit it with the close weapon she is wielding or an unarmed strike," +
             " it is knocked flying 10 feet in a direction of attack.");
            awesomeAbility.m_Icon = Helpers.GetIcon("bdf58317985383540920c723db07aa3b"); //pummeling bully
            awesomeAbility.LocalizedDuration = Main.MakeLocalizedString($"{awesomeAbility.name}.Duration", "");
            awesomeAbility.LocalizedSavingThrow = Main.MakeLocalizedString($"{awesomeAbility.name}.SavingThrow", "");
            awesomeAbility.Type = AbilityType.Special;
            awesomeAbility.CanTargetEnemies = true;
            awesomeAbility.CanTargetSelf = false;
            awesomeAbility.Animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Special;
            awesomeAbility.AnimationStyle = Kingmaker.View.Animation.CastAnimationStyle.CastActionSpecialAttack;
            awesomeAbility.ActionType = CommandType.Standard;
            awesomeAbility.AvailableMetamagic = Metamagic.Reach | Metamagic.Heighten | Metamagic.Quicken;
            awesomeAbility.MaterialComponent = tripAction.MaterialComponent;
            awesomeAbility.AddComponent<AbilityEffectRunAction>(a =>
            {
                a.Actions = Helpers.CreateActionList(Helpers.Create<ContextActionCombatManeuver>(c =>
                {
                    c.Type = CombatManeuverTypeExtender.AwesomeBlow.ToCombatManeuverType();
                    c.OnSuccess = Helpers.CreateActionList();
                })
                );
            });

            awesomeAbility.AddComponents(Helpers.Create<AbilityTargetNotBiggerUnlessFact>(a => a.fact = awesome_blow_improved),
                                  Helpers.Create<AbilityCasterMainWeaponGroupCheck>(a =>
                                  {
                                      a.groups = new WeaponFighterGroup[] { WeaponFighterGroup.Close };
                                      a.extra_categories = new WeaponCategory[] { WeaponCategory.UnarmedStrike };
                                  }
                                  )
                                  );

            awesome_blow = Helpers.CreateBlueprint<BlueprintFeature>("RMAwesomeBlowFeat");//Common.AbilityToFeature(ability, false);
            awesome_blow.m_Icon = awesomeAbility.m_Icon;
            awesome_blow.SetNameDescription(awesomeAbility.Name, awesomeAbility.Description);
            awesome_blow.Groups = new FeatureGroup[] { FeatureGroup.None };
            awesome_blow.AddComponent<AddFacts>(c => {
                c.m_Facts = new BlueprintUnitFactReference[] {
                    awesomeAbility.ToReference<BlueprintUnitFactReference>()
                };
            });
            awesome_blow.AddComponent(Helpers.Create<ManeuverTrigger>(m =>
            {
                m.OnlySuccess = true;
                m.ManeuverType = CombatManeuverTypeExtender.AwesomeBlow.ToCombatManeuverType();
                m.Action = Helpers.CreateActionList(Helpers.Create<ContextActionDealWeaponDamage>(),
                                                    Helpers.Create<ContextActionForceMove>(f => f.distance_dice = new ContextDiceValue() {
                                                        DiceType = Kingmaker.RuleSystem.DiceType.Zero,
                                                        BonusValue = 10,
                                                        DiceCountValue = 0
                                                    } )
                                                    );
            }//Helpers.CreateContextDiceValue(DiceType.Zero, 0, 10)
                )
            );

            awesome_blow_ability = awesomeAbility;
        }


        static void createPerfectWarrior()
        {
            perfect_warrior = CommonHelpers.CreateFeature("RMPerfectWarriorBrawlerFeature",
                                                    "Perfect Warrior",
                                                    "At 20th level, the brawler has reached the highest levels of her art. The brawler’s maneuver training increases by 2 and her dodge bonus to AC improves by 2. This replaces the 20th-level improvement to martial flexibility.",
                                                    Helpers.GetIcon("2a6a2f8e492ab174eb3f01acf5b7c90a"), //defensive stance
                                                    FeatureGroup.None
                                                    );
        }


        static void createMartialTraining()
        {
            var monk_class = Resources.GetBlueprint<BlueprintCharacterClass>("e8f21e5b58e0569468e420ebea456124");
            var fighter_class = Resources.GetBlueprint<BlueprintCharacterClass>("48ac8db94d5de7645906c7d0ad3bcfbd");

            martial_training = CommonHelpers.CreateFeature("RMBrawlerMartialTrainingFeat",
                                         "Martial Training",
                                         "At 1st level, a brawler counts her total brawler levels as both fighter levels and monk levels for the purpose of qualifying for feats. She also counts as both a fighter and a monk for feats and magic items that have different effects based on whether the character has levels in those classes (such as Stunning Fist and a monk’s robe). This ability does not automatically grant feats normally granted to fighters and monks based on class level, namely Stunning Fist.",
                                         
                                         null,
                                         FeatureGroup.None,
                                         CommonHelpers.createClassLevelsForPrerequisites(monk_class, BrawlerClass),
                                         CommonHelpers.createClassLevelsForPrerequisites(fighter_class, BrawlerClass)
                                         );

            var stunning_fist_resource = Resources.GetBlueprint<BlueprintAbilityResource>("d2bae584db4bf4f4f86dd9d15ae56558");
            stunning_fist_resource.m_MaxAmount.m_ClassDiv = stunning_fist_resource.m_MaxAmount.m_ClassDiv.AddToArray(BrawlerClass.ToReference<BlueprintCharacterClassReference>());
          //  ClassToProgression.addClassToResource(BrawlerClass, new BlueprintArchetype[0], stunning_fist_resource, monk_class);
            //perfect strike will make a copy of resource, so will be update automatically

            //fix robes ?
        }

        static void createBrawlersCunning()
        {
            brawlers_cunning = CommonHelpers.CreateFeature("RMBrawlersCunningFeature",
                                                     "Brawler’s Cunning",
                                                     "If the brawler’s Intelligence score is less than 13, it counts as 13 for the purpose of meeting the prerequisites of combat feats.",
                                                     Helpers.GetIcon("ae4d3ad6a8fda1542acf2e9bbc13d113"), //foxs cunning
                                                     FeatureGroup.None,
                                                     Helpers.Create<ReplaceStatForPrerequisites>(r => { r.OldStat = StatType.Intelligence; r.SpecificNumber = 13; r.Policy = ReplaceStatForPrerequisites.StatReplacementPolicy.SpecificNumber; })
                                                     );
        }

        static void createBrawlersFlurry()
        {
            var flurry1_monk = Resources.GetBlueprint<BlueprintFeature>("332362f3bd39ebe46a740a36960fdcb4");
            var flurry11_monk = Resources.GetBlueprint<BlueprintFeature>("de25523acc24b1448aa90f74d6512a08");
            flurry1_monk.Ranks++;
            flurry11_monk.Ranks++;


            brawlers_flurry = CommonHelpers.CreateFeature("RMBrawlersFlurryUnlock",
                                                    "Brawler’s Flurry",
                                                    "At 2nd level, a brawler can make a flurry of blows as a full attack. When making a flurry of blows, the brawler can make one additional attack at his highest base attack bonus. This additional attack stacks with the bonus attacks from haste and other similar effects. When using this ability, the brawler can make these attacks with any combination of his unarmed strikes, weapons from the close fighter weapon group, or weapons with thee monk special weapon quality. He takes no penalty for using multiple weapons when making a flurry of blows, but he does not gain any additional attacks beyond what's already granted by the flurry for doing so. (He can still gain additional attacks from a high base attack bonus, from this ability, and from haste and similar effects).\nAt 11th level, a brawler can make an additional attack at his highest base attack bonus whenever he makes a flurry of blows. This stacks with the first attack from this ability and additional attacks from haste and similar effects.",
                                                    null,
                                                    FeatureGroup.None,
                                                    Helpers.Create<SpecificWeaponGroupOrFeralCombatFeatureUnlock>(s =>
                                                    {
                                                        s.m_NewFact = flurry1_monk.ToReference<BlueprintUnitFactReference>();
                                                        s.weapon_groups = new WeaponFighterGroup[] { WeaponFighterGroup.Monk, WeaponFighterGroup.Close };
                                                    })
                                                    );
            brawlers_flurry.Ranks = flurry1_monk.Ranks;
            brawlers_flurry11 = CommonHelpers.CreateFeature("RMBrawlersFlurry11Unlock",
                                                    "Brawler’s Flurry",
                                                    "At 2nd level, a brawler can make a flurry of blows as a full attack. When making a flurry of blows, the brawler can make one additional attack at his highest base attack bonus. This additional attack stacks with the bonus attacks from haste and other similar effects. When using this ability, the brawler can make these attacks with any combination of his unarmed strikes, weapons from the close fighter weapon group, or weapons with thee monk special weapon quality. He takes no penalty for using multiple weapons when making a flurry of blows, but he does not gain any additional attacks beyond what's already granted by the flurry for doing so. (He can still gain additional attacks from a high base attack bonus, from this ability, and from haste and similar effects).\nAt 11th level, a brawler can make an additional attack at his highest base attack bonus whenever he makes a flurry of blows. This stacks with the first attack from this ability and additional attacks from haste and similar effects.",
                                                    null,
                                                    FeatureGroup.None,
                                                    Helpers.Create<SpecificWeaponGroupOrFeralCombatFeatureUnlock>(s =>
                                                    {
                                                        s.m_NewFact = flurry11_monk.ToReference<BlueprintUnitFactReference>();
                                                        s.weapon_groups = new WeaponFighterGroup[] { WeaponFighterGroup.Monk, WeaponFighterGroup.Close };
                                                    })
                                                    );
            brawlers_flurry11.Ranks = flurry11_monk.Ranks;
            var buff = Helpers.CreateBuff("RMBrawlerTwoWeaponFlurryEnabledBuff", bp =>
            {
                bp.SetName("Brawler’s Flurry: Use Off-Hand attacks");
                bp.SetDescription(brawlers_flurry.Description);
                bp.m_Icon = Helpers.GetIcon("ac8aaf29054f5b74eb18f2af950e752d");
                bp.AddComponent(Helpers.Create<AddBrawlerFlurryOnActivation>());
            } );

            var toggle = Helpers.CreateBlueprint<BlueprintActivatableAbility>("RMBrawlerTwoWeaponFlurryActivatable", bp => {
                bp.SetNameDescription(buff);
                bp.m_Icon = buff.m_Icon;
                bp.m_ActivateWithUnitCommand = CommandType.Free;
                bp.ActivationType = AbilityActivationType.Immediately;
                bp.DeactivateImmediately = false;
                bp.Components = buff.Components;
            });
                
                
 

            brawlers_flurry.AddComponents(Helpers.Create<AddBrawlerPartUnitFact>(a => a.groups = new WeaponFighterGroup[] { WeaponFighterGroup.Monk, WeaponFighterGroup.Close }),
                              Helpers.Create<AddBrawlerFlurryExtraAttacks>(),
                             Helpers.Create<AddFacts>(c => {
                                 c.m_Facts = new BlueprintUnitFactReference[]
                                 {
                                     toggle.ToReference<BlueprintUnitFactReference>()
                                 };
                             }));
            brawlers_flurry11.AddComponents(Helpers.Create<AddBrawlerFlurryExtraAttacks>());

            brawlers_flurry.IsClassFeature = true;
            brawlers_flurry11.IsClassFeature = true;
        }



        public class UnitPartBrawler : UnitPart
        {
            [JsonProperty]
            private bool active;
            [JsonProperty]
            private int extra_attacks = 0;
            [JsonProperty]
            private WeaponFighterGroup[] weapon_groups;

            public void Initialize(params WeaponFighterGroup[] brawler_weapon_groups)
            {
                weapon_groups = brawler_weapon_groups;
            }

            public bool IsActive()
            {
                return active;
            }

            public void Activate()
            {
                active = true;
                EventBus.RaiseEvent<IUnitActiveEquipmentSetHandler>((Action<IUnitActiveEquipmentSetHandler>)(h => h.HandleUnitChangeActiveEquipmentSet(this.Owner)));
            }

            public void Deactivate()
            {
                active = false;
                EventBus.RaiseEvent<IUnitActiveEquipmentSetHandler>((Action<IUnitActiveEquipmentSetHandler>)(h => h.HandleUnitChangeActiveEquipmentSet(this.Owner)));
            }

            public void IncreaseExtraAttacks()
            {
                extra_attacks++;
            }

            public void DecreaseExtraAttacks()
            {
                extra_attacks--;
            }

            public int GetNumExtraAttacks()
            {
                return extra_attacks;
            }


            public bool CheckTwoWeaponFlurry()
            {
                if (!IsActive())
                {
                    return false;
                }
                if (!Owner.Body.PrimaryHand.HasWeapon || !Owner.Body.SecondaryHand.HasWeapon)
                {
                    return false;
                }
                var weapon1 = Owner.Body.PrimaryHand.MaybeWeapon;
                var weapon2 = Owner.Body?.SecondaryHand?.MaybeWeapon;
                if (weapon_groups == null)
                {
                    Initialize(new WeaponFighterGroup[] { WeaponFighterGroup.Monk , WeaponFighterGroup.Close });
                }

                foreach (var group in weapon_groups)
                {
                    if(weapon1.Blueprint.FighterGroup.Contains(group) && weapon2.Blueprint.FighterGroup.Contains(group))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }



        //fix twf to work correctly with and brawlers flurry
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(TwoWeaponFightingAttacks))]
        [HarmonyPatch("OnEventAboutToTrigger", MethodType.Normal)]
        //[HarmonyPatch(new Type[] { typeof(RuleCalculateAttacksCount) })]
        class TwoWeaponFightingAttacks__OnEventAboutToTrigger__Patch
        {
            static bool Prefix(TwoWeaponFightingAttacks __instance, RuleCalculateAttacksCount evt)
            {
        
                var maybeWeapon1 = evt.Initiator.Body.PrimaryHand?.MaybeWeapon;
                var maybeWeapon2 = evt.Initiator.Body.SecondaryHand?.MaybeWeapon;
                if (!evt.Initiator.Body.PrimaryHand.HasWeapon
                    || !evt.Initiator.Body.SecondaryHand.HasWeapon
                       || (maybeWeapon1.Blueprint.IsNatural && (!maybeWeapon1.Blueprint.IsUnarmed ))
                       || (maybeWeapon2.Blueprint.IsNatural && (!maybeWeapon2.Blueprint.IsUnarmed ))
                    )
                    return true; //let original function work?

                
                var brawler_part = evt.Initiator.Get<UnitPartBrawler>();
             
                if (brawler_part != null && (brawler_part.CheckTwoWeaponFlurry()))
                {
                  
                    for (int i = 1; i < brawler_part.GetNumExtraAttacks(); i++)
                    {
                      
                        ++evt.Result.SecondaryHand.AdditionalAttacks;
                    }
               
                }
                else if (__instance.Fact.GetRank() > 1)
                {
                   
                    for (int i = 2; i < __instance.Fact.GetRank(); i++)
                    {
                    
                        ++evt.Result.SecondaryHand.PenalizedAttacks;
                    }
                }
         
                return true;
            }
        }


        [AllowedOn(typeof(BlueprintUnitFact))]
        public class AddBrawlerTwoWeaponFlurryPart : OldStyleUnitPart, IUnitSubscriber
        {
            public WeaponFighterGroup[] groups;

            public void Add(EntityFact fact)
            {
                fact.Owner.Ensure<UnitPartBrawler>().Initialize(groups);
            }


            public void Remove(EntityFact fact)
            {
                fact.Owner.Remove<UnitPartBrawler>();
            }
        }


        [AllowedOn(typeof(BlueprintUnitFact))]
        public class BrawlerTwoWeaponFlurryExtraAttack : OldStyleUnitPart, IUnitSubscriber
        {
            public void Add(EntityFact fact)
            {
                fact.Owner.Get<UnitPartBrawler>()?.IncreaseExtraAttacks();
            }


            public void Remove(EntityFact fact)
            {
                fact.Owner.Get<UnitPartBrawler>()?.DecreaseExtraAttacks();
            }
        }


        [AllowedOn(typeof(BlueprintUnitFact))]
        public class ActivateBrawlerTwoWeaponFlurry : OldStyleUnitPart, IUnitSubscriber
        {

            public void Add(EntityFact fact)
            {
                fact.Owner.Get<UnitPartBrawler>()?.Activate();
            }


            public void Remove(EntityFact fact)
            {
                fact.Owner.Get<UnitPartBrawler>()?.Deactivate();
            }
        }
    }
}
