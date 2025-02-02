using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float mouseSensitivity = 2f; // 마우스 감도

    void Update()
    {
        // 마우스 이동에 따른 Y축 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX); // Y축 회전

        // W, A, S, D 키를 통한 이동
        float moveHorizontal = Input.GetAxis("Horizontal"); // A, D 키
        float moveVertical = Input.GetAxis("Vertical"); // W, S 키

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        movement = transform.TransformDirection(movement); // 로컬 공간으로 변환
        transform.position += movement * moveSpeed * Time.deltaTime; // 이동
    }

}
