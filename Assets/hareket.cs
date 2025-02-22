using UnityEngine;

public class hareket : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;

    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float gravity = 9.81f;
    public float jumpHeight = 1.5f;
    public float mouseSensitivity = 2f;

    private Vector3 velocity;
    private bool isGrounded;
    
    private float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Mouse'u ekranın ortasında kilitle
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Yere basıyor mu kontrol et
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // Yatay hareket girişlerini al
        float moveX = Input.GetAxisRaw("Horizontal"); // Anında tepki vermesi için "Raw" kullanıyoruz
        float moveZ = Input.GetAxisRaw("Vertical");

        // Koşma ve yürüme ayarı
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Yön belirleme
        Vector3 move = transform.forward * moveZ + transform.right * moveX;
        move = move.normalized * currentSpeed;

        // Hareketi uygula
        controller.Move(move * Time.deltaTime);

        // Zıplama
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }

        // Yer çekimi uygula
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Fare ile kamera kontrolü
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Kameranın aşırı yukarı/aşağı bakmasını engelle

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
