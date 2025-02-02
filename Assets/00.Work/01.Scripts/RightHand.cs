using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    public float launchForce = 10f; // 발사 힘
    public float returnSpeed = 5f; // 돌아오는 속도
    private GameObject currentHand;

    void Update()
    {
        // 오른쪽 핸드 발사
        if (Input.GetMouseButtonDown(1) && currentHand == null) // 우클릭
        {
            currentHand = Instantiate(Resources.Load("Right"), transform.position, Quaternion.identity) as GameObject;
            Rigidbody rb = currentHand.GetComponent<Rigidbody>();
            rb.AddForce(transform.TransformDirection(Vector3.right) * launchForce, ForceMode.Impulse);
        }

        // 핸드가 돌아오는 기능
        ReturnHand();
    }

    private void ReturnHand()
    {
        if (currentHand != null)
        {
            Vector3 returnPosition = transform.position; // 플레이어 위치로 돌아옴
            currentHand.transform.position = Vector3.MoveTowards(currentHand.transform.position, returnPosition, returnSpeed * Time.deltaTime);

            // 핸드가 플레이어에게 도착하면 제거
            if (Vector3.Distance(currentHand.transform.position, returnPosition) < 0.1f)
            {
                Destroy(currentHand);
                currentHand = null;
            }
        }
    }

}
