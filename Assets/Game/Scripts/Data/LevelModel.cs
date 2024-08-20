using UnityEngine;

namespace Live17Game
{
    public static class LevelModel
    {
        private static LevelData[] LEVEL_DATA_LIST = new LevelData[]
        {
            new LevelData(new PlatformSizeRange(2, 3), 2),
            new LevelData(new PlatformSizeRange(2, 3), 3),
            new LevelData(new PlatformSizeRange(1, 3), 3),
            new LevelData(new PlatformSizeRange(1, 2), 4),
            new LevelData(new PlatformSizeRange(1, 2), 5),
            new LevelData(new PlatformSizeRange(1, 2), 6),
        };

        public static LevelData GetLevelData(uint level)
        {
            if (level >= LEVEL_DATA_LIST.Length)
            {
                return LEVEL_DATA_LIST[LEVEL_DATA_LIST.Length - 1];
            }

            return LEVEL_DATA_LIST[level];
        }
    }
}