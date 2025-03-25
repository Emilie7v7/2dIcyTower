using System;
using UnityEngine;

namespace _Scripts.JSON
{
    [Serializable]
    public class OptionsData
    {
        public int fpsCount;
        public bool showFps;
        public bool vSync;
        public int controlMode; // 0 = tap to throw, 1 = joysticks
        public float musicVolume;
        public float sfxVolume;
        public float uiVolume;
        
        public OptionsData()
        {
            fpsCount = 60;
            showFps = false;
            vSync = false;
            controlMode = 0; // default tap to throw
            musicVolume = 1f;
            sfxVolume = 1f;
            uiVolume = 1f;
        }
    }
    
}
