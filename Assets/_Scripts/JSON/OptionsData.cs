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
        
        public OptionsData()
        {
            fpsCount = 60;
            showFps = false;
            vSync = true;
            controlMode = 0; // default tap to throw
        }
    }
    
}
