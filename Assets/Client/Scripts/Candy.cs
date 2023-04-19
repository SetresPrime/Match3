using System;
using UnityEngine;
using UnityEngine.UI;

public class Candy : MonoBehaviour
{
    public Action<Candy> OnClick;

    [SerializeField] private Image image;
    [SerializeField] private Button button;

    public Color Color => image.color;

    public int X { get; private set; }
    public int Y { get; private set; }

    private void Awake()
    {
        button?.onClick.AddListener(() => OnClick.Invoke(this));
    }

    public void SetValue(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SpawnAnim(Field field)
    {
        CandyAnimation.SpawnCandy(this);
    }

    public void Fall(Vector3 targetPosition)
    {
        CandyAnimation.FallAnim(this, targetPosition);
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
}
