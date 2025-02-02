using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    public float launchForce = 10f; // �߻� ��
    public float returnSpeed = 5f; // ���ƿ��� �ӵ�
    private GameObject currentHand;

    void Update()
    {
        // ������ �ڵ� �߻�
        if (Input.GetMouseButtonDown(1) && currentHand == null) // ��Ŭ��
        {
            currentHand = Instantiate(Resources.Load("Right"), transform.position, Quaternion.identity) as GameObject;
            Rigidbody rb = currentHand.GetComponent<Rigidbody>();
            rb.AddForce(transform.TransformDirection(Vector3.right) * launchForce, ForceMode.Impulse);
        }

        // �ڵ尡 ���ƿ��� ���
        ReturnHand();
    }

    private void ReturnHand()
    {
        if (currentHand != null)
        {
            Vector3 returnPosition = transform.position; // �÷��̾� ��ġ�� ���ƿ�
            currentHand.transform.position = Vector3.MoveTowards(currentHand.transform.position, returnPosition, returnSpeed * Time.deltaTime);

            // �ڵ尡 �÷��̾�� �����ϸ� ����
            if (Vector3.Distance(currentHand.transform.position, returnPosition) < 0.1f)
            {
                Destroy(currentHand);
                currentHand = null;
            }
        }
    }

}
