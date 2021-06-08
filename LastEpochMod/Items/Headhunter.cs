using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using System.Collections;
using System.IO;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpochMod.Items
{
    public static class Headhunter
    {
        public const string Name = "Headhunter";

        public const float BuffDuration = 20f;

        private static Texture2D _texture;
        public static Texture2D Texture
        {
            get
            {
                if (!_texture)
                {
                    var modDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Mods", nameof(LastEpochMod));
                    Directory.CreateDirectory(modDirectoryPath);
                    var pngPath = Path.Combine(modDirectoryPath, Name + ".png");
                    var png = File.ReadAllBytes(pngPath);

                    _texture = new Texture2D(1, 1);
                    ImageConversion.LoadImage(_texture, png, true);
                }

                return _texture;
            }
        }

        private static Sprite _sprite;
        public static Sprite Sprite
        {
            get
            {
                if (!_sprite)
                {
                    _sprite = Sprite.Create(Texture, new Rect(0, 0, Texture.width, Texture.height), Vector2.zero);
                }

                return _sprite;
            }
        }

        public const string LoreText =
            "A man's soul rules from a cavern of bone, " +
            "learns and judges through flesh-born windows. " +
            "The heart is meat. " +
            "The head is where the Man is. " +
            "- Lavianga, Advisor to Kaom";

        internal static void Init()
        {
            ClassInjector.RegisterTypeInIl2Cpp<HHTracker>();

            AddUniqueEntry();

            AddHeadhunterOnKillEffect();
        }

        private static void AddUniqueEntry()
        {
            //The Scavenger is an unique leather belt
            var theScavenger = Finder.GetUniqueItem("The Scavenger");
            var uleros = Finder.GetUniqueItem("Chains of Uleros");

            var headhunterEntry = new UniqueList.Entry();

            headhunterEntry.name = Name;
            headhunterEntry.displayName = Name;
            headhunterEntry.loreText = LoreText;

            headhunterEntry.baseType = theScavenger.baseType;
            headhunterEntry.subTypes = theScavenger.subTypes;

            headhunterEntry.canDropRandomly = true;

            headhunterEntry.isSetItem = false;
            headhunterEntry.setID = 0;

            headhunterEntry.levelRequirement = 1;
            headhunterEntry.overrideLevelRequirement = true;

            headhunterEntry.rerollChance = theScavenger.rerollChance;

            AddHeadhunterMods(headhunterEntry);

            foreach (var entry in uleros.tooltipEntries)
            {
                MelonLogger.Msg("modDisplay : " + entry.modDisplay);
            }
            MelonLogger.Msg("");
            foreach (var entry in uleros.tooltipDescriptions)
            {
                MelonLogger.Msg("altText : " + entry.altText);
                MelonLogger.Msg("description : " + entry.description);
                MelonLogger.Msg("setMod: " + entry.setMod);
                MelonLogger.Msg("setRequirement: " + entry.setRequirement);
            }
            MelonLogger.Msg("");
            foreach (var entry in uleros.mods)
            {
                MelonLogger.Msg("canRoll : " + entry.canRoll);
                MelonLogger.Msg("hideInTooltip : " + entry.hideInTooltip);
                MelonLogger.Msg("maxValue : " + entry.maxValue);
                MelonLogger.Msg("property : " + entry.property.ToString());
                MelonLogger.Msg("rollID : " + entry.rollID);
                MelonLogger.Msg("specialTag : " + entry.specialTag);
                MelonLogger.Msg("tags : " + entry.tags);
                MelonLogger.Msg("type : " + entry.type.ToString());
                MelonLogger.Msg("value : " + entry.value);
            }


            headhunterEntry.tooltipEntries = new List<UniqueModDisplayListEntry>();
            var tooltipEntry0 = new UniqueModDisplayListEntry(0);
            headhunterEntry.tooltipEntries.Add(tooltipEntry0);
            var tooltipEntry1 = new UniqueModDisplayListEntry(1);
            headhunterEntry.tooltipEntries.Add(tooltipEntry1);
            var tooltipEntry2 = new UniqueModDisplayListEntry(2);
            headhunterEntry.tooltipEntries.Add(tooltipEntry2);

            var tooltipEntry3 = new UniqueModDisplayListEntry(128);
            headhunterEntry.tooltipEntries.Add(tooltipEntry3);

            headhunterEntry.tooltipDescriptions = new List<ItemTooltipDescription>();
            var tooltipDescription0 = new ItemTooltipDescription();
            tooltipDescription0.description = "When you Kill a Rare monster, you gain its Modifiers for 20 seconds";
            headhunterEntry.tooltipDescriptions.Add(tooltipDescription0);

            headhunterEntry.uniqueID = 666;

            var uniqueList = UniqueList.get();
            uniqueList.uniques.Add(headhunterEntry);
        }

        private static void AddHeadhunterMods(UniqueList.Entry headhunterEntry)
        {
            var headhunterMods = new List<UniqueItemMod>();

            var addedStrength = new UniqueItemMod();
            addedStrength.canRoll = true;
            addedStrength.property = SP.Strength;
            addedStrength.tags = AT.None;
            addedStrength.type = BaseStats.ModType.ADDED;
            addedStrength.setMaxValue(55);
            addedStrength.setValue(40);
            headhunterMods.Add(addedStrength);

            var addedDexterity = new UniqueItemMod();
            addedStrength.canRoll = true;
            addedDexterity.property = SP.Dexterity;
            addedDexterity.tags = AT.None;
            addedDexterity.type = BaseStats.ModType.ADDED;
            addedDexterity.setMaxValue(55);
            addedDexterity.setValue(40);
            headhunterMods.Add(addedDexterity);

            var addedLife = new UniqueItemMod();
            addedStrength.canRoll = true;
            addedLife.property = SP.Health;
            addedLife.tags = AT.None;
            addedLife.type = BaseStats.ModType.ADDED;
            addedLife.setMaxValue(60);
            addedLife.setValue(50);
            headhunterMods.Add(addedLife);

            headhunterEntry.mods = headhunterMods;
        }

        private static void AddHeadhunterOnKillEffect()
        {
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(OnSceneLoadedAddHeadhunterEventListener));
        }

        private static readonly Action<Ability, Actor> OnKillHHEffectAction = new Action<Ability, Actor>(OnKillHHEffect);
        private static void OnSceneLoadedAddHeadhunterEventListener(Scene scene, LoadSceneMode mode)
        {
            var playerActor = Player.Cache.Actor;

            if (playerActor)
            {
                if (!Player.Cache.GameObject.GetComponent<HHTracker>())
                {
                    var abilityEventListener = Player.Cache.GameObject.GetComponent<AbilityEventListener>();
                    abilityEventListener.add_onKillEvent(OnKillHHEffectAction);
                    Player.Cache.GameObject.AddComponent<HHTracker>();
                }
            }
        }

        private static bool CanStealMods(Actor actor) =>
            actor.tag == "Enemy" &&
            actor.rarity == Actor.Rarity.Rare &&
            !actor.data.isBossOrMiniBoss();

        private static void OnKillHHEffect(Ability ability, Actor killedActor)
        {
            var playerActor = Player.Cache.Actor;
            if (!playerActor)
            {
                return;
            }

            var isHeadhunterEquipped = playerActor.itemContainersManager.hasUniqueEquipped(666);
            if (!isHeadhunterEquipped)
            {
                return;
            }

            if (CanStealMods(killedActor))
            {
                var monsterMods = killedActor.appliedMods;
                foreach (var monsterMod in monsterMods)
                {
                    var monsterModName = monsterMod.name.ToLowerInvariant();
                    if (monsterModName.Contains("summon") || monsterModName.Contains("twinned"))
                    {
                        continue;
                    }

                    var statMod = monsterMod.TryCast<StatsMonsterMod>();
                    if (statMod)
                    {
                        statMod.TimedApplyMod(playerActor, killedActor.data.level, true, 0);
                        MelonLogger.Warning("Adding to player the stat mod : " + monsterMod.name);
                    }
                    else
                    {
                        var componentMod = monsterMod.TryCast<ComponentMonsterMod>();
                        if (componentMod)
                        {
                            componentMod.TimedApplyMod(playerActor);
                            MelonLogger.Warning("Adding to player the comp mod : " + monsterMod.name);
                        }
                    }
                }
            }
        }

        private static void TimedApplyMod(this StatsMonsterMod self, Actor actor, int level, bool isRare, float effectModifier)
        {
            if (!actor)
            {
                return;
            }

            var monsterModStats = self.stats;
            if (monsterModStats == null)
            {
                return;
            }

            var characterStats = actor.stats.Cast<CharacterStats>();
            if (!characterStats)
            {
                return;

            }

            var baseHealth = actor.health;
            if (!baseHealth)
            {
                return;
            }

            var rareModifier = isRare ? self.rareModifier + 1 : 1;

            var scalingType = self.scalingType;

            var finalScalingValue = 1f;
            if (scalingType != StatsMonsterMod.ScalingType.None)
            {
                var scalingValue = 0f;

                if (scalingType == StatsMonsterMod.ScalingType.Level)
                {
                    scalingValue = level;
                }
                else if (scalingType == StatsMonsterMod.ScalingType.Health)
                {
                    scalingValue = baseHealth.maxHealth;
                }

                finalScalingValue =
                    (scalingValue * self.perUnit) + self.flatScaling +
                    (scalingValue * self.perUnitSquared * scalingValue);
            }

            var monsterModStatsWithoutEffectModifier = (byte)self.statsWithoutEffectModifier;

            var i = 0;
            foreach (var statToClone in monsterModStats)
            {
                var statMultiplier = rareModifier;

                if ((int)self.effectModifierEffect - (int)MonsterMod.EffectModifierEffect.Partial > (int)MonsterMod.EffectModifierEffect.None ||
                    !EpochExtensions.bitSet(monsterModStatsWithoutEffectModifier, i))
                {
                    statMultiplier = (float)(effectModifier + 1) * rareModifier;
                }

                if (!self.onlyScaleSomeStats || i < self.numberOfStatsToScale)
                {
                    statMultiplier *= finalScalingValue;
                }

                var stat =
                    statMultiplier == 1 ?
                    new Stats.Stat(statToClone) :
                    new Stats.Stat(statToClone, statMultiplier);

                var buff = new Buff(stat, BuffDuration);
                actor.characterMutator.statBuffs.addBuff(buff);

                i++;
            }
        }

        private static void TimedApplyMod(this ComponentMonsterMod self, Actor actor)
        {
            if (!actor)
            {
                return;
            }

            var componentTypeToAdd = self.getComponentType();
            if (componentTypeToAdd != null)
            {
                var component = actor.gameObject.AddComponent(componentTypeToAdd);
                MelonCoroutines.Start(DestroyAfterDelay(component, BuffDuration));
            }
        }

        private static IEnumerator DestroyAfterDelay(Component component, float duration)
        {
            yield return new WaitForSeconds(duration);

            UnityEngine.Object.Destroy(component);
        }
    }

    public class HHTracker : MonoBehaviour
    {
        public HHTracker(IntPtr ptr) : base(ptr) { }
    }
}
