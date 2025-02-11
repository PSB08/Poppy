using TMPro;
using UnityEngine;

public class OrangeHand : MonoBehaviour
{
    //주황 손, 마우스 오른쪽 클릭 시 총알 발사
    [SerializeField] private TextMeshProUGUI text;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public float fireCooldown = 3f; //3초 쿨타임 추가

    private float lastFireTime = -3f; //게임 시작 시 바로 발사 가능하도록 설정
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= lastFireTime + fireCooldown)
        {
            FireBullet();
            lastFireTime = Time.time; //마지막 발사 시간을 현재 시간으로 갱신
        }

        UpdateCooldownText();
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletPrefab.transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
    }

    private void UpdateCooldownText()
    {
        float remainingTime = Mathf.Max(0, lastFireTime + fireCooldown - Time.time);
        text.text = ""+remainingTime.ToString("F1") + "s";
    }
}
