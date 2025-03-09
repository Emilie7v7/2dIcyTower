using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace _Scripts.Managers
{
    public class CompressionTest : MonoBehaviour
    {
        private void Start()
        {
            string testJson = "{\"PlayerCoins\":100,\"MaxHealth\":5}";

            // Compress the JSON
            string compressedData = CompressString(testJson);
            Debug.Log("Compressed Data: " + compressedData);

            // Decompress back to JSON
            string decompressedData = DecompressString(compressedData);
            Debug.Log("Decompressed Data: " + decompressedData);

            // Check if decompression matches original JSON
            if (testJson == decompressedData)
            {
                Debug.Log("✅ Compression and Decompression WORKS correctly!");
            }
            else
            {
                Debug.LogError("❌ Compression/Decompression FAILED!");
            }
        }

        private static string CompressString(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            using MemoryStream msi = new MemoryStream(bytes);
            using MemoryStream mso = new MemoryStream();
            using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
            {
                msi.CopyTo(gs);
            }
            return System.Convert.ToBase64String(mso.ToArray());
        }

        private static string DecompressString(string compressedText)
        {
            byte[] bytes = System.Convert.FromBase64String(compressedText);
            using MemoryStream msi = new MemoryStream(bytes);
            using MemoryStream mso = new MemoryStream();
            using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                gs.CopyTo(mso);
            }
            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }
}
