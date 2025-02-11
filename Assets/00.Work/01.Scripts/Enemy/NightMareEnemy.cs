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

    private bool isCutsceneActive = false;

    void Update()
    {
        if (player == null || isCutsceneActive) return;

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

    private IEnumerator KillPlayerCutscene()
    {
        isCutsceneActive = true; // �ƽ� Ȱ��ȭ

        // 1. �÷��̾� ���� ��Ȱ��ȭ
        if (player.GetComponent<CharacterController>())
            player.GetComponent<CharacterController>().enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

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
