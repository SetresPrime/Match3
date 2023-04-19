using UnityEngine;
using DG.Tweening;

public class CandyAnimation
{
    private const float CANDY_SPAWN_TIME = 0.1f;
    private const float CANDY_FALL_TIME = 0.5f;

    public static void FallAnim(Candy candy, Vector3 targetPosition)
    {
        candy.transform.DOMove(targetPosition, CANDY_FALL_TIME);
    }

    public static void SpawnCandy(Candy candy)
    {
        var targetPosition = candy.transform.position;

        candy.transform.position += new Vector3(0, 1.444f, 0);
        candy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        var sequence = DOTween.Sequence();

        sequence.Append(candy.transform.DOScale(1.2f, CANDY_SPAWN_TIME))
                .AppendInterval(0.05f)
                .Append(candy.transform.DOScale(1.0f, CANDY_SPAWN_TIME))
                .Append(candy.transform.DOMove(targetPosition, 0.3f));
    }
}
