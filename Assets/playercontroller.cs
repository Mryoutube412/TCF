using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPlayerController : MonoBehaviour
{
    public float walkSpeed = 5f; // Normal Yürüme Hızı
    public float runSpeed = 10f; // Koşma Hızı (Shift)
    public float crouchSpeed = 2.5f; // Eğilme Hızı (CTRL)
    public float gravity = -9.81f; // Yerçekimi
    public float jumpHeight = 1.5f; // Zıplama Yüksekliği
    public float mouseSensitivity = 2f; // Fare Hassasiyeti
    public float slopeLimit = 45f; // Eğim Limiti

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isCrouching = false;

    public Transform cameraTransform; // Kamera Referansı
    private float originalHeight;
    public float crouchHeight = 0.5f; // Eğilme Yüksekliği

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // İmleci Kilitle
        originalHeight = controller.height; // Normal Boy Yüksekliği
    }

    void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleMouseLook();
        HandleCrouch();
        CheckFallImpact();
    }

    // Hareket ve Koşma
    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = walkSpeed;

        // Shift tuşu basılıysa koş
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }

        // Eğilme hızını uygula
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Zıplama
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // Yerçekimi Uygula
    void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Eğimde Kayma
        if (IsOnSlope() && !controller.isGrounded)
        {
            Vector3 slideDirection = Vector3.down;
            controller.Move(slideDirection * 5f * Time.deltaTime);
        }
    }

    // Fare Hareketi
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // Eğilme (CTRL)
    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = crouchHeight;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            controller.height = originalHeight;
        }
    }

    // Yüksekten Düşme ve Sarsılma
    void CheckFallImpact()
    {
        if (controller.isGrounded && velocity.y < -10f) // Yüksekten düşme algıla
        {
            StartCoroutine(SlowMotionEffect());
        }
    }

    // Yavaşlama ve Sarsılma Efekti
    IEnumerator SlowMotionEffect()
    {
        Time.timeScale = 0.2f; // Zamanı yavaşlat (10 FPS gibi görünür)
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f; // Normale döndür
    }

    // Eğim Kontrolü
    bool IsOnSlope()
    {
        if (controller.isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 + 0.3f))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                return angle > slopeLimit;
            }
        }
        return false;
    }
}
