using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform grabPack; // ���� �ִ� �׷��� ������Ʈ
    public float throwForce = 10f; // ������ ��
    public float returnSpeed = 5f; // ���ƿ��� �ӵ�
    public float maxDistance = 10f; // �ִ� ���ư��� �Ÿ�

    private Vector3 initialPosition; // �׷����� �ʱ� ��ġ
    private bool isThrown = false; // �׷����� ���ư��� �ִ��� ����
    private bool isReturning = false; // �׷����� ���ƿ��� �ִ��� ����

    void Start()
    {
        // �׷����� �ʱ� ��ġ ����
        initialPosition = grabPack.position;
    }

    void Update()
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

    void ThrowGrabPack()
    {
        isThrown = true;

        // �׷����� ������ ����
        Vector3 throwDirection = transform.forward;
        grabPack.GetComponent<Rigidbody>().AddForce(throwDirection * throwForce, ForceMode.Impulse);

        // �ִ� �Ÿ����� ���ư����� ����
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
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

    void ReturnGrabPack()
    {
        isReturning = true;

        // �׷����� ���� ȿ�� ��Ȱ��ȭ
        grabPack.GetComponent<Rigidbody>().velocity = Vector3.zero;
        grabPack.GetComponent<Rigidbody>().isKinematic = true;
    }

}
