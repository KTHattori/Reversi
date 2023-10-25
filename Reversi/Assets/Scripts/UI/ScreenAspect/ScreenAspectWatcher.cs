using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Core;
using UnityEngine;

namespace T0R1
{
    public class Orientation
    {
        public static readonly string Changed = "orientation-changed";
        public static readonly string Portrait = "orientation-portrait";
        public static readonly string Landscape = "orientation-landscape";
    }
    public class ScreenAspectWatcher : MonoSingleton<ScreenAspectWatcher>
    {
        private bool isPortrait = false;
        private bool orientationChanged = true;

        // Update is called once per frame
        void Update()
        {
            if (Screen.width < Screen.height)
            {
                // 縦画面
                if(!isPortrait)
                {
                    orientationChanged = true;
                    isPortrait = true;
                }
            }
            else
            {
                // 横画面
                if(isPortrait)
                {
                    orientationChanged = true;
                    isPortrait = false;
                }
            }

            if(orientationChanged) OnOrientationChanged(isPortrait);
        }

        public override void OnInitialize()
        {
            if (Screen.width < Screen.height)
            {
                // 縦画面
                isPortrait = true;
            }
            else
            {
                // 横画面
                isPortrait = false;
            }
            orientationChanged = true;
        }

        public override void OnFinalize()
        {
            
        }
        
        private void OnOrientationChanged(bool isPortrait)
        {
            EventDispatcher.DispatchEvent(Orientation.Changed);

            if(isPortrait)
            {
                EventDispatcher.DispatchEvent(Orientation.Portrait);
            }
            else
            {
                EventDispatcher.DispatchEvent(Orientation.Landscape);
            }
        }
    }
}
