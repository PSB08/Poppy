using UnityEngine;

public class OrangeHand : MonoBehaviour
{
    //주황 손, 마우스 오른쪽 클릭 시 총알 발사
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            FireBullet();
        }
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletPrefab.transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
    }

}
