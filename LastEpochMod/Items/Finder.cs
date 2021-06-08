using MelonLoader;

namespace LastEpochMod.Items
{
    public static class Finder
    {
        internal static UniqueList.Entry GetUniqueItem(string name)
        {
            var uniqueList = UniqueList.get();

            foreach (var uniqueEntry in uniqueList.uniques)
            {
                if (name.Equals(uniqueEntry.name, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return uniqueEntry;
                }
            }

            return null;
        }

        internal static void PrintUniqueItems()
        {
            var uniqueList = UniqueList.get();

            foreach (var uniqueEntry in uniqueList.uniques)
            {
                MelonLogger.Msg(uniqueEntry.name);
            }
        }

        internal static void PrintAllEquippableSubItems()
        {
            var itemList = ItemList.get();

            foreach (var item in itemList.EquippableItems)
            {
                MelonLogger.Warning(item.BaseTypeName);

                foreach (var subItem in item.subItems)
                {
                    MelonLogger.Msg(subItem.name);
                }
            }
        }
    }
}
