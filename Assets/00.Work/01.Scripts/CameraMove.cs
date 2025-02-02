using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    public float mouseSensitivity = 2f; // ���콺 ����

    void Update()
    {
        // ���콺 �̵��� ���� Y�� ȸ��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX); // Y�� ȸ��

        // W, A, S, D Ű�� ���� �̵�
        float moveHorizontal = Input.GetAxis("Horizontal"); // A, D Ű
        float moveVertical = Input.GetAxis("Vertical"); // W, S Ű

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        movement = transform.TransformDirection(movement); // ���� �������� ��ȯ
        transform.position += movement * moveSpeed * Time.deltaTime; // �̵�
    }

}
