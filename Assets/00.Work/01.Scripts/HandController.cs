using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform grabPack; // 씬에 있는 그랩팩 오브젝트
    public float throwForce = 10f; // 날리는 힘
    public float returnSpeed = 5f; // 돌아오는 속도
    public float maxDistance = 10f; // 최대 날아가는 거리

    private Vector3 initialPosition; // 그랩팩의 초기 위치
    private bool isThrown = false; // 그랩팩이 날아가고 있는지 여부
    private bool isReturning = false; // 그랩팩이 돌아오고 있는지 여부

    void Start()
    {
        // 그랩팩의 초기 위치 저장
        initialPosition = grabPack.position;
    }

    void Update()
    {
        // 그랩팩 발사
        if (Input.GetMouseButtonDown(0) && !isThrown) // 마우스 왼쪽 클릭
        {
            ThrowGrabPack();
        }

        // 그랩팩 돌아오기
        if (Input.GetMouseButtonDown(1) && isThrown) // 마우스 오른쪽 클릭
        {
            ReturnGrabPack();
        }

        // 그랩팩이 돌아오는 중인 경우
        if (isReturning)
        {
            grabPack.position = Vector3.MoveTowards(grabPack.position, initialPosition, returnSpeed * Time.deltaTime);

            // 그랩팩이 초기 위치에 도달하면 상태 초기화
            if (grabPack.position == initialPosition)
            {
                isThrown = false;
                isReturning = false;
            }
        }
    }

    void ThrowGrabPack()
    {
        isThrown = true;

        // 그랩팩을 앞으로 날림
        Vector3 throwDirection = transform.forward;
        grabPack.GetComponent<Rigidbody>().AddForce(throwDirection * throwForce, ForceMode.Impulse);

        // 최대 거리까지 날아가도록 제한
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        while (isThrown)
        {
            // 그랩팩이 최대 거리를 넘어가면 돌아오기 시작
            if (Vector3.Distance(grabPack.position, initialPosition) >= maxDistance)
            {
                ReturnGrabPack();
                yield break;
            }
            yield return null;
        }
    }

    void ReturnGrabPack()
    {
        isReturning = true;

        // 그랩팩의 물리 효과 비활성화
        grabPack.GetComponent<Rigidbody>().velocity = Vector3.zero;
        grabPack.GetComponent<Rigidbody>().isKinematic = true;
    }

}
