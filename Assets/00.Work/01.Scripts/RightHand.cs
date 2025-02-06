using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RightHand : MonoBehaviour
{
    public Transform player;
    public Transform hand;
    public Transform[] hands;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float grabDistance = 5f;
    public float moveDuration = 0.5f;
    public float returnDelay = 0.2f;
    public float returnDuration = 0.5f;
    public float bulletSpeed = 20f; 
    public float throwForce = 5f;
    public float disableColliderTime = 0.2f;

    private bool hasGrabbed = false;
    private bool isReturning = false;
    private GameObject grabbedObject = null;
    private Vector3 startOffset;
    private Tween moveTween;
    private int currentHandIndex = 0;
    private Transform activeHand;
    private Tween returnTween;

    private void Start()
    {
        ChangeHand(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeHand(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeHand(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeHand(2);

        if (Input.GetMouseButtonDown(1))
        {
            if (currentHandIndex == 0 || currentHandIndex == 2) LaunchHand();
            if (currentHandIndex == 1) FireBullet();
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

    private void LaunchHand()
    {
        if (!hasGrabbed)
        {
            hasGrabbed = true;
            startOffset = hand.position - player.position;
            Vector3 targetPosition = player.position + player.forward * grabDistance;

            moveTween = hand.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => DOVirtual.DelayedCall(returnDelay, ReturnHand));
        }
        
    }

    private void ReturnHand()
    {
        isReturning = true;
        DOVirtual.DelayedCall(returnDuration, () =>
        {
            isReturning = false;
            hasGrabbed = false;
        });
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletPrefab.transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
    }

    private void ChangeHand(int index)
    {
        if (index == currentHandIndex) return;

        foreach (Transform h in hands) h.gameObject.SetActive(false);

        hands[index].gameObject.SetActive(true);
        activeHand = hands[index];
        currentHandIndex = index;
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
