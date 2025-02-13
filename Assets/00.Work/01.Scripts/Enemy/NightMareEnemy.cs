using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NightMareEnemy : MonoBehaviour
{
    #region Value
    [SerializeField] private Transform player;
    [SerializeField] private GameObject handObj;
    [SerializeField] private MonoBehaviour mouseLookScript;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float bulletDetectionRange = 3f;
    [SerializeField] private float retreatDistance = 1.5f;
    [SerializeField] private float retreatDuration = 0.3f;
    [SerializeField] private float chaseStartDistance = 10f; // 추적 시작 거리

    private bool isCutsceneActive = false;
    private bool isRetreating = false;
    private bool isChasing = false; // 추적 여부
    #endregion

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

        if (player == null || isCutsceneActive || isRetreating) return;

        DetectNearbyBullets();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseStartDistance) isChasing = true;
        if (distanceToPlayer > chaseStartDistance + 2f) isChasing = false; // 일정 거리 벗어나면 멈춤

        if (isChasing)
        {
            if (distanceToPlayer > stopDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                StartCoroutine(KillPlayerCutscene());
            }
        }
    }

    private void DetectNearbyBullets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, bulletDetectionRange); 

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Bullet")) 
            {
                RetreatFromPlayer(); 
                break; 
            }
        }
    }

    private void RetreatFromPlayer()
    {
        if (player == null) return;

        isRetreating = true; // 후퇴 상태 활성화

        // 플레이어 반대 방향으로 이동
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatPosition = transform.position + retreatDirection * retreatDistance;

        // Y축 이동을 완전히 막기 위해 Y값을 유지한 상태로 새로운 벡터 생성
        retreatPosition = new Vector3(retreatPosition.x, transform.position.y, retreatPosition.z);

        // DOTween을 사용해 XZ 좌표만 이동 (Y값은 그대로 유지)
        transform.DOMoveX(retreatPosition.x, retreatDuration);
        transform.DOMoveZ(retreatPosition.z, retreatDuration).OnComplete(() =>
        {
            isRetreating = false; // 후퇴 완료 후 다시 움직일 수 있도록 설정
        });
    }


    private IEnumerator KillPlayerCutscene()
    {
        isCutsceneActive = true; // 컷신 활성화

        handObj.SetActive(false);

        // 1. 플레이어 조작 비활성화
        if (player.GetComponent<CharacterController>())
            player.GetComponent<CharacterController>().enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
 
        Vector3 targetPosition = transform.position - transform.forward * -3f; // 적의 위치 근처로 카메라 이동
        yield return Camera.main.transform.DOMove(targetPosition, 0.1f).WaitForCompletion(); // 카메라 이동

        // 카메라가 적을 바라보도록 회전
        yield return Camera.main.transform.DOLookAt(transform.position, 0.3f).WaitForCompletion();

        yield return new WaitForSeconds(0.3f); // 카메라 이동과 회전하는 시간

        // 3. 적을 흔들기
        transform.DOShakePosition(1.5f, 0.2f, 10, 90, false, true);

        yield return new WaitForSeconds(1.5f); // 적이 흔들리는 시간

        // 3. 플레이어 제거
        SceneManager.LoadScene("GameOver");
    }


}
