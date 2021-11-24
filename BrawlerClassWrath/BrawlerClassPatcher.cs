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
using System.Collections.Generic;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Buffs.Components;

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

        static public BlueprintArchetype wild_child;
        static public BlueprintFeatureSelection animal_companion;
        static public BlueprintFeatureSelection wildchild_mountedcombat_feats;

        static public BlueprintArchetype venomfist;
        static public BlueprintFeature[] venomous_strike = new BlueprintFeature[5];

        static public BlueprintArchetype snakebite_striker;
        static public BlueprintFeature opportunist;


        static public BlueprintArchetype steel_breaker;
        static public BlueprintFeature exploit_weakness;
        static public BlueprintFeature sunder_training;
        static public BlueprintFeature disarm_training;


        static public BlueprintArchetype turfer;
        static public BlueprintFeatureSelection favorite_turf_selection;
        static public BlueprintFeature[] favored_turf = new BlueprintFeature[6];
        static public BlueprintFeature terrain_mastery;

        static public BlueprintArchetype beastwrestler;
        static public BlueprintFeatureSelection beast_training_selection;
        static public BlueprintFeature[] beast_training = new BlueprintFeature[12];
        static public BlueprintFeature beast_defences;


        static public BlueprintArchetype exemplar;
        static public BlueprintFeature call_to_arms;
        static public BlueprintFeature inspire_courage;
        static public BlueprintFeature inspire_greatness;
        static public BlueprintFeature inspire_heroics;
        static public BlueprintFeature field_instruction;
        static public BlueprintAbility field_instruction_ability;
        static public BlueprintFeature performance_resource_feature;
        static public BlueprintFeature inspiring_prowess;


        static public BlueprintArchetype mutagenic_mauler;
        static public BlueprintFeature mutagen;
        static public BlueprintFeature mutagen_damage_bonus;
        static public BlueprintFeatureSelection discovery;
        static public BlueprintFeature greater_mutagen;
        static public BlueprintFeature beastmorph_speed;
        static public BlueprintFeature beastmorph_blindsense;

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

            createWildChild();
            createSnakebiteStriker();
            createSteelBreaker();
            createVenomfist();
            createTurfer();
            createBeastWrestler();
            createExemplar();
            createMutagenicMauler();

            BrawlerClass.m_Archetypes = new BlueprintArchetypeReference[] { wild_child.ToReference<BlueprintArchetypeReference>(),
                venomfist.ToReference<BlueprintArchetypeReference>(),
                snakebite_striker.ToReference<BlueprintArchetypeReference>(),
            steel_breaker.ToReference<BlueprintArchetypeReference>(),
            turfer.ToReference<BlueprintArchetypeReference>(),
            beastwrestler.ToReference<BlueprintArchetypeReference>(),
            exemplar.ToReference<BlueprintArchetypeReference>(),
            mutagenic_mauler.ToReference<BlueprintArchetypeReference>()};
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
            unarmed_strike.SetNameDescription("Unarmed Strike", "At 1st level, a brawler gains Improved Unarmed Strike as a bonus feat. The damage dealt by a Medium brawler's unarmed strike increases with level: 1d6 at levels 1–3, 1d8 at levels 4–7, 1d10 at levels 8–11, 2d6 at levels 12–15, 2d8 at levels 16–19, and 2d10 at level 20.\nIf the brawler is Small, his unarmed strike damage increases as follows: 1d4 at levels 1–3, 1d6 at levels 4–7, 1d8 at levels 8–11, 1d10 at levels 12–15, 2d6 at levels 16–19, and 2d8 at level 20.\nIf the brawler is Large, his unarmed strike damage increases as follows: 1d8 at levels 1–3, 2d6 at levels 4–7, 2d8 at levels 8–11, 3d6 at levels 12–15, 3d8 at levels 16–19, and 4d8 at level 20.");
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
            brawlers_strike_alignment.m_AllFeatures = new BlueprintFeatureReference[0];
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
                                                    "At 2nd level, a brawler can make a flurry of blows as a full attack. When making a flurry of blows, the brawler can make one additional attack at his highest base attack bonus. This additional attack stacks with the bonus attacks from haste and other similar effects. When using this ability, the brawler can make these attacks with any combination of his unarmed strikes, weapons from the close fighter weapon group, or weapons with the monk special weapon quality. He takes no penalty for using multiple weapons when making a flurry of blows, but he does not gain any additional attacks beyond what's already granted by the flurry for doing so. (He can still gain additional attacks from a high base attack bonus, from this ability, and from haste and similar effects).\nAt 11th level, a brawler can make an additional attack at his highest base attack bonus whenever he makes a flurry of blows. This stacks with the first attack from this ability and additional attacks from haste and similar effects.",
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
                                                    "At 2nd level, a brawler can make a flurry of blows as a full attack. When making a flurry of blows, the brawler can make one additional attack at his highest base attack bonus. This additional attack stacks with the bonus attacks from haste and other similar effects. When using this ability, the brawler can make these attacks with any combination of his unarmed strikes, weapons from the close fighter weapon group, or weapons with the monk special weapon quality. He takes no penalty for using multiple weapons when making a flurry of blows, but he does not gain any additional attacks beyond what's already granted by the flurry for doing so. (He can still gain additional attacks from a high base attack bonus, from this ability, and from haste and similar effects).\nAt 11th level, a brawler can make an additional attack at his highest base attack bonus whenever he makes a flurry of blows. This stacks with the first attack from this ability and additional attacks from haste and similar effects.",
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
                bp.SetName("Toggle Brawler's Flurry");
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

        // ARCHETYPES
        // WILD CHILD

        static void createWildChild()
        {
            wild_child = Helpers.CreateBlueprint<BlueprintArchetype>("RMWildChild", a =>
            {
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "Wild Child");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "The wild child works with his sworn animal friend to conquer the challenges that lay before them. This kinship could come from being lost in the wilderness and raised by animals or growing up with an exotic pet.");
            });
            wild_child.m_ParentClass = BrawlerClass;

            createAnimalCompanion();
            createWildChildMounted();

            wild_child.RemoveFeatures = new LevelEntry[]
            {
                Helpers.LevelEntry(1, combat_feat),
                Helpers.LevelEntry(4, combat_feat),
                Helpers.LevelEntry(6, combat_feat),
                Helpers.LevelEntry(10, combat_feat),
                Helpers.LevelEntry(12, combat_feat),
                Helpers.LevelEntry(16, combat_feat),
                Helpers.LevelEntry(18, combat_feat),
            };

            wild_child.AddFeatures = new LevelEntry[] {Helpers.LevelEntry(1, animal_companion, wildchild_mountedcombat_feats),
                                                             Helpers.LevelEntry(2, wildchild_mountedcombat_feats),
                                                             Helpers.LevelEntry(6, wildchild_mountedcombat_feats),
                                                             Helpers.LevelEntry(10, wildchild_mountedcombat_feats),
                                                             Helpers.LevelEntry(14, wildchild_mountedcombat_feats),
                                                             Helpers.LevelEntry(18, wildchild_mountedcombat_feats)
                                                            };

            brawler_progression.m_UIDeterminatorsGroup = brawler_progression.m_UIDeterminatorsGroup.AddToArray(animal_companion.ToReference<BlueprintFeatureBaseReference>());
            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(wildchild_mountedcombat_feats));
            wild_child.ReplaceClassSkills = true;
            wild_child.ClassSkills = BrawlerClass.ClassSkills.AddToArray(StatType.SkillLoreNature);
        }


        static void createWildChildMounted()
        {
            wildchild_mountedcombat_feats = Helpers.CreateBlueprint<BlueprintFeatureSelection>("RMWildChildMountedCombatFeats");
            var soheiMountedFeats = Resources.GetBlueprint<BlueprintFeatureSelection>("59bd6f915ba1dee44a8316f97fd51967");

            wildchild_mountedcombat_feats.SetName("Wild Child Bonus Feat");
            wildchild_mountedcombat_feats.SetDescription("At 1st level, 2nd level, and every 4 levels thereafter, a Wild Child can select a bonus {g|Encyclopedia:Feat}feat{/g}. These feats must be taken from the following list: Combat Reflexes, Dodge, Crane Style, Blind Fight, Improved {g|Encyclopedia:Initiative}Initiative{/g}\nAt 6th level, the following feats are added to the list: Improved Trip, Improved Disarm\nAt 10th level, the following feats are added to the list: Improved {g|Encyclopedia:Critical}Critical{/g}, Improved Blind Fight.\nA wild child need not have any of the prerequisites normally required for these feats to select them.");
            wildchild_mountedcombat_feats.IgnorePrerequisites = true;
            wildchild_mountedcombat_feats.Ranks = 1;
            wildchild_mountedcombat_feats.IsClassFeature = true;
            wildchild_mountedcombat_feats.m_Icon = soheiMountedFeats.m_Icon;
            wildchild_mountedcombat_feats.m_AllFeatures = soheiMountedFeats.m_AllFeatures;
            wildchild_mountedcombat_feats.m_Features = soheiMountedFeats.m_Features;


        }


        static void createAnimalCompanion()
        {
            var animal_companion_progression = Helpers.CreateBlueprint<BlueprintProgression>( "RMWildChildAnimalCompanionProgression");
            var huntsmasterCompanion = Resources.GetBlueprint<BlueprintProgression>("924fb4b659dcb4f4f906404ba694b690");


            var classRefs = new BlueprintProgression.ClassWithLevel[getBrawlerArray().Count()];

            for (var i = 0; i < classRefs.Length; i++)
            {
                classRefs[i] = Helpers.ClassToClassWithLevel(getBrawlerArray()[i]);
            }

            animal_companion_progression.m_Classes = classRefs;
            animal_companion_progression.LevelEntries = huntsmasterCompanion.LevelEntries;
            animal_companion_progression.Ranks = 1;
            animal_companion_progression.IsClassFeature = true;

            animal_companion = Helpers.CreateBlueprint<BlueprintFeatureSelection>("RMAnimalCompanionSelectionWildCHild");
            animal_companion.Ranks = 1;
            animal_companion.IsClassFeature = true;
            var huntsCompanionFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("2995b36659b9ad3408fd26f137ee2c67");
            animal_companion.m_AllFeatures = huntsCompanionFeat.m_AllFeatures;
            animal_companion.SetNameDescription(huntsCompanionFeat);
            animal_companion.SetComponents(huntsCompanionFeat.Components);
            animal_companion.SetDescription("At 1st level, a wild child forms a bond with a loyal companion that accompanies the wild child on his adventures. A wild child can begin play with any of the animals available to a druid. The wild child uses his brawler level as his effective druid level for determining the abilities of his animal companion.");
            var add_progression = Helpers.Create<AddFeatureOnApply>();
            add_progression.m_Feature = animal_companion_progression.ToReference<BlueprintFeatureReference>();
            animal_companion.ReplaceOneComponent<AddFeatureOnApply>(add_progression);
        }

        //venomous fist


        static void createVenomfist()
        {
            venomfist = Helpers.CreateBlueprint<BlueprintArchetype>("VenomfistBrawler", a =>
            {
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "Venomfist");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "Thanks to alchemical experiments and rigorous study of venomous creatures, a venomfist has toxic unarmed strikes.");
            });
            wild_child.m_ParentClass = BrawlerClass;

            createVenomousStrike();

            venomfist.RemoveFeatures = new LevelEntry[]
            {
                Helpers.LevelEntry(1, unarmed_strike),
                Helpers.LevelEntry(4, knockout, Resources.GetBlueprint<BlueprintFeature>("8267a0695a4df3f4ca508499e6164b98")),
                Helpers.LevelEntry(5, close_weapon_mastery),
                Helpers.LevelEntry(8, Resources.GetBlueprint<BlueprintFeature>("f790a36b5d6f85a45a41244f50b947ca")),
                Helpers.LevelEntry(12, Resources.GetBlueprint<BlueprintFeature>("b3889f445dbe42948b8bb1ba02e6d949")),
                Helpers.LevelEntry(16, Resources.GetBlueprint<BlueprintFeature>("078636a2ce835e44394bb49a930da230")),
                Helpers.LevelEntry(20, Resources.GetBlueprint<BlueprintFeature>("df38e56fa8b3f0f469d55f9aa26b3f5c")),
            };



        venomfist.AddFeatures = new LevelEntry[] {Helpers.LevelEntry(1, venomous_strike[0]),
                                                              Helpers.LevelEntry(4, venomous_strike[1]),
                                                              Helpers.LevelEntry(5, venomous_strike[2]),
                                                              Helpers.LevelEntry(10, venomous_strike[3]),
                                                              Helpers.LevelEntry(16, venomous_strike[4]),
                                                         };

            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(venomous_strike));
        }


        static void createVenomousStrike()
        {
            var fatigued = Resources.GetBlueprint<BlueprintBuff>("e6f2fc5d73d88064583cb828801212f4");
            var shaken = Resources.GetBlueprint<BlueprintBuff>("25ec6cb6ab1845c48a95f9c20b034220");
            var sickened = Resources.GetBlueprint<BlueprintBuff>("4e42460798665fd4cb9173ffa7ada323");
            var blinded = Resources.GetBlueprint<BlueprintBuff>("0ec36e7596a4928489d2049e1e1c76a7");
            var exhausted = Resources.GetBlueprint<BlueprintBuff>("46d1b9cc3d0fd36469a471b047d773a2");
            var staggered = Resources.GetBlueprint<BlueprintBuff>("df3950af5a783bd4d91ab73eb8fa0fd3");
            var dazed = CommonHelpers.dazed_non_mind_affecting;
            var stunned = Resources.GetBlueprint<BlueprintBuff>("09d39b38bb7c6014394b6daced9bacd3");

            var poison_stats = new StatType[] { StatType.Unknown, StatType.Constitution, StatType.Dexterity, StatType.Strength };

            List<BlueprintAbility> secondary_effect_toggle = new List<BlueprintAbility>();
            List<BlueprintAbility> poison_damage_type_toggle = new List<BlueprintAbility>();

            var secondary_effect_buff = CommonHelpers.CreateBuff("VenomfistSecondaryEffectBuff", 
                                                           "",
                                                           "",
                                                           null,
                                                           null,
                                                           CommonHelpers.CreateSpellDescriptor(SpellDescriptor.Poison));
            secondary_effect_buff.m_Flags = BlueprintBuff.Flags.HiddenInUi | BlueprintBuff.Flags.Harmful;


            var immune_to_secondary_condition_buff = CommonHelpers.CreateBuff("ImmuneToSecondaryVenomfistEffect",
                                                            "Secondary Venom Fist Effect Immunity",
                                                            "The creature is immune to the secondary effects of the venomfist’s poison for 24 hours.",
                                                            Helpers.GetIcon("b48b4c5ffb4eab0469feba27fc86a023"), //delay poison
                                                            null
                                                            );
            immune_to_secondary_condition_buff.m_Flags = BlueprintBuff.Flags.RemoveOnRest;

            var apply_secondary_effect = CommonHelpers.CreateConditional(CommonHelpers.createContextConditionHasFact(immune_to_secondary_condition_buff, has: false),
                                                                   CommonHelpers.createContextActionApplyChildBuff(secondary_effect_buff)
                                                                  );

            var apply_immunity_to_secondary_effect = CommonHelpers.CreateConditional(CommonHelpers.createContextConditionHasFact(immune_to_secondary_condition_buff, has: false),
                                                                               CommonHelpers.createContextActionApplyBuff(immune_to_secondary_condition_buff,
                                                                                                                    CommonHelpers.CreateContextDuration(1, DurationRate.Days),
                                                                                                                    dispellable: false)
                                                                               );

            DiceFormula[] diceFormulas = new DiceFormula[] {new DiceFormula(1, DiceType.D4),
                                                            new DiceFormula(1, DiceType.D6),
                                                            new DiceFormula(1, DiceType.D8),
                                                            new DiceFormula(1, DiceType.D10),
                                                            new DiceFormula(2, DiceType.D6),
                                                            new DiceFormula(2, DiceType.D8)};

            venomous_strike[0] = CommonHelpers.CreateFeature("VenomousStrike1Feature",
                                                       "Venomous Strike",
                                                       "A venomfist’s unarmed strikes deal damage as a creature one size category smaller (1d4 at first level for Medium venomfists). If she hits with her first unarmed strike in a round, the target must succeed at a Fortitude saving throw (DC = 10 + half the venomfist’s brawler level + her Constitution modifier) or take an additional amount of damage equal to the venomfist’s Constitution modifier. The venomfist is immune to this toxin.",
                                                       Helpers.GetIcon("c7773d1b408fea24dbbb0f7bf3eb864e"), //physical enchantment strength
                                                       FeatureGroup.None,
                                                      Helpers.Create<ContextWeaponDamageDiceReplacementWeaponCategory>(c =>
                                                      {
                                                          c.dice_formulas = diceFormulas;
                                                          c.value = Helpers.CreateContextValue(AbilityRankType.Default);
                                                          c.categories = new WeaponCategory[] {WeaponCategory.UnarmedStrike};
                                                      }),
                                                      Helpers.CreateContextRankConfig(baseValueType: ContextRankBaseValueType.ClassLevel,
                                                                                        type: AbilityRankType.Default,
                                                                                        progression: ContextRankProgression.StartPlusDivStep,
                                                                                        startLevel: 4,
                                                                                        stepLevel: 4,
                                                                                        classes: getBrawlerArray()),
                                                       CommonHelpers.CreateSpellDescriptor(SpellDescriptor.Poison)
                                                       );

            venomous_strike[1] = CommonHelpers.CreateFeature("VenomousStrike4Feature",
                                                       "Venomous Strike",
                                                       "At 4th level, a target that fails this save must succeed at a second saving throw 1 round later or take the same amount of damage again. This effect repeats as long as the target continues to fail its saving throws, to a maximum number of rounds equal to 1 plus 1 additional round for every 4 brawler levels the venomfist has. Unlike other poisons, multiple doses of a venomfist’s poison never stack; the more recent poison effect replaces the older one.",
                                                       Helpers.GetIcon("c7773d1b408fea24dbbb0f7bf3eb864e"), //physical enchantment strength
                                                       FeatureGroup.None
                                                       );

            venomous_strike[2] = CommonHelpers.CreateFeature("VenomousStrike5Feature",
                                                       "Venomous Strike",
                                                       "At 5th level, after the venomfist gets 8 hours of rest, she can choose a secondary effect for her venom to impose. She can choose fatigued, shaken, or sickened. A creature that fails its saving throw against her venom also gains the chosen condition until it succeeds at a save against the venom or until the venom’s duration ends. Once a creature succeeds at its save against the poison, it becomes immune to the secondary condition for 24 hours, but the attack still deals the extra damage.",
                                                       fatigued.Icon,
                                                       FeatureGroup.None
                                                       );

            venomous_strike[3] = CommonHelpers.CreateFeature("VenomousStrike10Feature",
                                                       "Venomous Strike",
                                                       "At 10th level, when the venomfist chooses the condition her venom imposes, she can also cause her venom to deal ability score damage each round instead of hit point damage. She chooses Strength, Dexterity, or Constitution, and her venom deals 1d3 points of ability score damage each round. In addition, she adds blinded, exhausted, and staggered to the list of secondary effects she can choose for her venom.",
                                                       exhausted.Icon,
                                                       FeatureGroup.None
                                                       );

            venomous_strike[4] = CommonHelpers.CreateFeature("VenomousStrike16Feature",
                                                       "Venomous Strike",
                                                       "At 16th level, the venomfist’s venom is particularly potent. If it fails the initial save, the target must succeed at two consecutive saves before being cured of the venom, though if the first save is successful, the secondary effect ends and the creature is immune to the secondary effects of the venomfist’s poison for 24 hours. In addition, the venomfist adds dazed and stunned to the list of secondary effects she can choose for her venom.",
                                                       dazed.Icon,
                                                       FeatureGroup.None
                                                       );

            var secondary_effect_buffs = new Dictionary<BlueprintBuff, BlueprintFeature>
            {
                {fatigued, venomous_strike[2]},
                {shaken, venomous_strike[2] },
                {sickened, venomous_strike[2] },
                {blinded, venomous_strike[3] },
                {exhausted, venomous_strike[3] },
                {staggered, venomous_strike[3] },
                {dazed, venomous_strike[4] },
                {stunned, venomous_strike[4] }
            };

            var remove_stat_buffs = Helpers.Create<ContextActionRemoveBuffs>(c => c.Buffs = new BlueprintBuff[0]);
            var stat_buff_resource = CommonHelpers.CreateAbilityResource("VenomousStrikeStatBuffResource", "", "", "", null);
            stat_buff_resource.SetFixedResource(1);

            venomous_strike[0].AddComponents(CommonHelpers.createContextCalculateAbilityParamsBasedOnClass(BrawlerClass, StatType.Constitution),
                                             Helpers.Create<RecalculateOnStatChange>(r => r.Stat = StatType.Constitution),
                                             Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel,
                                                                             classes: getBrawlerArray(),
                                                                             progression: ContextRankProgression.OnePlusDivStep,
                                                                             stepLevel: 4,
                                                                             type: AbilityRankType.DamageDice
                                                                             )
                                             );
            foreach (var s in poison_stats)
            {
                string stat_text = s == StatType.Unknown ? "HP" : s.ToString();
                var buff = CommonHelpers.CreateBuff(stat_text + "VenomousStrikeEffectBuff",
                                              "Venomous Strike Effect: " + stat_text + " Damage",
                                              venomous_strike[0].Description,
                                              venomous_strike[0].Icon,
                                              null,
                                              Helpers.Create<BuffPoisonDamage>(p =>
                                              {
                                                  if (s == StatType.Unknown)
                                                  {
                                                      p.ContextValue = Helpers.CreateContextDiceValue(DiceType.Zero, 0, Helpers.CreateContextValue(AbilityRankType.DamageBonus));
                                                  }
                                                  else
                                                  {
                                                      p.ContextValue = Helpers.CreateContextDiceValue(DiceType.D3, 1, 0);
                                                  }
                                                  p.Stat = s;
                                                  p.SaveType = SavingThrowType.Fortitude;
                                                  p.contextSuccesfullSaves = Helpers.CreateContextValue(AbilityRankType.StatBonus);
                                                  p.contextTicks = Helpers.CreateContextValue(AbilityRankType.DamageDice);
                                                  p.on_successful_save_action = Helpers.CreateActionList(CommonHelpers.createContextActionRemoveBuffFromCaster(secondary_effect_buff),
                                                                                                         apply_immunity_to_secondary_effect);
                                              }),
                                              Helpers.CreateContextRankConfig(ContextRankBaseValueType.FeatureList, type: AbilityRankType.StatBonus,
                                                                              progression: ContextRankProgression.BonusValue, stepLevel: 1, featureList: new BlueprintFeature[] { venomous_strike[4] }),
                                              Helpers.CreateContextRankConfig(ContextRankBaseValueType.StatBonus, type: AbilityRankType.DamageBonus,
                                                                              stat: StatType.Constitution, min: 0),
                                              Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, classes: getBrawlerArray(), type: AbilityRankType.DamageDice,
                                                                              progression: ContextRankProgression.OnePlusDivStep, stepLevel: 4),
                                              CommonHelpers.CreateAddFactContextActions(activated: new GameAction[]{ apply_secondary_effect }),
                                              CommonHelpers.CreateSpellDescriptor(SpellDescriptor.Poison)
                                              );
                buff.Stacking = StackingType.Replace;

                var apply_buff = CommonHelpers.createContextActionApplyBuff(buff, CommonHelpers.CreateContextDuration(Helpers.CreateContextValue(AbilityRankType.DamageDice)), dispellable: false);
                var apply_buff_saved = CommonHelpers.CreateActionSavingThrow(SavingThrowType.Fortitude, CommonHelpers.CreateConditionalSaved(apply_immunity_to_secondary_effect, apply_buff));
                var owner_buff = CommonHelpers.CreateBuff(stat_text + "VenomousStrikeBuff",
                                                    buff.Name,
                                                    venomous_strike[0].Description,
                                                    venomous_strike[0].Icon,
                                                    null
                                                    );




                if (s != StatType.Unknown)
                {
                    var check_apply_buff_saved = CommonHelpers.CreateConditional(CommonHelpers.createContextConditionCasterHasFact(owner_buff), apply_buff_saved);
                    venomous_strike[0].AddComponent(CommonHelpers.createAddInitiatorAttackWithWeaponTriggerWithCategory(Helpers.CreateActionList(check_apply_buff_saved),
                                                                                                 only_first_hit: true));
                }
                else
                {
                    var check_apply_buff_saved = CommonHelpers.CreateConditional(CommonHelpers.CreateConditionsCheckerOr(CommonHelpers.createContextConditionCasterHasFact(owner_buff),
                                                                                                             CommonHelpers.createContextConditionCasterHasFact(venomous_strike[3], has: false)
                                                                                                             ),
                                                                           apply_buff_saved);
                    venomous_strike[0].AddComponent(CommonHelpers.createAddInitiatorAttackWithWeaponTriggerWithCategory(Helpers.CreateActionList(check_apply_buff_saved),
                                                                                                 only_first_hit: true));
                }
                owner_buff.m_Flags = BlueprintBuff.Flags.StayOnDeath;
                remove_stat_buffs.Buffs = remove_stat_buffs.Buffs.AddToArray(owner_buff);

                var owner_ability = CommonHelpers.CreateAbility(stat_text + "VenomousStrikeAbility",
                                                          owner_buff.Name,
                                                          owner_buff.Description,
                                                          "",
                                                          owner_buff.Icon,
                                                          AbilityType.Extraordinary,
                                                          CommandType.Standard,
                                                          AbilityRange.Personal,
                                                          "",
                                                          "",
                                                          CommonHelpers.CreateRunActions(remove_stat_buffs,
                                                                                   CommonHelpers.createContextActionApplyBuff(owner_buff, CommonHelpers.CreateContextDuration(), dispellable: false, is_permanent: true)),
                                                          stat_buff_resource.CreateResourceLogic()
                                                          );
                owner_ability.m_IsFullRoundAction = true;
                owner_ability.setMiscAbilityParametersSelfOnly();
                poison_damage_type_toggle.Add(owner_ability);
            }

            var remove_secondary_effect_buffs = Helpers.Create<ContextActionRemoveBuffs>(c => c.Buffs = new BlueprintBuff[0]);
            var secondary_effect_resource = CommonHelpers.CreateAbilityResource("VenomousStrikeSecondaryEffectResource", "", "", "", null);
            secondary_effect_resource.SetFixedResource(1);

            foreach (var b in secondary_effect_buffs)
            {
                var owner_buff = CommonHelpers.CreateBuff("VenomousStrike" + b.Key.name,
                                                    "Venomous Strike: " + b.Key.Name,
                                                    venomous_strike[2].Description,
                                                    b.Key.Icon,
                                                    null
                                                    );
                owner_buff.m_Flags = BlueprintBuff.Flags.StayOnDeath;
                remove_secondary_effect_buffs.Buffs = remove_secondary_effect_buffs.Buffs.AddToArray(owner_buff);

                var owner_ability = CommonHelpers.CreateAbility(b.Key.name + "VenomousStrikeAbility",
                                                          owner_buff.Name,
                                                          owner_buff.Description,
                                                          "",
                                                          owner_buff.Icon,
                                                          AbilityType.Extraordinary,
                                                          CommandType.Standard,
                                                          AbilityRange.Personal,
                                                          "",
                                                          "",
                                                          CommonHelpers.CreateRunActions(remove_secondary_effect_buffs,
                                                                                   CommonHelpers.createContextActionApplyBuff(owner_buff, CommonHelpers.CreateContextDuration(), dispellable: false, is_permanent: true)),
                                                          secondary_effect_resource.CreateResourceLogic(),
                                                          CommonHelpers.createContextCalculateAbilityParamsBasedOnClass(BrawlerClass, StatType.Constitution),
                                                          CommonHelpers.createAbilityShowIfCasterHasFact(b.Value)
                                                          );
                owner_ability.m_IsFullRoundAction = true;
                owner_ability.setMiscAbilityParametersSelfOnly();
                secondary_effect_toggle.Add(owner_ability);

                CommonHelpers.addContextActionApplyBuffOnConditionToActivatedAbilityBuffNoRemove(secondary_effect_buff,
                                                                                          CommonHelpers.CreateConditional(CommonHelpers.createContextConditionCasterHasFact(owner_buff),
                                                                                                                    CommonHelpers.createContextActionApplyChildBuff(b.Key)
                                                                                                                   )
                                                                                         );
            }

            var secondary_effects_wrapper = CommonHelpers.createVariantWrapper("VenomousStrikeSecondaryEffectBaseAbility", "", secondary_effect_toggle.ToArray());
            secondary_effects_wrapper.SetNameDescriptionIcon("Venomous Strike Secondary Effect",
                                                             venomous_strike[2].Description,
                                                             Helpers.GetIcon("d797007a142a6c0409a74b064065a15e") //poison
                                                             );

            venomous_strike[2].AddComponents(CommonHelpers.CreateAddFact(secondary_effects_wrapper),
                                            secondary_effect_resource.CreateAddAbilityResource()
                                            );

            var stat_damage_wrapper = CommonHelpers.createVariantWrapper("VenomousStrikeStatDamageBase", "", poison_damage_type_toggle.ToArray());
            stat_damage_wrapper.SetNameDescriptionIcon("Venomous Strike: Stat Damage",
                                                       "At 10th level, when the venomfist chooses the condition her venom imposes, she can also cause her venom to deal ability score damage each round instead of hit point damage. She chooses Strength, Dexterity, or Constitution, and her venom deals 1d3 points of ability score damage each round.",
                                                       Helpers.GetIcon("fd101fbc4aacf5d48b76a65e3aa5db6d")
                                                       );

            venomous_strike[3].AddComponents(CommonHelpers.CreateAddFact(stat_damage_wrapper),
                                            stat_buff_resource.CreateAddAbilityResource());
        }

        // snakebite striker
        static void createSnakebiteStriker()
        {
            snakebite_striker = Helpers.CreateBlueprint<BlueprintArchetype>("SnakebiteStriker", a =>
            {
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "Snakebite Striker");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "With her lightning quickness and guile, a snakebite striker keeps her foes’ attention focused on her, because any one of her feints might be an actual attack. By giving up some of a brawler’s versatility, she increases her damage potential and exposes opponents to deadly and unexpected strikes.");
            });
            snakebite_striker.m_ParentClass = BrawlerClass;
 

            createOpportunist();

            var sneak_attack = Resources.GetBlueprint<BlueprintFeature>("9b9eac6709e1c084cb18c3a366e0ec87");
            snakebite_striker.RemoveFeatures = new LevelEntry[]
            {
                Helpers.LevelEntry(2, combat_feat),
                Helpers.LevelEntry(3, maneuver_training[0]),
                Helpers.LevelEntry(6, combat_feat),
                Helpers.LevelEntry(7, maneuver_training[1]),
                Helpers.LevelEntry(10, combat_feat),
                Helpers.LevelEntry(11, maneuver_training[2]),
                Helpers.LevelEntry(14, combat_feat),
                Helpers.LevelEntry(15, maneuver_training[3]),
                Helpers.LevelEntry(18, combat_feat),
                Helpers.LevelEntry(19, maneuver_training[4]),
            };

            snakebite_striker.AddFeatures = new LevelEntry[] {Helpers.LevelEntry(2, sneak_attack),
                                                              Helpers.LevelEntry(3, Resources.GetBlueprint<BlueprintFeature>("14a1fc1356df9f146900e1e42142fc9d")),
                                                              Helpers.LevelEntry(6, sneak_attack),
                                                              Helpers.LevelEntry(7, Resources.GetBlueprint<BlueprintFeature>("52913092cd018da47845f36e6fbe240f")),
                                                              Helpers.LevelEntry(10, sneak_attack),
                                                              Helpers.LevelEntry(11, opportunist, Resources.GetBlueprint<BlueprintFeature>("e2d1fa11f6b095e4fb2fd1dcf5e36eb3")),
                                                              Helpers.LevelEntry(14, sneak_attack),
                                                              Helpers.LevelEntry(15, Resources.GetBlueprint<BlueprintFeature>("6ce0dd0cd1ef43eda9e62cdf483e05c3")),
                                                              Helpers.LevelEntry(18, sneak_attack),
                                                             };


            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(sneak_attack));
            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(Resources.GetBlueprint<BlueprintFeature>("14a1fc1356df9f146900e1e42142fc9d"),
                Resources.GetBlueprint<BlueprintFeature>("52913092cd018da47845f36e6fbe240f"),
                opportunist,
                Resources.GetBlueprint<BlueprintFeature>("e2d1fa11f6b095e4fb2fd1dcf5e36eb3"),
                Resources.GetBlueprint<BlueprintFeature>("6ce0dd0cd1ef43eda9e62cdf483e05c3")
                ));

            snakebite_striker.ReplaceClassSkills = true;
            snakebite_striker.ClassSkills = BrawlerClass.ClassSkills.AddToArray(StatType.SkillStealth);
        }



        static void createOpportunist()
        {
            opportunist = CommonHelpers.CreateFeature("SnakebiteStrikerOpportunist",
                "Snakebite Opportunist",
                "At 11th level, once per round the snakebite striker can make an attack of opportunity against an opponent who has just been struck for damage in melee by another character. This attack counts as an attack of opportunity for that round. She cannot use this ability more than once per round, even if she has the Combat Reflexes feat or a similar ability. At 19th level, she can use this ability twice per round.",
                Resources.GetBlueprint<BlueprintFeature>("5bb6dc5ce00550441880a6ff8ad4c968").Icon, 
                FeatureGroup.None);

            opportunist.ComponentsArray = new BlueprintComponent[]
            {
                Helpers.Create<OpportunistMultipleAttacks>(o => o.num_extra_attacks = Helpers.CreateContextValue(AbilityRankType.Default)),
                Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, classes: getBrawlerArray(),
                                                progression: ContextRankProgression.DelayedStartPlusDivStep,
                                                startLevel: 19,
                                                stepLevel: 100)
            };
        }


        // STEEL-BREAKER

        static void createSteelBreaker()
        {
            steel_breaker = Helpers.CreateBlueprint<BlueprintArchetype>("SteelBreakerBrawler", a =>
            {
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "Steel-Breaker");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "The steel-breaker studies destruction and practices it as an art form. She knows every defense has a breaking point, and can shatter those defenses with carefully planned strikes.");
            });
            steel_breaker.m_ParentClass = BrawlerClass;

            createExploitWeakness();
            createSunderAndDisarmTraining();

            steel_breaker.RemoveFeatures = new LevelEntry[]
            {
                Helpers.LevelEntry(3, maneuver_training[0]),
                Helpers.LevelEntry(5, brawlers_strike_magic),
                Helpers.LevelEntry(7, maneuver_training[1]),
                Helpers.LevelEntry(9, brawlers_strike_cold_iron_and_silver),
                Helpers.LevelEntry(11, maneuver_training[2]),
                Helpers.LevelEntry(12, brawlers_strike_alignment),
                Helpers.LevelEntry(15, maneuver_training[3]),
                Helpers.LevelEntry(17, brawlers_strike_adamantine),
                Helpers.LevelEntry(19, maneuver_training[4]),
            };

            steel_breaker.AddFeatures = new LevelEntry[] {Helpers.LevelEntry(3, sunder_training),
                                                              Helpers.LevelEntry(5, exploit_weakness),
                                                              Helpers.LevelEntry(7, disarm_training),
                                                         };

            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(sunder_training, exploit_weakness, disarm_training));
        }


        static void createExploitWeakness()
        {
            var description = "At 5th level, as a swift action a steel-breaker can observe a creature or object to find its weak point by succeeding at a Wisdom check, adding her brawler level against a DC of 10 + target’s HD. If it succeeds, the steel-breaker gains a +2 bonus on attack rolls until the end of her turn, and any attacks she makes until the end of her turn ignore the creature's  DR.\n"
                               + "A steel-breaker can instead use this ability as a swift action to analyze the movements and expressions of one creature within 30 feet, granting a bonus on Reflex saving throws, as well as a dodge bonus to AC against that opponent equal to 1/2 her brawler level until the start of her next turn.";

            var attack_buff = CommonHelpers.CreateBuff("SteelBreakerExplotWeaknessAttackBuff",
                                                 "Exploit Weakness: Attack Bonus",
                                                 description,
                                                 Helpers.GetIcon("2c38da66e5a599347ac95b3294acbe00"), //true strike
                                                 null,
                                                 Helpers.Create<IgnoreTargetDR>(i => i.CheckCaster = true),
                                                 Helpers.Create<AttackBonusAgainstTarget>(a => { a.CheckCaster = true; a.Value = 2; })
                                                 );

            var defense_buff = CommonHelpers.CreateBuff("SteelBreakerExplotWeaknessDefenseBuff",
                                     "Exploit Weakness: Defense Bonus",
                                     description,
                                     Helpers.GetIcon("9e1ad5d6f87d19e4d8883d63a6e35568"), //mage armor
                                     null,
                                     Helpers.Create<ACBonusAgainstTarget>(i => { i.CheckCaster = true; i.Value = Helpers.CreateContextValue(AbilityRankType.Default); i.Descriptor = ModifierDescriptor.Dodge; }),
                                     Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, classes: getBrawlerArray(), progression: ContextRankProgression.Div2)
                                     );

            exploit_weakness = CommonHelpers.CreateFeature("SteelBreakerExploitWeaknessFeature",
                                                     "Exploit Weakness",
                                                     description,
                                                     attack_buff.Icon,
                                                     FeatureGroup.None,
                                                     Helpers.Create<SavingThrowBonusAgainstFactFromCaster>(a =>
                                                     {
                                                         a.Value = Helpers.CreateContextValue(AbilityRankType.Default);
                                                         a.Descriptor = ModifierDescriptor.UntypedStackable;
                                                         a.reflex = true;
                                                         a.fortitude = false;
                                                         a.will = false;
                                                         a.CheckedFact = defense_buff;
                                                     }),
                                                     Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, classes: getBrawlerArray(), progression: ContextRankProgression.Div2)
                                                     );

            var buffs = new BlueprintBuff[] { attack_buff, defense_buff };

            foreach (var b in buffs)
            {
                b.Stacking = StackingType.Stack;
                var apply_buff = CommonHelpers.createContextActionApplyBuff(b, CommonHelpers.CreateContextDuration(1), dispellable: false);
                var check = Helpers.Create<ContextActionCasterSkillCheck>(c =>
                {
                    c.Stat = StatType.Wisdom;
                    c.Success = Helpers.CreateActionList(apply_buff);
                    c.bonus = Helpers.CreateContextValue(AbilityRankType.StatBonus);
                });
                var ability = CommonHelpers.CreateAbility(b.name + "Ability",
                                                    b.Name,
                                                    b.Description,
                                                    "",
                                                    b.Icon,
                                                    AbilityType.Extraordinary,
                                                    CommandType.Swift,
                                                    AbilityRange.Close,
                                                    CommonHelpers.roundsPerLevelDuration,
                                                    "",
                                                    CommonHelpers.CreateRunActions(check),
                                                    Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, classes: getBrawlerArray(),
                                                                                    type: AbilityRankType.StatBonus),
                                                    CommonHelpers.createAbilitySpawnFx("8de64fbe047abc243a9b4715f643739f", position_anchor: AbilitySpawnFxAnchor.None, orientation_anchor: AbilitySpawnFxAnchor.None)
                                                    );
                ability.setMiscAbilityParametersSingleTargetRangedHarmful(true);
                exploit_weakness.AddComponent(CommonHelpers.CreateAddFact(ability));
            }
        }


        static void createSunderAndDisarmTraining()
        {
            sunder_training = Helpers.CreateBlueprint<BlueprintFeature>("SteelBreakerSunderTraining");
            sunder_training.Ranks = 1;
            sunder_training.IsClassFeature = true;
            sunder_training.SetComponents(maneuver_training[0].AllFeatures[3].ComponentsArray);
            sunder_training.SetNameDescription("Sunder Training",
                                               "At 3rd level, a steel-breaker receives additional training in sunder combat maneuvers. She gains a +2 bonus when attempting a sunder combat maneuver checks and a +2 bonus to her CMD when defending against this maneuver.\n"
                                               + "At 7th, 11th, 15th, and 19th levels, these bonuses increase by 1.");
            var oldComponent = sunder_training.GetComponent<ContextRankConfig>();
            sunder_training.ReplaceComponent(oldComponent, Helpers.Create<ContextRankConfig>(c => {
                c.m_StartLevel = -1;
                
                }));


            disarm_training = Helpers.CreateBlueprint<BlueprintFeature>("SteelBreakerDisarmTraining");
            disarm_training.Ranks = 1;
            disarm_training.IsClassFeature = true;
            disarm_training.SetComponents(maneuver_training[0].AllFeatures[1].ComponentsArray);
            disarm_training.SetNameDescription("Disarm Training",
                                               "At 7th level, a steel-breaker receives additional training in disarm combat maneuvers. She gains a +2 bonus when attempting a disarm combat maneuver checks and a +2 bonus to her CMD when defending against this maneuver.\n"
                                               + "At 11th, 15th, and 19th levels, these bonuses increase by 1.");
        }


        // TURFER

        static void createTurfer()
        {
            turfer = Helpers.CreateBlueprint<BlueprintArchetype>("TurferBrawler", a =>
            {
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "Turfer");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "A turfer has a mastery over particular types of terrain, and takes advantage of that in fights against their enemies.");
            });
            turfer.m_ParentClass = BrawlerClass;

            createFavouriteTurfs();
            createFavouriteTurfSelection();
            createTerrainMastery();

            turfer.RemoveFeatures = new LevelEntry[]
            {
                Helpers.LevelEntry(3, maneuver_training[0]),
                Helpers.LevelEntry(4, knockout),
                Helpers.LevelEntry(7, maneuver_training[1]),
                Helpers.LevelEntry(11, maneuver_training[2]),
                Helpers.LevelEntry(15, maneuver_training[3]),
                Helpers.LevelEntry(19, maneuver_training[4]),
            };

            turfer.AddFeatures = new LevelEntry[] {Helpers.LevelEntry(3, favorite_turf_selection),
                Helpers.LevelEntry(4, terrain_mastery),
               Helpers.LevelEntry(7, favorite_turf_selection),
               Helpers.LevelEntry(10, terrain_mastery),
                 Helpers.LevelEntry(11, favorite_turf_selection),
                Helpers.LevelEntry(15, favorite_turf_selection),
                Helpers.LevelEntry(16, terrain_mastery),
                Helpers.LevelEntry(19, favorite_turf_selection),
             };

            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(favorite_turf_selection, terrain_mastery));
        }

        static void createTerrainMastery()
        {
            var feat = CommonHelpers.CreateFeature($"RMTerrainMasteryFeature",
                                             "Terrain Mastery",
                                             "At 4th level, a turfer gains a +10 enhancement bonus to her base speed, as long as she's in one of her favored terrains. " +
                                            "At 10th and 16th levels, this enhancement bonus increases by 10 feet. " +
                                            "The turfer loses this ability when wearing heavy armour or carrying a heavy load.",
                                             Resources.GetBlueprint<BlueprintFeature>("4cd06a915daa74f4094952f2b3314b3b").Icon
                                             ,
                                             FeatureGroup.FavoriteTerrain,
                                             Helpers.Create<TerrainMastery>(c => {
                                                 c.maxLoad = Encumbrance.Medium;
                                                 c.required_armor = new ArmorProficiencyGroup[] {ArmorProficiencyGroup.Medium, ArmorProficiencyGroup.Light, ArmorProficiencyGroup.None };
                                             }));
            feat.Ranks = 3;
            terrain_mastery = feat;

        }

        static void createFavouriteTurfSelection()
        {

            var selection = CommonHelpers.CreateFeatureSelection("TurferFavoredTurfSelection",
                "Favoured Turf",
                 "At 3rd level, a turfer chooses a type of terrain from the ranger’s favored terrain list. When in that type of terrain, she gains a +2 bonus on initiative checks and a +1 bonus on combat maneuver checks and to CMD. " +
                                                "At 7th level and every 4 brawler levels thereafter, the turfer chooses an additional terrain in which to gain these bonuses. " +
                                                "Each time, in one selected terrain(including the one just chosen), her bonus on initiative checks increases by 2," +
                                                " and her bonus on combat maneuver checks and to CMD increases by 1.",
                 null,
                 FeatureGroup.FavoriteTerrain
                );

            selection.m_AllFeatures = new BlueprintFeatureReference[] {
                favored_turf[0].ToReference<BlueprintFeatureReference>(),
                favored_turf[1].ToReference<BlueprintFeatureReference>(),
                favored_turf[2].ToReference<BlueprintFeatureReference>(),
                favored_turf[3].ToReference<BlueprintFeatureReference>(),
                favored_turf[4].ToReference<BlueprintFeatureReference>(),
                favored_turf[5].ToReference<BlueprintFeatureReference>()
            };

            favorite_turf_selection = selection;
        }
        static void createFavouriteTurfs()
        {
            var terrains = new AreaSetting[] { AreaSetting.Abyss, AreaSetting.Desert, AreaSetting.Forest, AreaSetting.Highlands, AreaSetting.Underground, AreaSetting.Urban};

            

            var names = new string[] { "Abyss", "Desert", "Forest", "Highlands", "Underground", "Urban" };
            var icons = new UnityEngine.Sprite[]
            {
                Resources.GetBlueprint<BlueprintFeature>("b3f10ef830d9fc44eab628ca1c1ed4fb").Icon,
                Resources.GetBlueprint<BlueprintFeature>("fb164906bd9c9ff4c813167fcb9ae338").Icon,
                Resources.GetBlueprint<BlueprintFeature>("19e1c418cbad1b540ad52fee0fd7f16b").Icon,
                Resources.GetBlueprint<BlueprintFeature>("8dc89bc3543a8724895477cd1472f591").Icon,
                Resources.GetBlueprint<BlueprintFeature>("ab32f12f647277743bbaf6e791b38c3a").Icon,
                Resources.GetBlueprint<BlueprintFeature>("515cafe9efb8e1c48be3d6ec41bc23ef").Icon, 
            };

                for (int j = 0; j < terrains.Length; j++)
                {
                var feat = CommonHelpers.CreateFeature($"RMFavouriteTurf" + terrains[j].ToString() + "Feature",
                                                 "Favored Turf" + ": " + names[j],
                                                 "At 3rd level, a turfer chooses a type of terrain from the ranger’s favored terrain list. When in that type of terrain, she gains a +2 bonus on initiative checks and a +1 bonus on combat maneuver checks and to CMD." +
                                                " At 7th level and every 4 brawler levels thereafter, the turfer chooses an additional terrain in which to gain these bonuses. " +
                                                "Each time, in one selected terrain(including the one just chosen), her bonus on initiative checks increases by 2," +
                                                " and her bonus on combat maneuver checks and to CMD increases by 1.",
                                                 icons[j],
                                                 FeatureGroup.FavoriteTerrain,
                                                 Helpers.Create<FavoredTurf>(c => {
                                                     c.Setting = terrains[j];
                                                 }));

                favored_turf[j] = feat;
                }
            

        }
        // BEAST WRESTLER

        static void createBeastWrestler()
        {
            beastwrestler = Helpers.CreateBlueprint<BlueprintArchetype>("BeastWrestlerBrawler", a =>
            {
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "Beast-Wrestler");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "For these mighty grapplers, wrestling normal opponents has lost its challenge—they seek greater targets for glory. Beast-wrestlers challenge trolls to unarmed combat, and the greatest seek out the great linnorm their lands are known for and wrestle these primeval dragons into submission.");
            });
            beastwrestler.m_ParentClass = BrawlerClass;


            createBeastDefences();
            createBeastTrainings();
            createBeastTrainingSelection();


            beastwrestler.RemoveFeatures = new LevelEntry[]
            {
                Helpers.LevelEntry(3, maneuver_training[0]),
                Helpers.LevelEntry(4, ac_bonus),
                Helpers.LevelEntry(7, maneuver_training[1]),
                Helpers.LevelEntry(11, maneuver_training[2]),
                Helpers.LevelEntry(15, maneuver_training[3]),
                Helpers.LevelEntry(19, maneuver_training[4]),
            };

            beastwrestler.AddFeatures = new LevelEntry[] {Helpers.LevelEntry(3, beast_training_selection),
                Helpers.LevelEntry(4, beast_defences),
               Helpers.LevelEntry(7, beast_training_selection),
                 Helpers.LevelEntry(11, beast_training_selection),
                Helpers.LevelEntry(15, beast_training_selection),
                Helpers.LevelEntry(19, beast_training_selection),
             };

            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(beast_training_selection, beast_defences));
        }
        static void createBeastTrainingSelection()
        {
                var selection = CommonHelpers.CreateFeatureSelection("BeastWrestlerFavoredEnemySelection",
                    "Beast Training",
                     "At 3rd level, a beast-wrestler selects a creature type from the ranger favored enemies table. The only normal humanoid subtype the beast-wrestler can select is giant. She gains a +2 bonus on combat maneuver checks and to her CMD against creatures of the type she selected." +
                                                    "At 7th, 11th, 15th, and 19th levels, the beast - wrestler can select an additional type of creature. " +
                                                    "In addition, at each such interval the bonuses against any one type of creature she’s chosen (including the one just selected, if so desired) increase by 2. " +
                                                    "If a specific creature falls into more than one category, the beast-wrestler’s bonuses don’t stack; she simply uses whichever bonus is higher.",
                     null,
                     FeatureGroup.FavoriteTerrain
                    );

                selection.m_AllFeatures = new BlueprintFeatureReference[] {
                    beast_training[0].ToReference<BlueprintFeatureReference>(),
                    beast_training[1].ToReference<BlueprintFeatureReference>(),
                    beast_training[2].ToReference<BlueprintFeatureReference>(),
                    beast_training[3].ToReference<BlueprintFeatureReference>(),
                    beast_training[4].ToReference<BlueprintFeatureReference>(),
                    beast_training[5].ToReference<BlueprintFeatureReference>(),
                    beast_training[6].ToReference<BlueprintFeatureReference>(),
                    beast_training[7].ToReference<BlueprintFeatureReference>(),
                    beast_training[8].ToReference<BlueprintFeatureReference>(),
                    beast_training[9].ToReference<BlueprintFeatureReference>(),
                    beast_training[10].ToReference<BlueprintFeatureReference>(),
                    beast_training[11].ToReference<BlueprintFeatureReference>()
                };

                beast_training_selection = selection;
        }
        static void createBeastDefences()
        {
            beast_defences = CommonHelpers.CreateFeature("BeastDefencesRMFeature",
                                                    "Beast Defences",
                                                    "At 4th level, when facing enemies she selected with beast training, the beast-wrestler gains a bonus to AC equal to 1/2 her beast training bonus against that creature.",
                                                    Helpers.GetIcon("2a6a2f8e492ab174eb3f01acf5b7c90a"), //defensive stance
                                                    FeatureGroup.None
                                                    );
        }
        static void createBeastTrainings()
        {

            var names = new string[] { "Aberrations", "Animals", "Constructs", "Dragons", "Fey", "Giants", "MagicalBeasts", "MonstrousHumanoid", "Outsider", "Plant", "Undead", "Vermin" };
            var icons = new UnityEngine.Sprite[]
            {
                Resources.GetBlueprint<BlueprintFeature>("7081934ab5f8573429dbd26522adcc39").Icon,
                Resources.GetBlueprint<BlueprintFeature>("1ef8d7ab3ca4795498ff446cb027e2f3").Icon,
                Resources.GetBlueprint<BlueprintFeature>("6ea5a4a19ccb81a498e18a229cc5038a").Icon,
                Resources.GetBlueprint<BlueprintFeature>("918555c021b3a2944beed35df53b4c56").Icon,
                Resources.GetBlueprint<BlueprintFeature>("be3d454ea70a8bb468b0a8a087e7e65b").Icon, //fey
                Resources.GetBlueprint<BlueprintFeature>("bd59614d30bcadd46bd56aabe0de819f").Icon, // giants
                Resources.GetBlueprint<BlueprintFeature>("f807fac786faa86438428c79f5629654").Icon, //magical beasts
                Resources.GetBlueprint<BlueprintFeature>("0fd21e10dff071e4580ef9f30a0334df").Icon, //monstrous humanoids
                Resources.GetBlueprint<BlueprintFeature>("f643b38acc23e8e42a3ed577daeb6949").Icon,
                Resources.GetBlueprint<BlueprintFeature>("4ae78c44858bc1942934efe7c149d039").Icon,
                Resources.GetBlueprint<BlueprintFeature>("5941963eae3e9864d91044ba771f2cc2").Icon,
                Resources.GetBlueprint<BlueprintFeature>("f6dac9009747b91408644fa834dd0d99").Icon,
            };

            for (int j = 0; j < names.Length; j++)
            {
                var newName = names[j];
                if (names[j] == "MagicalBeasts" )
                {
                    newName = "Magical Beasts";
                }
                else if(names[j] == "MonstrousHumanoid")
                {
                    newName = "Monstrous Humanoids";
                }
                var feat = CommonHelpers.CreateFeature($"RMBeastTraining" + names[j] + "Feature",
                                                 "Beast Training" + ": " + newName,
                                                 "At 3rd level, a beast-wrestler selects a creature type from the ranger favored enemies table. The only normal humanoid subtype the beast-wrestler can select is giant. She gains a +2 bonus on combat maneuver checks and to her CMD against creatures of the type she selected." +
                                                "At 7th, 11th, 15th, and 19th levels, the beast - wrestler can select an additional type of creature. " +
                                                "In addition, at each such interval the bonuses against any one type of creature she’s chosen (including the one just selected, if so desired) increase by 2. " +
                                                "If a specific creature falls into more than one category, the beast-wrestler’s bonuses don’t stack; she simply uses whichever bonus is higher.",
                                                 icons[j],
                                                 FeatureGroup.FavoriteEnemy);

                switch (j)
                {
                    case 0: //aberrations
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("3bec99efd9a363242a6c8d9957b75e91").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("3bec99efd9a363242a6c8d9957b75e91").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 1: // animals
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("a95311b3dc996964cbaa30ff9965aaf6").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("a95311b3dc996964cbaa30ff9965aaf6").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 2: // constructs
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("fd389783027d63343b4a5634bd81645f").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("fd389783027d63343b4a5634bd81645f").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 3: // dragons
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("455ac88e22f55804ab87c2467deff1d6").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("455ac88e22f55804ab87c2467deff1d6").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 4: // fey
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("018af8005220ac94a9a4f47b3e9c2b4e").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("018af8005220ac94a9a4f47b3e9c2b4e").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 5: // giants
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("f9c388137f4faa74aac9065a68b56880").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("f9c388137f4faa74aac9065a68b56880").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 6: // magical beasts
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("625827490ea69d84d8e599a33929fdc6").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("625827490ea69d84d8e599a33929fdc6").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 7: // monstrous humanoid
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("57614b50e8d86b24395931fffc5e409b").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("57614b50e8d86b24395931fffc5e409b").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 8: // outsider
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("b7f02ba92b363064fb873963bec275ee").ToReference<BlueprintUnitFactReference>(), //aasimar
                                Resources.GetBlueprint<BlueprintFeature>("9054d3988d491d944ac144e27b6bc318").ToReference<BlueprintUnitFactReference>(),//outsiders
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c =>
                        {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("b7f02ba92b363064fb873963bec275ee").ToReference<BlueprintUnitFactReference>(), //aasimar
                                Resources.GetBlueprint<BlueprintFeature>("9054d3988d491d944ac144e27b6bc318").ToReference<BlueprintUnitFactReference>(),//outsiders
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 9: // plant
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("706e61781d692a042b35941f14bc41c5").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("706e61781d692a042b35941f14bc41c5").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 10: // undead
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("734a29b693e9ec346ba2951b27987e33").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    case 11: // vermin
                        feat.AddComponent(Helpers.Create<BeastTraining>(c => {
                            c.m_CheckedFacts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("09478937695300944a179530664e42ec").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                        }));
                        feat.AddComponent(Helpers.Create<BeastDefences>(c => {
                            c.m_Facts = new BlueprintUnitFactReference[]
                            {
                                Resources.GetBlueprint<BlueprintFeature>("09478937695300944a179530664e42ec").ToReference<BlueprintUnitFactReference>(),
                                Resources.GetBlueprint<BlueprintBuff>("82574f7d14a28e64fab8867fbaa17715").ToReference<BlueprintUnitFactReference>() //instant enemy buff
                            };
                            c.requiredFact = beast_defences.ToReference<BlueprintUnitFactReference>();
                        }));
                        break;
                    default:
                        break;
                }


                beast_training[j] = feat;
            }


        }


        // EXEMPLAR
        static void createExemplar()
        {

            exemplar =  Helpers.CreateBlueprint<BlueprintArchetype>("BrawlerExemplar", a =>
            {
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "Exemplar");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "A versatile soldier who inspires her companions with her fighting prowess, an exemplar is at home on the front lines of battles anywhere.");
            });
            exemplar.m_ParentClass = BrawlerClass;

            var improved_unarmed_strike = Resources.GetBlueprint<BlueprintFeature>("7812ad3672a4b9a4fb894ea402095167");
            createCallToArms();
            createInspiringProwess();
            createFieldInstruction();

            exemplar.RemoveFeatures = new LevelEntry[]
            {
                Helpers.LevelEntry(1, unarmed_strike, improved_unarmed_strike),
                Helpers.LevelEntry(3, maneuver_training[0]),
                Helpers.LevelEntry(4, ac_bonus),
                Helpers.LevelEntry(5, brawlers_strike_magic, close_weapon_mastery),
                Helpers.LevelEntry(7, maneuver_training[1]),
                Helpers.LevelEntry(9, brawlers_strike_cold_iron_and_silver),
                Helpers.LevelEntry(11, maneuver_training[2]),
                Helpers.LevelEntry(12, brawlers_strike_alignment),
                Helpers.LevelEntry(15, maneuver_training[3]),
                Helpers.LevelEntry(17, brawlers_strike_adamantine),
                Helpers.LevelEntry(19, maneuver_training[4]),
                Helpers.LevelEntry(20, perfect_warrior),
            };

            exemplar.AddFeatures = new LevelEntry[] {Helpers.LevelEntry(1, call_to_arms),
                                                     Helpers.LevelEntry(3, inspiring_prowess, inspire_courage, performance_resource_feature),
                                                     Helpers.LevelEntry(5, field_instruction),
                                                     Helpers.LevelEntry(11, inspire_greatness),
                                                     Helpers.LevelEntry(15, inspire_heroics)
                                                    };

            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(call_to_arms, inspiring_prowess, field_instruction) );
            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(inspire_courage, inspire_greatness, inspire_heroics));

                                                                                  
            exemplar.OverrideAttributeRecommendations = true;
            exemplar.RecommendedAttributes = BrawlerClass.RecommendedAttributes.AddToArray(StatType.Charisma);
        }


        static void createInspiringProwess()
        {
            var resource = Resources.GetBlueprint<BlueprintAbilityResource>("e190ba276831b5c4fa28737e5e49e6a6");
            resource.m_MaxAmount.m_Class = resource.m_MaxAmount.m_Class.AddToArray(BrawlerClass.ToReference<BlueprintCharacterClassReference>());
            resource.m_MaxAmount.m_Archetypes = resource.m_MaxAmount.m_Archetypes.AddToArray(exemplar.ToReference<BlueprintArchetypeReference>());
            //ClassToProgression.addClassToResource(brawler_class, new BlueprintArchetype[] { exemplar }, resource, library.Get<BlueprintCharacterClass>("772c83a25e2268e448e841dcd548235f"));

            var bardCourage = Resources.GetBlueprint<BlueprintActivatableAbility>("70274c5aa9124424c984217b62dabee8");


            var inspire_courage_ability = Helpers.CreateBlueprint<BlueprintActivatableAbility>("ExemplarInspireCourageToggleAbility", c=> {
                c.m_Buff = bardCourage.m_Buff;
                c.SetNameDescription(bardCourage);
                c.SetDescription("A 3rd level exemplar can use his inspiring prowess to inspire courage in his allies (including himself), bolstering them against fear and improving their combat abilities. To be affected, an ally must be able to perceive the evangelist's performance. An affected ally receives a +1 morale bonus on saving throws against charm and fear effects and a +1 competence bonus on attack and weapon damage rolls. At 7th level, and every six exemplar levels thereafter, this bonus increases by +1, to a maximum of +4 at 19th level.");
                c.m_ActivateWithUnitCommand = bardCourage.m_ActivateWithUnitCommand;
                c.m_SelectTargetAbility = bardCourage.m_SelectTargetAbility;
                c.Group = bardCourage.Group;
                c.WeightInGroup = bardCourage.WeightInGroup;
                c.DeactivateIfCombatEnded = true;
                c.DeactivateIfOwnerDisabled = true;
                c.ActivationType = AbilityActivationType.WithUnitCommand;
                c.ResourceAssetIds = bardCourage.ResourceAssetIds;
                c.m_Icon = bardCourage.m_Icon;
                c.SetComponents(bardCourage.Components);
            });

            var inspire_courage_buff = Resources.GetBlueprint<BlueprintBuff>("6d6d9e06b76f5204a8b7856c78607d5d ");

            var buffConfig = inspire_courage_buff.GetComponent<ContextRankConfig>();

            buffConfig.m_AdditionalArchetypes = buffConfig.m_AdditionalArchetypes.AddToArray(exemplar.ToReference<BlueprintArchetypeReference>());
            buffConfig.m_Class = buffConfig.m_Class.AddToArray(BrawlerClass.ToReference<BlueprintCharacterClassReference>());


            var bardGreatness = Resources.GetBlueprint<BlueprintActivatableAbility>("be36959e44ac33641ba9e0204f3d227b");

            var inspire_greatness_ability = Helpers.CreateBlueprint<BlueprintActivatableAbility>("ExemplarInspireGreatnessToggleAbility", c => {
                c.m_Buff = bardGreatness.m_Buff;
                c.SetNameDescription(bardGreatness);
                c.SetDescription("An exemplar of 11th level or higher can use his inspiring prowess to inspire greatness in all allies within 30 feet, granting extra fighting capability. A creature inspired with greatness gains 2 bonus Hit Dice (d10s), the commensurate number of temporary hit points (apply the target's Constitution modifier, if any, to these bonus Hit Dice), a +2 competence bonus on attack rolls, and a +1 competence bonus on Fortitude saves.");
                c.m_ActivateWithUnitCommand = bardGreatness.m_ActivateWithUnitCommand;
                c.m_SelectTargetAbility = bardGreatness.m_SelectTargetAbility;
                c.Group = bardGreatness.Group;
                c.WeightInGroup = bardGreatness.WeightInGroup;
                c.DeactivateIfCombatEnded = true;
                c.DeactivateIfOwnerDisabled = true;
                c.ActivationType = AbilityActivationType.WithUnitCommand;
                c.ResourceAssetIds = bardGreatness.ResourceAssetIds;
                c.m_Icon = bardGreatness.m_Icon;
                c.SetComponents(bardGreatness.Components);
            });


            var bardHeroics = Resources.GetBlueprint<BlueprintActivatableAbility>("a4ce06371f09f504fa86fcf6d0e021e4");
            var inspire_heroics_ability = Helpers.CreateBlueprint<BlueprintActivatableAbility>("ExemplarInspireHeroicsToggleAbility", c => {
                c.m_Buff = bardHeroics.m_Buff;
                c.SetNameDescription(bardHeroics);
                c.SetDescription("An exemplar of 15th level or higher can inspire tremendous heroism in all allies within 30 feet. Inspired creatures gain a +4 morale bonus on saving throws and a +4 dodge bonus to AC. The effect lasts for as long as the targets are able to witness the performance.");
                c.m_ActivateWithUnitCommand = bardHeroics.m_ActivateWithUnitCommand;
                c.m_SelectTargetAbility = bardHeroics.m_SelectTargetAbility;
                c.Group = bardHeroics.Group;
                c.WeightInGroup = bardHeroics.WeightInGroup;
                c.DeactivateIfCombatEnded = true;
                c.DeactivateIfOwnerDisabled = true;
                c.ActivationType = AbilityActivationType.WithUnitCommand;
                c.ResourceAssetIds = bardHeroics.ResourceAssetIds;
                c.m_Icon = bardHeroics.m_Icon;
                c.SetComponents(bardHeroics.Components);
            });


            inspire_courage = CommonHelpers.ActivatableAbilityToFeature(inspire_courage_ability, false);
            inspire_heroics = CommonHelpers.ActivatableAbilityToFeature(inspire_heroics_ability, false);
            inspire_greatness = CommonHelpers.ActivatableAbilityToFeature(inspire_greatness_ability, false);

            var performance_resource = Resources.GetBlueprint<BlueprintAbilityResource>("e190ba276831b5c4fa28737e5e49e6a6");
            performance_resource_feature = Resources.GetBlueprint<BlueprintFeature>("b92bfc201c6a79e49afd0b5cfbfc269f");
            performance_resource_feature.AddComponent(Helpers.Create<NewMechanics.IncreaseResourcesByClassWithArchetype>(i =>
            {
                i.Resource = performance_resource;
                i.CharacterClass = BrawlerClass;
                i.Archetype = exemplar;
                i.base_value = -2;
            }));

            inspiring_prowess = CommonHelpers.CreateFeature("ExemplarInspiringProwessFeature",
                                                      "Inspiring Prowess",
                                                      "At 3rd level, an exemplar gains the ability to use certain bardic performances. She can use this ability for a number of rounds per day equal to 3 + her Charisma modifier; this increases by 1 round per brawler level thereafter. The exemplar’s effective bard level for this ability is equal to her brawler level – 2. At 3rd level, the exemplar can use inspire courage. At 11th level, the exemplar can use inspire greatness. At 15th level, the exemplar can use inspire heroics. Instead of the Perform skill, she activates this ability with impressive flourishes and displays of martial talent (this uses visual components).",
                                                      null,
                                                      FeatureGroup.None,
                                                      Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, ContextRankProgression.BonusValue, classes: getBrawlerArray(),
                                                                                      stepLevel: -2
                                                                                     )
                                                     );
            inspiring_prowess.ReapplyOnLevelUp = true;
        }


        static void createCallToArms()
        {
            var resource = CommonHelpers.CreateAbilityResource("ExemplarCallToArmsResource", "", "", "", null);
            resource.SetIncreasedByLevelStartPlusDivStep(3, 2, 1, 2, 1, 0, 0.0f, getBrawlerArray());

            var buff = CommonHelpers.CreateBuff("CallToArmsBuff",
                                          "Call to Arms",
                                          "At 1st level, an exemplar rouse her allies into action. All allies within 30 feet are no longer flat-footed, even if they are surprised. Using this ability is a move action. This ability can be used 3 times per day + 1 more time per 2 exemplar levels. At 6th level, the exemplar can use it as a swift action instead. At 10th level, she can use it as a free action.",
                                          Helpers.GetIcon("76f8f23f6502def4dbefedffdc4d4c43"),
                                          null,
                                          Helpers.Create<FlatFootedIgnore>(f => f.Type = FlatFootedIgnoreType.UncannyDodge)
                                          );

            var ability = CommonHelpers.CreateAbility("CallToArmsAbility",
                                                buff.Name,
                                                buff.Description,
                                                "",
                                                buff.Icon,
                                                AbilityType.Extraordinary,
                                                CommandType.Move,
                                                AbilityRange.Personal,
                                                CommonHelpers.oneRoundDuration,
                                                "",
                                                CommonHelpers.CreateRunActions(CommonHelpers.createContextActionApplyBuff(buff, CommonHelpers.CreateContextDuration(1), dispellable: false)),
                                                CommonHelpers.createAbilitySpawnFx("8de64fbe047abc243a9b4715f643739f", anchor: AbilitySpawnFxAnchor.SelectedTarget, position_anchor: AbilitySpawnFxAnchor.None, orientation_anchor: AbilitySpawnFxAnchor.None),
                                                CommonHelpers.CreateAbilityTargetsAround(30.Feet(), TargetType.Ally),
                                                resource.CreateResourceLogic()
                                                );
            ability.setMiscAbilityParametersSelfOnly();

            call_to_arms = CommonHelpers.AbilityToFeature(ability, false);
            call_to_arms.AddComponent(resource.CreateAddAbilityResource());

            var feature_move = CommonHelpers.CreateFeature("CallToArmsMoveFeature",
                                                     "",
                                                     "",
                                                     null,
                                                     FeatureGroup.None,
                                                     Helpers.Create<UseAbilitiesAsSwiftAction>(m => m.abilities = new BlueprintAbility[] { ability })
                                                     );

            feature_move.HideInCharacterSheetAndLevelUp = true;
            feature_move.HideInUI = true;
            var feature_swift = CommonHelpers.CreateFeature("CallToArmsSwiftFeature",
                                                     "",
                                                     "",
                                                     null,
                                                     FeatureGroup.None,
                                                     Helpers.Create<UseAbilitiesAsFreeAction>(m => m.abilities = new BlueprintAbility[] { ability })
                                                     );

            feature_swift.HideInCharacterSheetAndLevelUp = true;
            feature_swift.HideInUI = true; 

            call_to_arms.AddComponents(CommonHelpers.CreateAddFeatureOnClassLevel(feature_move, 6, getBrawlerArray()),
                                       CommonHelpers.CreateAddFeatureOnClassLevel(feature_swift, 10, getBrawlerArray())
                                       );
        }


        static void createFieldInstruction()
        {
            var vanguardInstruction = Resources.GetBlueprint<BlueprintAbility>("00af3b5f43aa7ae4c87bcfe4e129f6e8"); //vanguard tactician
            field_instruction_ability = Helpers.CreateBlueprint<BlueprintAbility>("BrawlerFieldInstructionAbility", c => {
                c.SetName("Field Instruction");
                c.SetDescription("At 5th level, as a standard action an exemplar can grant a teamwork feat to all allies within 30 feet who can see and hear her. This teamwork feat must be one the exemplar knows or has gained with the martial flexibility ability. Allies retain the use of this teamwork feat for 3 rounds + 1 round for every 2 brawler levels. If the granted teamwork feat is one gained from martial flexibility, this duration ends immediately if the exemplar loses access to that feat. Allies don’t need to meet the prerequisites of this teamwork feat. The exemplar can use this ability once per day at 5th level, plus one additional time per day at 9th, 12th, and 17th level.");
                c.Type = vanguardInstruction.Type;
                c.Range = vanguardInstruction.Range;
                c.CanTargetSelf = vanguardInstruction.CanTargetSelf;
                c.Animation = vanguardInstruction.Animation;
                c.AnimationStyle = vanguardInstruction.AnimationStyle;
                c.ActionType = vanguardInstruction.ActionType;
                c.LocalizedDuration = vanguardInstruction.LocalizedDuration;
                c.LocalizedSavingThrow = vanguardInstruction.LocalizedSavingThrow;
                c.MaterialComponent = vanguardInstruction.MaterialComponent;
                c.m_Icon = vanguardInstruction.m_Icon;
                c.SetComponents(vanguardInstruction.Components);
            });
            
            var tactician_resource = CommonHelpers.CreateAbilityResource("FieldInstrucitonResource", "", "", "", null);
            tactician_resource.name = "BrawlerTacticianResource";
            tactician_resource.SetFixedResource(1);
            var oldComponent = field_instruction_ability.GetComponent<AbilityResourceLogic>();
            field_instruction_ability.ReplaceComponent(oldComponent, CommonHelpers.CreateResourceLogic(tactician_resource));

            var abilities = field_instruction_ability.GetComponent<AbilityVariants>();
            var new_abilities = new BlueprintAbilityReference[0];

            foreach (var a in abilities.m_Variants)
            {
                var abilityToCopy = Resources.GetBlueprint<BlueprintAbility>(a.Guid);
                var new_ability = Helpers.CreateBlueprint<BlueprintAbility>(a.GetBlueprint().name.Replace("Vanguard", "Exemplar"));
                new_ability.SetNameDescription(abilityToCopy);
                new_ability.Type = abilityToCopy.Type;
                new_ability.Range = abilityToCopy.Range;
                new_ability.CanTargetSelf = abilityToCopy.CanTargetSelf;
                new_ability.Animation = abilityToCopy.Animation;
                new_ability.AnimationStyle = abilityToCopy.AnimationStyle;
                new_ability.ActionType = abilityToCopy.ActionType;
                new_ability.LocalizedDuration = abilityToCopy.LocalizedDuration;
                new_ability.LocalizedSavingThrow = abilityToCopy.LocalizedSavingThrow;
                new_ability.MaterialComponent = abilityToCopy.MaterialComponent;
                new_ability.m_Icon = abilityToCopy.m_Icon;
                new_ability.SetComponents(abilityToCopy.Components);
                var abilityOldComponent = new_ability.GetComponent<AbilityResourceLogic>();
                new_ability.ReplaceComponent(abilityOldComponent, CommonHelpers.CreateResourceLogic(tactician_resource));
                new_ability.Parent = field_instruction_ability;
                var abilityRankConfig = new_ability.GetComponent<ContextRankConfig>();
                new_ability.ReplaceComponent(abilityRankConfig, Helpers.CreateContextRankConfig(baseValueType: ContextRankBaseValueType.ClassLevel,
                                                                                                    progression: ContextRankProgression.DivStep,
                                                                                                    stepLevel: 2,
                                                                                                    classes: new BlueprintCharacterClass[] { BrawlerClass }
                                                                                                    )
                                                                    );


                new_ability.SetName(abilityToCopy.Name.Replace("Tactician", "Field Instruction"));
                new_abilities = new_abilities.AddToArray(new_ability.ToReference<BlueprintAbilityReference>());
                //change buffs to pick name from parent ability
            }

            field_instruction_ability.GetComponent<AbilityVariants>().m_Variants = new_abilities;

            field_instruction = CommonHelpers.CreateFeature("FieldInstructionFeature",
                                                      field_instruction_ability.Name,
                                                      field_instruction_ability.Description,
                                                     field_instruction_ability.Icon,
                                                     FeatureGroup.None,
                                                     CommonHelpers.CreateAddFact(field_instruction_ability),
                                                     CommonHelpers.CreateAddAbilityResource(tactician_resource),
                                                     Helpers.Create<ContextIncreaseResourceAmount>(c =>
                                                     {
                                                         c.Value = Helpers.CreateContextValue(AbilityRankType.Default);
                                                         c.Resource = tactician_resource;
                                                     }),
                                                     Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, ContextRankProgression.Custom,
                                                                                     classes: getBrawlerArray(),
                                                                                     customProgression: new (int, int)[] { (8, 0), (11, 1), (16, 2), (20, 3) }
                                                                                     )
                                                    );
            field_instruction.ReapplyOnLevelUp = true;
        }


        //MUTAGEN MAULER
        static void createMutagenicMauler()
        {


            mutagenic_mauler = Helpers.CreateBlueprint<BlueprintArchetype>("BrawlerMutagenicMauler", a =>
            {
                a.LocalizedName = Helpers.CreateString($"{a.name}.Name", "Mutagenic Mauler");
                a.LocalizedDescription = Helpers.CreateString($"{a.name}.Description", "Not content with perfecting her body with natural methods, a mutagenic mauler resorts to alchemy to unlock the primal beast within.");
            });
            mutagenic_mauler.m_ParentClass = BrawlerClass;
            createMutagen();
            createBeastmorph();

            mutagenic_mauler.RemoveFeatures = new LevelEntry[]
            {
                Helpers.LevelEntry(1, combat_feat),
                Helpers.LevelEntry(4, ac_bonus),
                Helpers.LevelEntry(6, combat_feat),
                Helpers.LevelEntry(10, combat_feat),
                Helpers.LevelEntry(12, combat_feat),
            };

            mutagenic_mauler.AddFeatures = new LevelEntry[] {Helpers.LevelEntry(1, mutagen),
                                                     Helpers.LevelEntry(4, beastmorph_speed),
                                                     Helpers.LevelEntry(6, mutagen_damage_bonus),
                                                     Helpers.LevelEntry(10, discovery),
                                                     Helpers.LevelEntry(12, greater_mutagen),
                                                     Helpers.LevelEntry(13, beastmorph_blindsense),
                                                    };

            brawler_progression.UIGroups = brawler_progression.UIGroups.AddToArray(Helpers.CreateUIGroup(mutagen, beastmorph_speed, mutagen_damage_bonus, discovery, greater_mutagen, beastmorph_blindsense)
                                                                                   );
        }


        static void createBeastmorph()
        {
            beastmorph_speed = CommonHelpers.CreateFeature("MutagenicMaulerBeastmorphSpeedBonusFeature",
                                                     "Beastmorph: Speed Bonus",
                                                     "Starting at 4th level, a mutagenic mauler gains additional abilities when using her mutagen. At 4th level, she gains a +10 enhancement bonus to her base speed. At 13th level, the enhancement bonus to her base speed increases to +15 feet. At 18th level, the enhancement bonus to her base speed increases to +20 feet.",
                                                     Helpers.GetIcon("4f8181e7a7f1d904fbaea64220e83379"), //expeditious retreat
                                                     FeatureGroup.None
                                                     );

            var beastmorph_speed_buff = CommonHelpers.CreateBuff("MutagenicMaulerBeastmorphSpeedBonusBuff",
                                         beastmorph_speed.Name,
                                         beastmorph_speed.Description,
                                         beastmorph_speed.Icon,
                                         null,
                                         CommonHelpers.CreateAddContextStatBonus(StatType.Speed, ModifierDescriptor.Enhancement),
                                         Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, ContextRankProgression.Custom,
                                                                         classes: getBrawlerArray(),
                                                                         customProgression: new (int, int)[] { (12, 10), (17, 15), (20, 20) }
                                                                         )
                                         );


            beastmorph_blindsense = CommonHelpers.CreateFeature("MutagenicMaulerBeastmorphBlindsenseFeature",
                                         "Beastmorph: Blindsense",
                                         "At 13th level, a mutagenic mauler gains blindsense ability within 30 feet, when using her mutagen.",
                                         Helpers.GetIcon("b3da3fbee6a751d4197e446c7e852bcb"), //true seeing
                                         FeatureGroup.None
                                         );

            var beastmorph_blindsense_buff = CommonHelpers.CreateBuff("MutagenicMaulerBeastmorphBlindsenseBuff",
                                         beastmorph_blindsense.Name,
                                         beastmorph_blindsense.Description,
                                         beastmorph_blindsense.Icon,
                                         null,
                                         CommonHelpers.createBlindsense(30)
                                         );

            var mutagens = new BlueprintFeature[]
            {
                mutagen,
                //Resources.GetBlueprint<BlueprintFeature>("cee8f65448ce71c4b8b8ca13751dd8ea"), //mutagen
                 //Resources.GetBlueprint<BlueprintFeature>("76c61966afdd82048911f3d63c6fe0bc"), //greater mutagen
                 greater_mutagen,
                 Resources.GetBlueprint<BlueprintFeature>("6f5cb651e26bd97428523061b07ffc85"), //grand mutagen

            };

            foreach (var m in mutagens)
            {
                var comp = m.GetComponent<AddFacts>();

                foreach (var f in comp.Facts)
                {
                    var buff = CommonHelpers.extractActions<ContextActionApplyBuff>((f as BlueprintAbility).GetComponent<AbilityEffectRunAction>().Actions.Actions)[0].Buff;

                    CommonHelpers.addContextActionApplyBuffOnFactsToActivatedAbilityBuffNoRemove(buff, beastmorph_speed_buff, beastmorph_speed);
                    CommonHelpers.addContextActionApplyBuffOnFactsToActivatedAbilityBuffNoRemove(buff, beastmorph_blindsense_buff, beastmorph_blindsense);
                }
            }

        }


        static void createMutagen()
        {
            var alchemist_mutagen = Resources.GetBlueprint<BlueprintFeature>("cee8f65448ce71c4b8b8ca13751dd8ea");


            mutagen = CommonHelpers.CreateFeature(
                "MutagenicMaulerMutagen",
                alchemist_mutagen.m_DisplayName,
                "At 1st level, a mutagenic mauler discovers how to create a mutagen that she can imbibe in order to heighten her physical prowess, though at the cost of her personality. This functions as an alchemist’s mutagen and uses the brawler’s class level as her alchemist level for this ability (alchemist levels stack with brawler levels for determining the effect of this ability). A mutagenic mauler counts as an alchemist for the purpose of imbibing a mutagen prepared by someone else.",
                alchemist_mutagen.m_Icon,
                FeatureGroup.None,
                alchemist_mutagen.Components
                ); 
                
             
            mutagen.GetComponent<AddFacts>().DoNotRestoreMissingFacts = true;

            foreach (var factRef in mutagen.GetComponent<AddFacts>().Facts)
            {

                if (factRef is BlueprintAbility fact)
                {
                    foreach (var c in fact.GetComponents<ContextRankConfig>().ToArray())
                    {
                        var newRankConfig = c;

                        newRankConfig.m_AdditionalArchetypes = c.m_AdditionalArchetypes.AddToArray(mutagenic_mauler.ToReference<BlueprintArchetypeReference>());
                        newRankConfig.m_Class = c.m_Class.AddToArray(BrawlerClass.ToReference<BlueprintCharacterClassReference>());
                        mutagen.ReplaceComponent(c, newRankConfig);
                    }
                }

            }

           // alchemist_mutagen.AddComponent(Helpers.Create<NewMechanics.FeatureReplacement>(f => f.replacement_feature = mutagen));
           // mutagen.SetDescription("At 1st level, a mutagenic mauler discovers how to create a mutagen that she can imbibe in order to heighten her physical prowess, though at the cost of her personality. This functions as an alchemist’s mutagen and uses the brawler’s class level as her alchemist level for this ability (alchemist levels stack with brawler levels for determining the effect of this ability). A mutagenic mauler counts as an alchemist for the purpose of imbibing a mutagen prepared by someone else.");
            var mutagen_damage_buff = CommonHelpers.CreateBuff("MutagenincMaulerDamageBonusBuff",
                                                         "Mutagen Damage Bonus",
                                                         "At 6th level, a mutagenic mauler gains a +2 bonus on damage rolls when she attacks in melee while in her mutagenic form. This bonus increases to +3 at 11th level, and to +4 at 16th level.",
                                                         Helpers.GetIcon("85067a04a97416949b5d1dbf986d93f3"), //stone fist
                                                         null,
                                                         Helpers.Create<WeaponAttackTypeDamageBonus>(w =>
                                                         {
                                                             w.Value = Helpers.CreateContextValue(AbilityRankType.Default);
                                                             w.Type = WeaponRangeType.Melee;
                                                             w.Descriptor = ModifierDescriptor.UntypedStackable;
                                                             w.AttackBonus = 1;
                                                         }
                                                         ),
                                                         Helpers.CreateContextRankConfig(ContextRankBaseValueType.ClassLevel, classes: getBrawlerArray(),
                                                                                         progression: ContextRankProgression.StartPlusDivStep,
                                                                                         startLevel: 1, stepLevel: 5, max: 4)
                                                       );

            mutagen_damage_bonus = CommonHelpers.CreateFeature("MutagenicMaulerDamageBonusFeature",
                                                         mutagen_damage_buff.Name,
                                                         mutagen_damage_buff.Description,
                                                         mutagen_damage_buff.Icon,
                                                         FeatureGroup.None);

            var mutagens = new BlueprintFeature[]
            {
                mutagen,
                //Resources.GetBlueprint<BlueprintFeature>("cee8f65448ce71c4b8b8ca13751dd8ea"), //mutagen
                 Resources.GetBlueprint<BlueprintFeature>("76c61966afdd82048911f3d63c6fe0bc"), //greater mutagen
                 Resources.GetBlueprint<BlueprintFeature>("6f5cb651e26bd97428523061b07ffc85"), //grand mutagen

            };

            foreach (var m in mutagens)
            {
                var comp = m.GetComponent<AddFacts>();

                foreach (var f in comp.Facts)
                {
                    var buff = CommonHelpers.extractActions<ContextActionApplyBuff>((f as BlueprintAbility).GetComponent<AbilityEffectRunAction>().Actions.Actions)[0].Buff;

                    CommonHelpers.addContextActionApplyBuffOnFactsToActivatedAbilityBuffNoRemove(buff, mutagen_damage_buff, mutagen_damage_bonus);
                }
            }

            var alch_discovery = Resources.GetBlueprint<BlueprintFeatureSelection>("cd86c437488386f438dcc9ae727ea2a6");
            discovery = CommonHelpers.CreateFeatureSelection(
                "MutagenicMaulerDiscovery",
                alch_discovery.m_DisplayName,
                "At 10th level, a mutagenic mauler learns one of the following alchemist discoveries: feral mutagen, preserve organs, spontaneous healing.",
                alch_discovery.m_Icon,
                FeatureGroup.Discovery
                );
                

            discovery.m_AllFeatures = new BlueprintFeatureReference[]
            {
                Resources.GetBlueprint<BlueprintFeature>("fd5f7b37ab4301c48a88cc196ee5f0ce").ToReference<BlueprintFeatureReference>(), //feral mutagen
                Resources.GetBlueprint<BlueprintFeature>("76b4bb8e54f3f5c418f421684c76ef4e").ToReference<BlueprintFeatureReference>(), //preserve organs
                Resources.GetBlueprint<BlueprintFeature>("2bc1ee626a69667469ab5c1698b99956").ToReference<BlueprintFeatureReference>(), //spontaneous healing
            };

            var spontaneous_healing_resource = Resources.GetBlueprint<BlueprintAbilityResource>("0b417a7292b2e924782ef2aab9451816");

            spontaneous_healing_resource.m_MaxAmount.m_ClassDiv = spontaneous_healing_resource.m_MaxAmount.m_ClassDiv.AddToArray(BrawlerClass.ToReference<BlueprintCharacterClassReference>());
            spontaneous_healing_resource.m_MaxAmount.m_ArchetypesDiv = spontaneous_healing_resource.m_MaxAmount.m_ArchetypesDiv.AddToArray(mutagenic_mauler.ToReference<BlueprintArchetypeReference>());

            var alchemist_greater_mutagen = Resources.GetBlueprint<BlueprintFeature>("76c61966afdd82048911f3d63c6fe0bc");
            greater_mutagen = CommonHelpers.CreateFeature(
                "MutagenicMaulerGreaterMutagen",
                alchemist_greater_mutagen.m_DisplayName,
                "At 12th level, the mutagenic mauler learns the greater mutagen discovery.",
                alchemist_greater_mutagen.m_Icon,
                FeatureGroup.None,
                alchemist_greater_mutagen.Components
                );

            
            greater_mutagen.RemoveComponents<PrerequisiteClassLevel>();



            foreach (var factRef in greater_mutagen.GetComponent<AddFacts>().Facts)
            {

                if (factRef is BlueprintAbility fact)
                {
                    foreach (var c in fact.GetComponents<ContextRankConfig>().ToArray())
                    {
                        var newRankConfig = c;

                        newRankConfig.m_AdditionalArchetypes = c.m_AdditionalArchetypes.AddToArray(mutagenic_mauler.ToReference<BlueprintArchetypeReference>());
                        newRankConfig.m_Class = c.m_Class.AddToArray(BrawlerClass.ToReference<BlueprintCharacterClassReference>());
                        greater_mutagen.ReplaceComponent(c, newRankConfig);
                    }
                }

            }
   
           // alchemist_greater_mutagen.AddComponent(Helpers.Create<NewMechanics.FeatureReplacement>(f => f.replacement_feature = greater_mutagen));
        }
        // MISC
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
                if (!Owner.Body.PrimaryHand.HasWeapon && !Owner.Body.SecondaryHand.HasWeapon)
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
                    if(weapon1.Blueprint.FighterGroup.Contains(group) && (weapon2 == null || weapon2.Blueprint.FighterGroup.Contains(group)))
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
