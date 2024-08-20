using UnityEngine;

namespace Live17Game
{
    public static class StorageManager
    {
        private const string GAME_DATA_ID = "BigWatermelon";

        private static string DEFAULT_GAME_DATA_JSON => JsonUtility.ToJson(new GameData());

        public static GameData LoadGameData()
        {
            string jsonString = PlayerPrefs.GetString(GAME_DATA_ID, DEFAULT_GAME_DATA_JSON);
            // Debug.Log($"===== LoadGameData:{jsonString}");
            GameData gameData = JsonUtility.FromJson<GameData>(jsonString);
            return gameData;
        }

        public static void SaveGameData(GameData gameData)
        {
            string jsonString = JsonUtility.ToJson(gameData);
            // Debug.Log($"===== SaveGameData:{jsonString}");
            PlayerPrefs.SetString(GAME_DATA_ID, jsonString);
            PlayerPrefs.Save();
        }

        public static void DeleteAllData()
        {
            PlayerPrefs.DeleteKey(GAME_DATA_ID);
            PlayerPrefs.Save();
        }
    }
}