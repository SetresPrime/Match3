using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Candy : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int colorNum;
    public Image image;
    public void SetValue(int x, int y, Color color)
    {
        X = x;
        Y = y;
        image.color = color;
    }

}
