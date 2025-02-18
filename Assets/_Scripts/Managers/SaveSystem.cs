using System.IO;
using _Scripts.JSON;
using UnityEngine;

namespace _Scripts.Managers
{
    public static class SaveSystem
    {
        private static string SavePath => Application.persistentDataPath + "/playerdata.json";

        public static void SaveData(PlayerData data)
        {
            var json = JsonUtility.ToJson(data, true);
            
            File.WriteAllText(SavePath, json);
            Debug.Log("Game Saved: " + SavePath);
        }

        public static PlayerData LoadData()
        {
            if (File.Exists(SavePath))
            {
                var json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<PlayerData>(json);
            }

            Debug.Log("No Save File Found. Creating New Data");
            return new PlayerData();
        }

        public static void DeleteData()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save Data Deleted" + SavePath);
            }
        }
    }
}
