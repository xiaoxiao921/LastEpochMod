using MelonLoader;
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

            ResourcesListener.Init();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Items.Spawner.AddUniqueItemToPlayerInventory(Items.Spawner.ItemName.GetValueAsString());
            }
        }
    }
}
