using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CandyAnimation : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    private float newCandy = 0.1f;
    private float moveTime = 0.5f;

    public Image image;

    Sequence sequence;
    public void Fall(Candy from, Candy to)
    {
        to.ColorNum = from.ColorNum;    
        
        image.color = Field.Instance.color[from.ColorNum];
        transform.position = from.transform.position;

        sequence = DOTween.Sequence();

        from.ClearColor();

        sequence.Append(transform.DOMove(to.transform.position, moveTime));

        sequence.AppendCallback(() =>
        {
            to.UpdateCandyColor();
            Destroy();
        });
    }
    public void NewCandyAnim(Candy candy)
    {
        candy.image.color = Field.Instance.color[0];
        candy.ColorNum = Random.Range(1, Field.Instance.color.Length);

        image.color = Field.Instance.color[candy.ColorNum];

        transform.position = new Vector3(0, 1.444257f, 0) + candy.transform.position;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(1.2f, newCandy));
        sequence.AppendInterval(0.05f);
        sequence.Append(transform.DOScale(1.0f, newCandy));

        sequence.Append(transform.DOMove(candy.transform.position, 0.3f));

        sequence.AppendCallback(() =>
        {
            candy.UpdateCandyColor();
            Destroy();
        });
    }
    public void Destroy()
    {
        sequence.Kill();
        Destroy(gameObject);        
    }
}
