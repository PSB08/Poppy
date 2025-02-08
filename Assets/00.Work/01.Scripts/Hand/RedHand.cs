using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HandFiring))]
public class RedHand : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

}
