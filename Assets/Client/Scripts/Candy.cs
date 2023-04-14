using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class Candy : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public int ColorNum;

    public Image image;

    CandyAnimation candyAnimation;

    [SerializeField]
    private CandyAnimation animationPref;
    public void SetValue(int x, int y, int colorNum)
    {
        X = x;
        Y = y;
        ColorNum = colorNum;
    }
    public void trans()
    {
        gameObject.transform.position = new Vector3(0,0, 0);
    }
    public void SpawnAnim()
    {
        Instantiate(animationPref, Field.Instance.transform, false).NewCandyAnim(this);
    }
    public void Fall()
    {
        if (Y + 1 != Field.Instance.FieldSizeY)
        {
            ClearColor();
            Instantiate(animationPref, Field.Instance.transform, false).FallAnim(Field.Instance.candies[X, Y + 1], this);
            Field.Instance.candies[X, Y + 1].Fall();
        }
        else
        {
            ColorNum = Random.Range(1, Field.Instance.color.Length);
            SpawnAnim();
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
