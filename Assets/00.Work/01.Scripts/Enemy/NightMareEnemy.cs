using UnityEngine;

public class NightMareEnemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float retreatDistance = 3f;
    [SerializeField] private float retreatSpeed = 5f;

    private bool isRetreating = false;
    private Vector3 retreatDirection;

    void Update()
    {
        if (player == null) return;

        if (isRetreating)
        {
            transform.position += retreatDirection * retreatSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, player.position) >= stopDistance + retreatDistance)
            {
                isRetreating = false;
            }
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            retreatDirection = (transform.position - player.position).normalized;
            isRetreating = true;
        }
    }

}
