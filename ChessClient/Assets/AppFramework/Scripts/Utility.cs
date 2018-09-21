using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Utility
    {
        public static string GetWriteAblePath()
        {
            string pathURL = "";
#if UNITY_ANDROID   //安卓
                pathURL= Application.dataPath + "!/assets/";
#elif UNITY_IPHONE  //iPhone
                pathURL=Application.dataPath + "/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR  //windows平台和web平台
            pathURL = Application.dataPath + "/StreamingAssets/";
#else
                pathURL =string.Empty;
#endif
            return pathURL;
        }
    }
}
