using UnityEngine;

public class ApplyForwardForce : MonoBehaviour
{
    public float initialForceMagnitude = 10f;
    public float delay = 3f;

    void Start()
    {
        // Apply a burst of forward force on start
        ApplyForce();

        Invoke("SelfDestruct", delay);
    }

    void ApplyForce()
    {
        // Get the rigidbody component attached to the GameObject
        Rigidbody rb = GetComponent<Rigidbody>();

        // Check if a Rigidbody component exists
        if (rb != null)
        {
            // Apply a burst of forward force to the GameObject
            rb.AddForce(transform.forward * initialForceMagnitude, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Rigidbody component not found!");
        }
    }

    void SelfDestruct()
    {
        // Destroy the GameObject this script is attached to
        Destroy(gameObject);
    }
}
