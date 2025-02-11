using TMPro;
using UnityEngine;

public class OrangeHand : MonoBehaviour
{
    //��Ȳ ��, ���콺 ������ Ŭ�� �� �Ѿ� �߻�
    [SerializeField] private TextMeshProUGUI text;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public float fireCooldown = 3f; //3�� ��Ÿ�� �߰�

    private float lastFireTime = -3f; //���� ���� �� �ٷ� �߻� �����ϵ��� ����
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= lastFireTime + fireCooldown)
        {
            FireBullet();
            lastFireTime = Time.time; //������ �߻� �ð��� ���� �ð����� ����
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
