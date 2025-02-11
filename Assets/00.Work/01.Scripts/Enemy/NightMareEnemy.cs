using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightMareEnemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private MonoBehaviour mouseLookScript;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float retreatDistance = 1.5f; // �ѿ� �¾��� �� �ڷ� ���� �Ÿ�
    [SerializeField] private float retreatDuration = 0.3f; // ���� �ð�

    private bool isCutsceneActive = false;

    private bool isRetreating = false;


    void Update()
    {
        if (player == null || isCutsceneActive || isRetreating) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            // �÷��̾ ����
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            // ���� ���� �Ÿ� ������ ������ �ƽ� ����
            StartCoroutine(KillPlayerCutscene());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")) // �Ѿ˰� �浹�ϸ�
        {
            RetreatFromPlayer(); // �ڷ� �з���
        }
    }
    private void RetreatFromPlayer()
    {
        if (player == null) return;

        isRetreating = true; // ���� ���� Ȱ��ȭ

        // �÷��̾� �ݴ� �������� �̵�
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatPosition = transform.position + retreatDirection * retreatDistance;

        // DOTween�� ����Ͽ� �ε巴�� ����
        transform.DOMove(retreatPosition, retreatDuration).OnComplete(() =>
        {
            isRetreating = false; // ���� �Ϸ� �� �ٽ� ������ �� �ֵ��� ����
        });
    }


    private IEnumerator KillPlayerCutscene()
    {
        isCutsceneActive = true; // �ƽ� Ȱ��ȭ

        // 1. �÷��̾� ���� ��Ȱ��ȭ
        if (player.GetComponent<CharacterController>())
            player.GetComponent<CharacterController>().enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. ���� �÷��̾�(ī�޶�) �߾����� �̵�
        Vector3 targetPosition = player.position + player.forward * 1.5f; // ī�޶� �� 1.5m
        transform.DOMove(targetPosition, 0.1f);

        yield return new WaitForSeconds(0.1f); // ���� �߾����� �̵��ϴ� �ð�

        // 3. ���� ����
        transform.DOShakePosition(1.5f, 0.2f, 10, 90, false, true);

        yield return new WaitForSeconds(1.5f); // ���� ��鸮�� �ð�

        // 3. �÷��̾� ����
        SceneManager.LoadScene("GameOver");
    }


}
