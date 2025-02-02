using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Transform leftHand; // �޼� ������Ʈ
    [SerializeField] private Transform rightHand; // ������ ������Ʈ
    [SerializeField] private Transform lReturnTransform; 
    [SerializeField] private Transform rReturnTransform; 
    public int endValue;
    public float duration;

    private bool leftHandClicked = false;
    private bool rightHandClicked = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !leftHandClicked) // ���� Ŭ��
        {
            leftHandClicked = true;
            Vector3 targetPosition = leftHand.position + leftHand.forward * endValue;

            leftHand.DOMove(targetPosition, duration)
                .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    leftHandClicked = false;
                });
        }

        if (Input.GetMouseButtonDown(1) && !rightHandClicked) // ������ Ŭ��
        {
            rightHandClicked = true;
            Vector3 targetPosition = rightHand.position + rightHand.forward * endValue;

            rightHand.DOMove(targetPosition, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (rReturnTransform != null)
                    {
                        rightHand.DOMove(rReturnTransform.position, duration)
                            .SetEase(Ease.Linear)
                            .OnComplete(() => rightHandClicked = false);
                    }
                    else
                    {
                        rightHandClicked = false;
                    }
                });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            other.transform.SetParent(null);
        }
    }


}
