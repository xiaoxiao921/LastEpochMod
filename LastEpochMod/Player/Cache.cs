using UnityEngine;

namespace LastEpochMod.Player
{
    public static class Cache
    {
        private static GameObject _player;
        public static GameObject GameObject
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

        private static Actor _actor;
        public static Actor Actor
        {
            get
            {
                if (!_actor)
                {
                    _actor = PlayerFinder.getPlayerActor();

                }

                return _actor;
            }
        }

        public static CharacterData CharacterData => PlayerFinder.getPlayerData();

        private static GenerateItems _playerGenerateItems;
        public static GenerateItems GenerateItems
        {
            get
            {
                if (!_playerGenerateItems)
                {
                    _playerGenerateItems = Actor.generateItems;
                }

                return _playerGenerateItems;
            }
        }

        private static ItemContainersManager _itemContainersManager;
        public static ItemContainersManager ItemContainersManager
        {
            get
            {
                if (!_itemContainersManager)
                {
                    _itemContainersManager = Actor.itemContainersManager;
                }

                return _itemContainersManager;
            }
        }
    }
}
