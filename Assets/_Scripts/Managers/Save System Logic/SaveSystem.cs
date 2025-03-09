using System.IO;
using System.IO.Compression;
using System.Text;
using _Scripts.JSON;
using Newtonsoft.Json;
using UnityEngine;

namespace _Scripts.Managers.Save_System_Logic
{
    public static class SaveSystem
    {
        public static string SavePath => Application.persistentDataPath + "/playerdata.json";

        public static void SaveData(PlayerData data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            var compressedJson = CompressString(json);
            File.WriteAllText(SavePath, compressedJson);

            Debug.Log($"Data Saved (Compressed): {compressedJson}");
        }
    
        public static PlayerData LoadData()
        {
            if (File.Exists(SavePath))
            {
                var savedData = File.ReadAllText(SavePath);

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

        // public static void DeleteData()
        // {
        //     if (File.Exists(SavePath))
        //     {
        //         File.Delete(SavePath);
        //         Debug.Log("Save Data Deleted: " + SavePath);
        //     }
        //
        //     //Immediately create a new default save to prevent issues
        //     SaveData(new PlayerData());
        // }
        
        public static void DeleteData()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save Data File Deleted.");
            }
            else
            {
                Debug.Log("No Save Data Found to Delete.");
            }
        }
    
        /// <summary>
        /// Function that can be used for debugging, because right now the save file JSON is saved as encoded file so it is not readable for person
        /// </summary>
        /// <returns></returns>
        public static string GetReadableSaveData()
        {
            if (!File.Exists(SavePath)) return "No Save File Found.";

            var savedData = File.ReadAllText(SavePath);

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
