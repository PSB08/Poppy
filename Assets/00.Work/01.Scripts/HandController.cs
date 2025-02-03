using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public float launchForce = 10f; // �߻� ��
    public float returnSpeed = 5f; // ���ƿ��� �ӵ�
    public GameObject leftGrabPackPrefab; // �޼� �׷��� ������
    public GameObject rightGrabPackPrefab; // ������ �׷��� ������

    private GameObject leftGrabPack; // ���� �߻�� �޼� �׷���
    private GameObject rightGrabPack; // ���� �߻�� ������ �׷���
    private bool isLeftGrabbing = false; // �޼� �׷����� ��ü�� ��� �ִ��� ����
    private bool isRightGrabbing = false; // ������ �׷����� ��ü�� ��� �ִ��� ����

    void Update()
    {
        // �޼� �׷��� �߻�
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� Ŭ��
        {
            LaunchGrabPack(ref leftGrabPack, leftGrabPackPrefab, ref isLeftGrabbing);
        }

        // ������ �׷��� �߻�
        if (Input.GetMouseButtonDown(1)) // ���콺 ������ Ŭ��
        {
            LaunchGrabPack(ref rightGrabPack, rightGrabPackPrefab, ref isRightGrabbing);
        }

        // �޼� �׷��� ���ƿ���
        if (leftGrabPack != null && !isLeftGrabbing)
        {
            ReturnGrabPack(ref leftGrabPack);
        }

        // ������ �׷��� ���ƿ���
        if (rightGrabPack != null && !isRightGrabbing)
        {
            ReturnGrabPack(ref rightGrabPack);
        }
    }

    void LaunchGrabPack(ref GameObject grabPack, GameObject prefab, ref bool isGrabbing)
    {
        if (grabPack != null) return; // �̹� �߻�� �׷����� ������ ����

        // �׷��� �߻�
        grabPack = Instantiate(prefab, transform.position, transform.rotation);
        Rigidbody rb = grabPack.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);

        // ��ü�� ���� �� �ֵ��� �ڷ�ƾ ����
        StartCoroutine(DetectGrab(grabPack, isGrabbing));
    }

    IEnumerator DetectGrab(GameObject grabPack, bool isGrabbing)
    {
        while (grabPack != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(grabPack.transform.position, grabPack.transform.forward, out hit, 1f))
            {
                if (hit.collider.CompareTag("Grabbable")) // "Grabbable" �±װ� �ִ� ��ü�� ���
                {
                    GrabObject(grabPack, hit.collider.gameObject, ref isGrabbing);
                    yield break; // ��ü�� ������ �ڷ�ƾ ����
                }
            }
            yield return null; // ���� �����ӱ��� ���
        }
    }

    void GrabObject(GameObject grabPack, GameObject target, ref bool isGrabbing)
    {
        isGrabbing = true;

        // ��ü�� �׷��ѿ� ���� (Fixed Joint ���)
        FixedJoint joint = grabPack.AddComponent<FixedJoint>();
        joint.connectedBody = target.GetComponent<Rigidbody>();

        Debug.Log("Object Grabbed: " + target.name);
    }

    void ReturnGrabPack(ref GameObject grabPack)
    {
        if (grabPack == null) return;

        // �׷����� �÷��̾� ������ ���ƿ����� �̵�
        Vector3 direction = (transform.position - grabPack.transform.position).normalized;
        grabPack.GetComponent<Rigidbody>().velocity = direction * returnSpeed;

        // �׷����� �÷��̾� ��ó�� �����ϸ� ����
        if (Vector3.Distance(grabPack.transform.position, transform.position) < 1f)
        {
            Destroy(grabPack);
            grabPack = null;
        }
    }


}
