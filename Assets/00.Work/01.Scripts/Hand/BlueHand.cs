using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(HandFiring))]
public class BlueHand : MonoBehaviour
{
    public Transform hand;
    public float grabDistance = 5f;
    public LayerMask grabbableLayer;
    public float throwForce = 5f;
    public float disableColliderTime = 0.2f;

    private bool hasGrabbed = false;
    private GameObject grabbedObject = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && grabbedObject != null)
        {
            ReleaseObject();
        }

        if (hasGrabbed && grabbedObject != null)
        {
            grabbedObject.transform.position = hand.position;
        }

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
