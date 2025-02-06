using DG.Tweening;
using UnityEngine;

public class LeftHand : MonoBehaviour
{
    public Transform player;
    public Transform hand;
    public float grabDistance = 5f;
    public float moveDuration = 0.5f;
    public float returnDelay = 0.2f;
    public float returnDuration = 0.5f;
    public LayerMask grabbableLayer;
    public float throwForce = 5f;
    public float disableColliderTime = 0.2f;

    private bool hasGrabbed = false;
    private bool isReturning = false;
    private GameObject grabbedObject = null;
    private Vector3 startOffset;
    private Tween moveTween;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasGrabbed)
        {
            hasGrabbed = true;
            startOffset = hand.position - player.position;
            Vector3 targetPosition = player.position + player.forward * grabDistance;

            moveTween = hand.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => DOVirtual.DelayedCall(returnDelay, StartReturningHand));
        }

        if (Input.GetKeyDown(KeyCode.E) && grabbedObject != null)
        {
            ReleaseObject();
        }

        if (hasGrabbed && grabbedObject != null)
        {
            grabbedObject.transform.position = hand.position;
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
            hasGrabbed = false;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (grabbedObject == null && ((1 << other.gameObject.layer) & grabbableLayer) != 0)
        {
            grabbedObject = other.gameObject;
            grabbedObject.transform.SetParent(hand);
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.SetParent(null);
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            Collider objCollider = grabbedObject.GetComponent<Collider>();

            rb.isKinematic = false;
            rb.AddForce(Vector3.up * throwForce, ForceMode.Impulse);

            objCollider.enabled = false;
            DOVirtual.DelayedCall(disableColliderTime, () => objCollider.enabled = true);

            grabbedObject = null;
        }
    }


}
