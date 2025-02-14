using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bluehandscanner : MonoBehaviour
{
    public string targetName = "BlueHand"; // The name of the object you want to detect collisions with.
    public Animator self;
    private bool isInsideTrigger = false;
    public float triggerZoneRadius = 1.0f; // Adjust this radius based on your trigger zone size.

    public AudioSource audioSource;
    public AudioClip opendoor;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == targetName)
        {
            isInsideTrigger = true;
            self.enabled = true;
            self.SetBool("scan", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == targetName)
        {
            isInsideTrigger = false;
        }
    }

    // Check periodically if the object is still inside the trigger zone.
    private void Update()
    {
        if (isInsideTrigger)
        {
            // Check if the target object is still within the desired zone.
            Collider[] colliders = Physics.OverlapSphere(transform.position, triggerZoneRadius);
            bool found = false;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.name == targetName)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // The target object has left the trigger zone.
                isInsideTrigger = false;
                self.enabled = false;
            }
        }
    }

    public void playsound()
    {
        audioSource.PlayOneShot(opendoor, 30.0f);
    }
}
