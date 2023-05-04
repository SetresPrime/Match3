using System;
using UnityEngine;
using UnityEngine.UI;

public class Candy : MonoBehaviour
{

    [SerializeField] private Image image;
    public Action<Candy> OnSelected;
    public Color Color => image.color;

    public int X { get; private set; }
    public int Y { get; private set; }
    public int color {get; private set; }
    public void SetValue(int x, int y, int Color)
    {
        X = x;
        Y = y;
        color = Color;
    }
    public void SelectCandy()
    {
        OnSelected.Invoke(this);
    }
    public void Fall(Vector3 targetPosition, bool IsLast)
    {
        CandyAnimation.FallAnim(this, targetPosition, IsLast);
    }
    public void SetColor(Color color)
    {
        image.color = color;
    }
}
