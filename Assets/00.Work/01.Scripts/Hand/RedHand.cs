using UnityEngine;

[RequireComponent(typeof(HandFiring))]
public class RedHand : MonoBehaviour
{
    //»¡°£ ¼Õ ÀûÀÌ¶û ´êÀ¸¸é Àû ÆÄ±«
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

}
