using MelonLoader;

namespace LastEpochMod
{
    internal static class Config
    {
        internal const string ConfigCategoryId = nameof(LastEpochMod);

        internal static MelonPreferences_Category Category;

        internal static void Init()
        {
            Category = MelonPreferences.CreateCategory(ConfigCategoryId, $"{ConfigCategoryId} Settings");

            Items.Spawner.ConfigInit();

            MelonPreferences.Save();
        }
    }
}
