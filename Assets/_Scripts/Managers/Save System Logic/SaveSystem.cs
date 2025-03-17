using System.IO;
using System.IO.Compression;
using System.Text;
using _Scripts.CoreSystem.StatSystem;
using _Scripts.JSON;
using Newtonsoft.Json;
using UnityEngine;

namespace _Scripts.Managers.Save_System_Logic
{
    public static class SaveSystem
    {
        #region Paths
        
        public static string PlayerDataSavePath => Application.persistentDataPath + "/playerdata.json";
        public static string OptionsDataSavePath => Application.persistentDataPath + "/options.json";

        #endregion
        
        #region Player Data

        public static void SavePlayerData(PlayerData data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            var compressedJson = CompressString(json);
            File.WriteAllText(PlayerDataSavePath, compressedJson);

            Debug.Log($"Data Saved (Compressed): {compressedJson}");
        }
        public static PlayerData LoadPlayerData()
        {
            if (File.Exists(PlayerDataSavePath))
            {
                var savedData = File.ReadAllText(PlayerDataSavePath);

                try
                {
                    var json = DecompressString(savedData);
                    return JsonConvert.DeserializeObject<PlayerData>(json) ?? new PlayerData();
                }
                catch
                {
                    Debug.LogWarning("Save file might be corrupted or already decompressed. Resetting to default.");
                    return new PlayerData();
                }
            }

            Debug.Log("No Save File Found. Creating New Data.");
            return new PlayerData();
        }
        public static void DeletePlayerData()
        {
            if (File.Exists(PlayerDataSavePath))
            {
                File.Delete(PlayerDataSavePath);
                Debug.Log("Save Data File Deleted.");
            }
            else
            {
                Debug.Log("No Save Data Found to Delete.");
            }
        }

        #endregion
        
        #region Options Data

        public static void SaveOptionsData(OptionsData data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(OptionsDataSavePath, json);
            Debug.Log($"Options Data Saved: {json}");
        }

        public static OptionsData LoadOptionsData()
        {
            if (File.Exists(OptionsDataSavePath))
            {
                var json = File.ReadAllText(OptionsDataSavePath);
                return JsonConvert.DeserializeObject<OptionsData>(json);
            }
            
            Debug.Log("No Options File Found. Creating New Data.");
            return new OptionsData();
        }

        public static void DeleteOptionsData()
        {
            if (File.Exists(OptionsDataSavePath))
            {
                File.Delete(OptionsDataSavePath);
                Debug.Log("Save Data File Deleted.");
            }
            else
            {
                Debug.Log("No Save Data Found to Delete.");
            }
        }
        
        #endregion
        
        #region Compression
        
        private static string CompressString(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }
            return System.Convert.ToBase64String(output.ToArray());
        }

        private static string DecompressString(string compressedText)
        {
            var bytes = System.Convert.FromBase64String(compressedText);
            using var input = new MemoryStream(bytes);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            {
                gzip.CopyTo(output);
                return Encoding.UTF8.GetString(output.ToArray());
            }
        }
        
        #endregion
        
        // Function that can be used for debugging, because right now the save file JSON is saved as encoded file so it is not readable for person
        public static string GetReadableSaveData()
        {
            if (!File.Exists(PlayerDataSavePath)) return "No Save File Found.";

            var savedData = File.ReadAllText(PlayerDataSavePath);

            try
            {
                var json = DecompressString(savedData);
                return json; //Return readable JSON
            }
            catch
            {
                return "Save file is already in readable JSON format:\n" + savedData;
            }
        }
    }
}
