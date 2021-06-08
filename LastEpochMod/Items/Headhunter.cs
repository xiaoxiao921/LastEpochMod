using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using System.Collections;
using System.Drawing.Imaging;
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
                    using var stream = new MemoryStream();
                    Properties.Resources.Headhunter.Save(stream, ImageFormat.Png);

                    _texture = new Texture2D(1, 1);
                    ImageConversion.LoadImage(_texture, stream.ToArray(), true);
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

        public static UniqueList.Entry UniqueEntry { get; private set; }

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

            // Just to get the same rollChance as Orians Eye
            var oriansEye = Finder.GetUniqueItem("Orians Eye");

            UniqueEntry = new UniqueList.Entry();

            UniqueEntry.name = Name;
            UniqueEntry.displayName = Name;
            UniqueEntry.loreText = LoreText;

            UniqueEntry.baseType = theScavenger.baseType;
            UniqueEntry.subTypes = theScavenger.subTypes;

            UniqueEntry.canDropRandomly = true;

            UniqueEntry.isSetItem = false;
            UniqueEntry.setID = 0;

            UniqueEntry.levelRequirement = 40;
            UniqueEntry.overrideLevelRequirement = true;

            UniqueEntry.rerollChance = oriansEye.rerollChance;

            AddHeadhunterMods(UniqueEntry);

            UniqueEntry.tooltipEntries = new List<UniqueModDisplayListEntry>();
            var tooltipEntry0 = new UniqueModDisplayListEntry(0);
            UniqueEntry.tooltipEntries.Add(tooltipEntry0);
            var tooltipEntry1 = new UniqueModDisplayListEntry(1);
            UniqueEntry.tooltipEntries.Add(tooltipEntry1);

            var tooltipEntry2 = new UniqueModDisplayListEntry(128);
            UniqueEntry.tooltipEntries.Add(tooltipEntry2);

            UniqueEntry.tooltipDescriptions = new List<ItemTooltipDescription>();
            var tooltipDescription0 = new ItemTooltipDescription();
            tooltipDescription0.description = "When you Kill a Rare monster, you gain its Modifiers for 20 seconds";
            UniqueEntry.tooltipDescriptions.Add(tooltipDescription0);

            UniqueEntry.uniqueID = 666;

            var uniqueList = UniqueList.get();
            uniqueList.uniques.Add(UniqueEntry);
        }

        private static void AddHeadhunterMods(UniqueList.Entry headhunterEntry)
        {
            var headhunterMods = new List<UniqueItemMod>();

            var addedAttributes = new UniqueItemMod();
            addedAttributes.canRoll = true;
            addedAttributes.property = SP.AllAttributes;
            addedAttributes.tags = AT.None;
            addedAttributes.type = BaseStats.ModType.ADDED;
            addedAttributes.setMaxValue(10);
            addedAttributes.setValue(5);
            headhunterMods.Add(addedAttributes);

            var addedLife = new UniqueItemMod();
            addedLife.canRoll = true;
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

        public static void TestRandomLeatherBeltDrop()
        {
            var itemDataUnpacked = new ItemDataUnpacked();
            PlayerFinder.getPlayerActor().generateItems.RollBaseItem(ref itemDataUnpacked, 50, false, UniqueEntry.baseType, UniqueEntry.subTypes[0], 7, ItemLocationTag.None, -1, -1, true);
            PlayerFinder.getPlayerActor().generateItems.GenerateAffixes(ref itemDataUnpacked, 50, false, false);
            if (itemDataUnpacked != null)
            {
                itemDataUnpacked.RebuildID();
                MelonLogger.Msg(itemDataUnpacked.FullName);
            }
        }
    }

    public class HHTracker : MonoBehaviour
    {
        public HHTracker(IntPtr ptr) : base(ptr) { }
    }
}
