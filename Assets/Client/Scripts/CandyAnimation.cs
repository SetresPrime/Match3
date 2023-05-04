using UnityEngine;
using DG.Tweening;
using System;
using System.Runtime.CompilerServices;

public class CandyAnimation
{
    public static Action OnEndAnim;
    private const float CANDY_FALL_TIME = 1f;
    private const float CANDY_SWIPE_TIME = 0.1f;

    public static void FallAnim(Candy candy, Vector3 targetPosition, bool IsLast)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(candy.transform.DOMove(targetPosition, CANDY_FALL_TIME));
        if (IsLast )
            sequence.AppendCallback(() => OnEndAnim.Invoke());
    }
    public static void SwipeAnim(Candy firstcandy, Candy secondcandy,bool IsBack)
    {
        var mediator = firstcandy;
        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() => 
        {
            firstcandy.transform.DOMove(secondcandy.transform.position, CANDY_SWIPE_TIME);
            secondcandy.transform.DOMove(mediator.transform.position, CANDY_SWIPE_TIME);
        });
        if (!IsBack)
            sequence.AppendCallback(() => OnEndAnim.Invoke());
    }
}
