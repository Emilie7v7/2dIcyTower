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
        public ControlModes controlMode;
        public float musicVolume;
        public float sfxVolume;
        public float uiVolume;
        public bool hideControlsPopUp;
        public enum ControlModes { Joystick, Touchscreen}
        
        public OptionsData()
        {
            fpsCount = 60;
            showFps = false;
            vSync = false;
            musicVolume = 1f;
            sfxVolume = 1f;
            uiVolume = 1f;
            controlMode = ControlModes.Touchscreen;
            hideControlsPopUp = false;
        }
    }
    
}
