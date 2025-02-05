using DG.Tweening;
using UnityEngine;

public class LeftHand : MonoBehaviour
{
    [SerializeField] private Transform returnTransform;
    public int endValue;
    public int duration;

    private bool hasClicked = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasClicked)
        {
            hasClicked = true;
            Vector3 targetPosition = transform.position + transform.forward * endValue;
            transform.DOMove(targetPosition, duration)
                .SetEase(Ease.Linear);
        }
    }


}
