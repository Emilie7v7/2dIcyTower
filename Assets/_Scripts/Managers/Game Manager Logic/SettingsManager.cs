using System.IO;
using _Scripts.Generics;
using _Scripts.JSON;
using _Scripts.Managers.Save_System_Logic;
using UnityEngine;

namespace _Scripts.Managers.Game_Manager_Logic
{
    public static class SettingsManager
    {
        public static OptionsData OptionsData { get; private set; }
        
        static SettingsManager()
        {
            LoadOptionsData();
        }

        #region Options Game Data

        public static void SaveOptionsData()
        {
            SaveSystem.SaveOptionsData(OptionsData);
        }

        public static void LoadOptionsData()
        {
            if (File.Exists(SaveSystem.OptionsDataSavePath))
            {
                OptionsData = SaveSystem.LoadOptionsData() ?? new OptionsData();
                Debug.Log("Options Loaded Successfully!");
            }
            else
            {
                Debug.Log("No Options File Found, Creating Default...");
                OptionsData = new OptionsData();
                SaveOptionsData(); // Ensure it's created
            }
            ApplySettings();
        }

        public static void ResetOptionsData()
        {
            SaveSystem.DeleteOptionsData();
            OptionsData = new OptionsData();
            
            SaveOptionsData();
            LoadOptionsData();
        }
        #endregion

        public static void ApplySettings()
        {
            QualitySettings.vSyncCount = OptionsData.vSync ? 1 : 0;
            Application.targetFrameRate = OptionsData.fpsCount; // Apply FPS cap **after** VSync
    
            Debug.Log($"✅ Settings Applied: FPS = {(OptionsData.fpsCount == -1 ? "Unlimited" : OptionsData.fpsCount)}, VSync = {(OptionsData.vSync ? "Enabled" : "Disabled")}");

            // 🔥 Force a Quality Settings Refresh to ensure Unity updates VSync
            QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel(), true);
        }

        // UI Methods to update settings dynamically
        public static void SetFPS(int fps)
        {
            OptionsData.fpsCount = fps;
            SaveOptionsData();
            ApplySettings();
        }

        public static void SetShowFPS(bool show)
        {
            OptionsData.showFps = show;
            SaveOptionsData();
            FPSCounter.SetVisibility(show);
        }
        
        public static void SetVSync(bool enabled)
        {
            OptionsData.vSync = enabled;
            SaveOptionsData();
            ApplySettings();
        }
    }
}
