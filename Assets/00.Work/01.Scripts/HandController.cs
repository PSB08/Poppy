using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform grabPack;
    public float throwForce = 10f;
    public float returnSpeed = 5f; 
    public float maxDistance = 10f;

    public Vector3 initialPosition; 
    private bool isThrown = false; 
    private bool isReturning = false;

    private void Start()
    {
        initialPosition = grabPack.position;
    }

    private void Update()
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

    private void ThrowGrabPack()
    {
        isThrown = true;

        // 그랩팩을 앞으로 날림
        Vector3 throwDirection = transform.forward;
        grabPack.GetComponent<Rigidbody>().AddForce(throwDirection * throwForce, ForceMode.Impulse);

        // 최대 거리까지 날아가도록 제한
        StartCoroutine(CheckDistance());
    }

    private IEnumerator CheckDistance()
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

    private void ReturnGrabPack()
    {
        isReturning = true;

        grabPack.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

}
