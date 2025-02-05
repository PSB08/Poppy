using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    public float mouseSensitivity = 100f; // ���콺 ����
    public float jumpHeight = 2f; // ���� ����

    private float xRotation = 0f; // ī�޶� ���� ȸ�� ����
    private CharacterController controller; // ĳ���� ��Ʈ�ѷ�
    private Vector3 velocity; // ���� �ӵ�
    private float gravity = -9.81f; // �߷� ���ӵ�

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ���콺 �Է����� ī�޶� ȸ��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ī�޶� ���� ȸ�� (Y��)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ī�޶� ���� ȸ�� ���� ����

        // ī�޶� ���� ȸ�� (X��)
        float yRotation = transform.eulerAngles.y + mouseX; // ���� Y�� ȸ�� ���� ���콺 �Է��� ����

        // ī�޶� ȸ�� ����
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Ű���� �Է����� �̵�
        float moveX = Input.GetAxis("Horizontal"); // A, D Ű
        float moveZ = Input.GetAxis("Vertical");   // W, S Ű

        Vector3 move = transform.right * moveX + transform.forward * moveZ; // �̵� ���� ���
        controller.Move(move * moveSpeed * Time.deltaTime); // �̵� ����

        // �߷� �� ���� ó��
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ���鿡 ����� �� �ʱ�ȭ
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // ����
        }

        velocity.y += gravity * Time.deltaTime; // �߷� ����
        controller.Move(velocity * Time.deltaTime); // ���� �̵� ����
    }

}
