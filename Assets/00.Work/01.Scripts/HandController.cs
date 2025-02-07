using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform player;
    public Transform hand;
    public float grabDistance = 5f;
    public float moveDuration = 0.5f;
    public float returnDelay = 0.2f;
    public float returnDuration = 0.5f;

    private bool isReturning = false;
    private Vector3 startOffset;
    private Tween moveTween;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startOffset = hand.position - player.position;
            Vector3 targetPosition = player.position + player.forward * grabDistance;

            moveTween = hand.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => DOVirtual.DelayedCall(returnDelay, StartReturningHand));
        }

        if (isReturning)
        {
            Vector3 returnPosition = player.position + startOffset * 0.2f;
            hand.position = Vector3.Lerp(hand.position, returnPosition, 1);
        }
    }

    private void StartReturningHand()
    {
        isReturning = true;
        DOVirtual.DelayedCall(returnDuration, () =>
        {
            isReturning = false;
        });
    }


}
