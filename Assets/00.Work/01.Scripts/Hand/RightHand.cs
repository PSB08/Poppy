using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RightHand : MonoBehaviour
{
    public Transform[] hands;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f; 

    private int currentHandIndex = 0;
    private Transform activeHand;

    private void Start()
    {
        ChangeHand(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeHand(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeHand(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeHand(2);
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

}
