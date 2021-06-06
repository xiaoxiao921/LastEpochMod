using UnityEngine;

namespace LastEpochMod.Player
{
    public static class Cache
    {
        private static GameObject _player;
        internal static GameObject GameObject
        {
            get
            {
                if (!_player)
                {
                    _player = PlayerFinder.getPlayer();
                    
                }

                return _player;
            }
        }

        private static GenerateItems _playerGenerateItems;
        internal static GenerateItems GenerateItems
        {
            get
            {
                if (!_playerGenerateItems)
                {
                    _playerGenerateItems = GameObject.GetComponent<GenerateItems>();
                }

                return _playerGenerateItems;
            }
        }

        private static ItemContainersManager _itemContainersManager;
        internal static ItemContainersManager ItemContainersManager
        {
            get
            {
                if (!_itemContainersManager)
                {
                    _itemContainersManager = GameObject.GetComponent<ItemContainersManager>();
                }

                return _itemContainersManager;
            }
        }
    }
}
