using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float mouseSensitivity = 100f; // 마우스 감도
    public float jumpHeight = 2f; // 점프 높이

    private float xRotation = 0f; // 카메라 수직 회전 각도
    private CharacterController controller; // 캐릭터 컨트롤러
    private Vector3 velocity; // 수직 속도
    private float gravity = -9.81f; // 중력 가속도

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 마우스 입력으로 카메라 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 카메라 수직 회전 (Y축)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 카메라 수직 회전 각도 제한

        // 카메라 수평 회전 (X축)
        float yRotation = transform.eulerAngles.y + mouseX; // 현재 Y축 회전 값에 마우스 입력을 더함

        // 카메라 회전 적용
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // 키보드 입력으로 이동
        float moveX = Input.GetAxis("Horizontal"); // A, D 키
        float moveZ = Input.GetAxis("Vertical");   // W, S 키

        Vector3 move = transform.right * moveX + transform.forward * moveZ; // 이동 방향 계산
        controller.Move(move * moveSpeed * Time.deltaTime); // 이동 적용

        // 중력 및 점프 처리
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 지면에 닿았을 때 초기화
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // 점프
        }

        velocity.y += gravity * Time.deltaTime; // 중력 적용
        controller.Move(velocity * Time.deltaTime); // 수직 이동 적용
    }

}
