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
        // �׷��� �߻�
        if (Input.GetMouseButtonDown(0) && !isThrown) // ���콺 ���� Ŭ��
        {
            ThrowGrabPack();
        }

        // �׷��� ���ƿ���
        if (Input.GetMouseButtonDown(1) && isThrown) // ���콺 ������ Ŭ��
        {
            ReturnGrabPack();
        }

        // �׷����� ���ƿ��� ���� ���
        if (isReturning)
        {
            grabPack.position = Vector3.MoveTowards(grabPack.position, initialPosition, returnSpeed * Time.deltaTime);

            // �׷����� �ʱ� ��ġ�� �����ϸ� ���� �ʱ�ȭ
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

        // �׷����� ������ ����
        Vector3 throwDirection = transform.forward;
        grabPack.GetComponent<Rigidbody>().AddForce(throwDirection * throwForce, ForceMode.Impulse);

        // �ִ� �Ÿ����� ���ư����� ����
        StartCoroutine(CheckDistance());
    }

    private IEnumerator CheckDistance()
    {
        while (isThrown)
        {
            // �׷����� �ִ� �Ÿ��� �Ѿ�� ���ƿ��� ����
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
