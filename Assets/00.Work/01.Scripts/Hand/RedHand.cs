using UnityEngine;

[RequireComponent(typeof(HandFiring))]
public class RedHand : MonoBehaviour
{
    //���� �� ���̶� ������ �� �ı�
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

}
