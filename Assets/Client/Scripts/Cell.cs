using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public void SetValue(int x, int y)
    {
        X = x;
        Y = y;
    }
}
