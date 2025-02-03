using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public float launchForce = 10f; // 발사 힘
    public float returnSpeed = 5f; // 돌아오는 속도
    public GameObject leftGrabPackPrefab; // 왼손 그랩팩 프리팹
    public GameObject rightGrabPackPrefab; // 오른손 그랩팩 프리팹

    private GameObject leftGrabPack; // 현재 발사된 왼손 그랩팩
    private GameObject rightGrabPack; // 현재 발사된 오른손 그랩팩
    private bool isLeftGrabbing = false; // 왼손 그랩팩이 물체를 잡고 있는지 여부
    private bool isRightGrabbing = false; // 오른손 그랩팩이 물체를 잡고 있는지 여부

    void Update()
    {
        // 왼손 그랩팩 발사
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            LaunchGrabPack(ref leftGrabPack, leftGrabPackPrefab, ref isLeftGrabbing);
        }

        // 오른손 그랩팩 발사
        if (Input.GetMouseButtonDown(1)) // 마우스 오른쪽 클릭
        {
            LaunchGrabPack(ref rightGrabPack, rightGrabPackPrefab, ref isRightGrabbing);
        }

        // 왼손 그랩팩 돌아오기
        if (leftGrabPack != null && !isLeftGrabbing)
        {
            ReturnGrabPack(ref leftGrabPack);
        }

        // 오른손 그랩팩 돌아오기
        if (rightGrabPack != null && !isRightGrabbing)
        {
            ReturnGrabPack(ref rightGrabPack);
        }
    }

    void LaunchGrabPack(ref GameObject grabPack, GameObject prefab, ref bool isGrabbing)
    {
        if (grabPack != null) return; // 이미 발사된 그랩팩이 있으면 무시

        // 그랩팩 발사
        grabPack = Instantiate(prefab, transform.position, transform.rotation);
        Rigidbody rb = grabPack.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);

        // 물체를 잡을 수 있도록 코루틴 시작
        StartCoroutine(DetectGrab(grabPack, isGrabbing));
    }

    IEnumerator DetectGrab(GameObject grabPack, bool isGrabbing)
    {
        while (grabPack != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(grabPack.transform.position, grabPack.transform.forward, out hit, 1f))
            {
                if (hit.collider.CompareTag("Grabbable")) // "Grabbable" 태그가 있는 물체만 잡기
                {
                    GrabObject(grabPack, hit.collider.gameObject, ref isGrabbing);
                    yield break; // 물체를 잡으면 코루틴 종료
                }
            }
            yield return null; // 다음 프레임까지 대기
        }
    }

    void GrabObject(GameObject grabPack, GameObject target, ref bool isGrabbing)
    {
        isGrabbing = true;

        // 물체를 그랩팩에 고정 (Fixed Joint 사용)
        FixedJoint joint = grabPack.AddComponent<FixedJoint>();
        joint.connectedBody = target.GetComponent<Rigidbody>();

        Debug.Log("Object Grabbed: " + target.name);
    }

    void ReturnGrabPack(ref GameObject grabPack)
    {
        if (grabPack == null) return;

        // 그랩팩이 플레이어 쪽으로 돌아오도록 이동
        Vector3 direction = (transform.position - grabPack.transform.position).normalized;
        grabPack.GetComponent<Rigidbody>().velocity = direction * returnSpeed;

        // 그랩팩이 플레이어 근처에 도달하면 제거
        if (Vector3.Distance(grabPack.transform.position, transform.position) < 1f)
        {
            Destroy(grabPack);
            grabPack = null;
        }
    }


}
