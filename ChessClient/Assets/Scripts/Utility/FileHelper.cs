using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FileHelper
{
    public static string GetWriteAblePath()
    {
        string dataPath = "";
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR)
        dataPath = UnityEngine.Application.streamingAssetsPath;
#else
        dataPath =UnityEngine.Application.persistentDataPath;
#endif
        return dataPath;
    }
    ////得到文件路径后开始操作
    //public static void OpenFile(byte[] bytes, RawImage face)
    //{
    //    Texture2D texture = new Texture2D(200, 200);
    //    texture.LoadImage(bytes);
    //    texture.Apply();
    //    TextureScale.Bilinear(texture, 200, 200);  //100*100大小
    //    face.texture = texture;
    //}

    //public static void SaveImage(RawImage face)
    //{
    //    string dataPath = GetWriteAblePath();
    //    //转换成字节流并保存在本地
    //    byte[] bytes = ((Texture2D)face.mainTexture).EncodeToJPG();
    //    string filename = dataPath + "/temp.jpg";
    //    File.WriteAllBytes(filename, bytes);
    //}
}
