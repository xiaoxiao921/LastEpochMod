using MelonLoader;
using System;
using UnityEngine;

namespace LastEpochMod
{
    public class LastEpochMod : MelonMod
    {
        public const string Name = nameof(LastEpochMod);
        public const string Version = "1.0.0";
        public const string Author = "iDeathHD";

        internal static LastEpochMod Instance;

        public override void OnApplicationStart()
        {
            Instance = this;

            Config.Init();

            Items.Headhunter.Init();

            ResourcesListener.Init();

            //Harmony.PatchAll();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Items.Spawner.AddUniqueItemToPlayerInventory(Items.Spawner.ItemName.GetValueAsString());
                //Items.Finder.PrintAllEquippableSubItems();

                foreach (var item in Player.Cache.GameObject.GetComponents<Component>())
                {
                    MelonLogger.Warning(item.ToString());
                }
            }
        }
    }
}
