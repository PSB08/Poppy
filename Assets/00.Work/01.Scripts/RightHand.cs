using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    public float grabDistance = 5f;
    public float moveDuration = 0.5f;
    public float returnDelay = 0.2f;
    public float returnDuration = 0.5f;

    private bool hasGrabbed = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !hasGrabbed)
        {
            hasGrabbed = true;
            Vector3 targetPosition = transform.position + transform.forward * grabDistance;

            transform.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(returnDelay, ReturnHand);
                });
        }
    }

    private void ReturnHand()
    {
        Vector3 returnPosition = transform.position - transform.forward * grabDistance;

        transform.DOMove(returnPosition, returnDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                hasGrabbed = false;
            });
    }

}
