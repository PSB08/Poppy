using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NightMareEnemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject handObj;
    [SerializeField] private MonoBehaviour mouseLookScript;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float bulletDetectionRange = 3f;
    [SerializeField] private float retreatDistance = 1.5f; // �ѿ� �¾��� �� �ڷ� ���� �Ÿ�
    [SerializeField] private float retreatDuration = 0.3f; // ���� �ð�

    private bool isCutsceneActive = false;

    private bool isRetreating = false;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>(); // NavMeshAgent ��������
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

        if (player == null || isCutsceneActive || isRetreating) return;

        DetectNearbyBullets();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // ���� ���� �Ÿ� ������ ������ �ƽ� ����
            StartCoroutine(KillPlayerCutscene());
        }
    }

    private void DetectNearbyBullets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, bulletDetectionRange); // ���� �� ������Ʈ ����

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Bullet")) // ������ ������Ʈ�� �Ѿ����� Ȯ��
            {
                RetreatFromPlayer(); // ���� ����
                break; // �� �� �����ϸ� �� �̻� Ȯ���� �ʿ� ����
            }
        }
    }

    private void RetreatFromPlayer()
    {
        if (player == null) return;

        isRetreating = true; // ���� ���� Ȱ��ȭ

        // �÷��̾� �ݴ� �������� �̵�
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatPosition = transform.position + retreatDirection * retreatDistance;

        // Y�� �̵��� ������ ���� ���� Y���� ������ ���·� ���ο� ���� ����
        retreatPosition = new Vector3(retreatPosition.x, transform.position.y, retreatPosition.z);

        // DOTween�� ����� XZ ��ǥ�� �̵� (Y���� �״�� ����)
        transform.DOMoveX(retreatPosition.x, retreatDuration);
        transform.DOMoveZ(retreatPosition.z, retreatDuration).OnComplete(() =>
        {
            isRetreating = false; // ���� �Ϸ� �� �ٽ� ������ �� �ֵ��� ����
        });
    }


    private IEnumerator KillPlayerCutscene()
    {
        isCutsceneActive = true; // �ƽ� Ȱ��ȭ

        handObj.SetActive(false);

        // 1. �÷��̾� ���� ��Ȱ��ȭ
        if (player.GetComponent<CharacterController>())
            player.GetComponent<CharacterController>().enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
 
        Vector3 targetPosition = transform.position - transform.forward * -3f; // ���� ��ġ ��ó�� ī�޶� �̵�
        Camera.main.transform.DOMove(targetPosition, 0.1f); // ī�޶� �̵�

        // ī�޶� ���� �ٶ󺸵��� ȸ��
        Camera.main.transform.DOLookAt(transform.position, 0.3f);

        yield return new WaitForSeconds(0.3f); // ī�޶� �̵��� ȸ���ϴ� �ð�

        // 3. ���� ����
        transform.DOShakePosition(1.5f, 0.2f, 10, 90, false, true);

        yield return new WaitForSeconds(1.5f); // ���� ��鸮�� �ð�

        // 3. �÷��̾� ����
        SceneManager.LoadScene("GameOver");
    }


}
