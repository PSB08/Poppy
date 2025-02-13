using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float headBobbingSpeed = 1f;
    public float headBobbingAmount = 0.05f;
    public float leanAmount = 0.1f;
    public AudioClip[] footstepSounds;
    public float footstepVolume = 0.5f;
    public float bobbingResetSpeed = 2f;

    public Transform weaponTransform;
    public float weaponBobbingSpeed = 1f;
    public float weaponBobbingAmount = 0.05f;

    private float verticalRotation = 0f;
    private Rigidbody rb;
    private Vector3 originalCameraPosition;
    private bool isMoving = false;
    private bool isResetting = false;
    private AudioSource audioSource;
    private float leanAngle = 0f;
    private Vector3 weaponInitialPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        originalCameraPosition = Camera.main.transform.localPosition;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;

        weaponInitialPosition = weaponTransform.localPosition;

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Rotation
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0f, horizontalRotation, 0f);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Movement
        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * movementSpeed;

        Vector3 moveDirection = (transform.right * horizontalMovement) + (transform.forward * verticalMovement);
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // Head bobbing and leaning
        if (rb.velocity.magnitude > 0.1f)
        {
            isMoving = true;
            isResetting = false;
            Camera.main.transform.localPosition = originalCameraPosition + GetHeadBobOffset();
            leanAngle = Mathf.Lerp(leanAngle, horizontalMovement * leanAmount, Time.deltaTime * 10f);
        }
        else
        {
            if (isMoving && !isResetting)
            {
                isResetting = true;
                StartCoroutine(ResetHeadBob());
            }
            isMoving = false;
            leanAngle = Mathf.Lerp(leanAngle, 0f, Time.deltaTime * 10f);
        }

        // Apply lean angle
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, -leanAngle * 10f);

        // Footstep sound
        if (isMoving && !audioSource.isPlaying)
        {
            PlayFootstepSound();
        }

        // Weapon bobbing
        if (isMoving)
        {
            weaponTransform.localPosition = weaponInitialPosition + GetWeaponBobOffset();
        }
        else
        {
            weaponTransform.localPosition = weaponInitialPosition;
        }
    }

    private Vector3 GetHeadBobOffset()
    {
        float headBobX = Mathf.Sin(Time.time * headBobbingSpeed) * headBobbingAmount;
        float headBobY = Mathf.Cos(Time.time * headBobbingSpeed * 2f) * headBobbingAmount * 0.5f;

        return new Vector3(headBobX, headBobY, 0f);
    }

    private IEnumerator ResetHeadBob()
    {
        float t = 0f;
        Vector3 startPosition = Camera.main.transform.localPosition;

        while (t < 1f)
        {
            t += Time.deltaTime * bobbingResetSpeed;
            Camera.main.transform.localPosition = Vector3.Lerp(startPosition, originalCameraPosition, t);
            yield return null;
        }

        isResetting = false;
    }

    private void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            AudioClip randomClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            audioSource.PlayOneShot(randomClip, footstepVolume);
        }
    }

    private Vector3 GetWeaponBobOffset()
    {
        float weaponBobX = Mathf.Sin(Time.time * weaponBobbingSpeed) * weaponBobbingAmount;
        float weaponBobY = Mathf.Cos(Time.time * weaponBobbingSpeed * 2f) * weaponBobbingAmount * 0.5f;

        return new Vector3(weaponBobX, weaponBobY, 0f);
    }
}