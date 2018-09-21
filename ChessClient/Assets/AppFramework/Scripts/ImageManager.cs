using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ImageManager : MonoBehaviour 
{
    public List<Sprite> ImageList=new List<Sprite>();

    public Sprite Get(string name)
    {
        return ImageList.FirstOrDefault(c => c.name == name);
    }
}
