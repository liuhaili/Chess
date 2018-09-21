using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Utility
{
    /// <summary>  
    /// 对相机截图。   
    /// </summary>  
    /// <returns>The screenshot2.</returns>  
    /// <param name="camera">Camera.要被截屏的相机</param>  
    /// <param name="rect">Rect.截屏的区域</param>  
    public static Texture2D CaptureCamera(Camera camera, Rect rect, string savePath)
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
        //ps: camera2.targetTexture = rt;  
        //ps: camera2.Render();  
        //ps: -------------------------------------------------------------------  

        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        camera.Render();
        //ps: camera2.targetTexture = null;  
        RenderTexture.active = null; // JC: added to avoid errors  
        GameObject.DestroyImmediate(rt);
        // 最后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        string dir = Path.GetDirectoryName(savePath);
        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);
        System.IO.File.WriteAllBytes(savePath, bytes);
        //Debug.Log(string.Format("截屏了一张照片: {0}", savePath));
        return screenShot;
    }

    public static IEnumerator ShareBattleResult(string type)
    {
        int w = Screen.width * 50 / 640;
        int h = Screen.height * 57 / 360;
        Rect rect = new Rect(w, h, Screen.width - w * 2, Screen.height - h * 2);
        string filepath = Application.persistentDataPath + "/battleresult.png";

        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        //只在每一帧渲染完成后才读取屏幕信息
        yield return new WaitForEndOfFrame();
        //读取屏幕像素信息并存储为纹理数据
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();// 这一句必须有，像素信息并没有保存在2D纹理贴图中

        //读取将这些纹理数据,成一个png图片文件
        byte[] bytes = screenShot.EncodeToPNG();
        //写入文件 并且指定路径，因为使用该函数写入文件，同名的文件会被覆盖，所以，在路径中使用Time.time只是为了不出现同名文件而已, 没有什么实际的意义，只是当作个测试 
        File.WriteAllBytes(filepath, bytes);

        //分享
        if (type == "wxtimeline")
            PlatformDifferences.WXShareToTimeline("", "", "", filepath);
        else if (type == "wxsession")
            PlatformDifferences.WXShare("", "", "", filepath);
        else
            PlatformDifferences.QQShare("", "", "", filepath);
    }
}
