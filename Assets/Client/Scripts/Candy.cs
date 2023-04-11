using UnityEngine;
using UnityEngine.UI;

public class Candy : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public int ColorNum;

    public Image image;

    CandyAnimation candyAnimation;
    public void SetValue(int x, int y, int colorNum)
    {
        X = x;
        Y = y;
        ColorNum = colorNum;
        UpdateCandyColor();
    }
    private void Switch()
    {
        if (Y + 1!= Field.Instance.FieldSizeY)
        {
            ClearColor();
            Field.Instance.CandyFall(X, Y);
            Field.Instance.candies[X, Y + 1].Switch();
        }
        else 
        {
            Field.Instance.NewCandyAnim(this);
        }
    }
    public void UpdateCandyColor()
    {
        image.color = Field.Instance.color[ColorNum];
    }
    public void ClearColor()
    {
        image.color = Field.Instance.color[0];        
    }
    public void SetAnimation(CandyAnimation candyAnim)
    {
        candyAnimation = candyAnim;
    }
    public void CancleAnimation()
    {
        if(candyAnimation != null)
            candyAnimation.Destroy();
    }
}
