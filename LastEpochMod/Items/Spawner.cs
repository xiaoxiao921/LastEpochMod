using MelonLoader;

namespace LastEpochMod.Items
{
    public static class Spawner
    {
        internal static MelonPreferences_Entry ItemName;
        private const string ConfigEntryIdItemName = "ItemName";
        private const string ItemNameConfigDefaultValue = "Apathy Maw";

        internal static MelonPreferences_Entry ItemRarity;
        private const string ConfigEntryIdItemRarity = "Rarity";
        private const int ItemRarityConfigDefaultValue = UniqueItemRarity;

        internal const int UniqueItemRarity = 7;

        internal static void ConfigInit()
        {
            ItemName = Config.Category.CreateEntry(ConfigEntryIdItemName, ItemNameConfigDefaultValue, "Name of the unique item to spawn.");
            ItemRarity = Config.Category.CreateEntry(ConfigEntryIdItemRarity, ItemRarityConfigDefaultValue, "Rarity of the item to spawn.");
        }

        public static void AddUniqueItemToPlayerInventory(string name)
        {
            var generateItems = Player.Cache.GenerateItems;
            var itemContainersManager = Player.Cache.ItemContainersManager;

            var itemDataUnpacked = generateItems.GenerateUniqueItem(name);
            itemContainersManager.inventory.TryAddItem(itemDataUnpacked, 1, Context.PICKUP);
        }

        public static ItemDataUnpacked GenerateUniqueItem(this GenerateItems self, string uniqueName, int socketNumber = 1)
        {
            return self.GenerateUniqueItem(Finder.GetUniqueItem(uniqueName), socketNumber);
        }

        public static ItemDataUnpacked GenerateUniqueItem(this GenerateItems self, UniqueList.Entry uniqueEntry, int socketNumber = 1)
        {
            return self.GenerateSpecificItem(uniqueEntry.baseType, 0, uniqueEntry.levelRequirement, UniqueItemRarity, uniqueEntry.uniqueID, socketNumber);
        }
    }
}
