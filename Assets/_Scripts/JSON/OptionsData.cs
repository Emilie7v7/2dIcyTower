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
        
        public OptionsData()
        {
            fpsCount = 60;
            showFps = false;
            vSync = true;
        }
    }
    
}
