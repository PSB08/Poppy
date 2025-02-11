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
    [SerializeField] private float retreatDistance = 1.5f; // 총에 맞았을 때 뒤로 가는 거리
    [SerializeField] private float retreatDuration = 0.3f; // 후퇴 시간

    private bool isCutsceneActive = false;

    private bool isRetreating = false;


    void Update()
    {
        if (player == null || isCutsceneActive || isRetreating) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            // 플레이어를 추적
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            // 적이 일정 거리 안으로 들어오면 컷신 실행
            StartCoroutine(KillPlayerCutscene());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")) // 총알과 충돌하면
        {
            RetreatFromPlayer(); // 뒤로 밀려남
        }
    }
    private void RetreatFromPlayer()
    {
        if (player == null) return;

        isRetreating = true; // 후퇴 상태 활성화

        // 플레이어 반대 방향으로 이동
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatPosition = transform.position + retreatDirection * retreatDistance;

        // DOTween을 사용하여 부드럽게 후퇴
        transform.DOMove(retreatPosition, retreatDuration).OnComplete(() =>
        {
            isRetreating = false; // 후퇴 완료 후 다시 움직일 수 있도록 설정
        });
    }


    private IEnumerator KillPlayerCutscene()
    {
        isCutsceneActive = true; // 컷신 활성화

        // 1. 플레이어 조작 비활성화
        if (player.GetComponent<CharacterController>())
            player.GetComponent<CharacterController>().enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. 적을 플레이어(카메라) 중앙으로 이동
        Vector3 targetPosition = player.position + player.forward * 1.5f; // 카메라 앞 1.5m
        transform.DOMove(targetPosition, 0.1f);

        yield return new WaitForSeconds(0.1f); // 적이 중앙으로 이동하는 시간

        // 3. 적을 흔들기
        transform.DOShakePosition(1.5f, 0.2f, 10, 90, false, true);

        yield return new WaitForSeconds(1.5f); // 적이 흔들리는 시간

        // 3. 플레이어 제거
        SceneManager.LoadScene("GameOver");
    }


}
