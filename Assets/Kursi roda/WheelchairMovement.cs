using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WheelchairMovement : MonoBehaviour
{
    public float moveSpeed = 4.0f; // Kecepatan maju/mundur
    public float rotationSpeed = 90.0f; // Kecepatan berputar (derajat per detik)

    // Untuk mouse look
    public Transform cameraTransform;
    public float mouseSensitivity = 100.0f;
    private float cameraPitch = 0.0f; // Rotasi vertikal kamera

    private CharacterController controller;

    void Start()
    {
        // Mengambil komponen CharacterController yang ada di objek ini
        controller = GetComponent<CharacterController>();

        // Mengunci cursor di tengah layar dan menyembunyikannya
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Pastikan cameraTransform sudah di-assign
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // --- Gerakan Maju/Mundur dan Berputar (Keyboard) ---
        float verticalInput = Input.GetAxis("Vertical"); // W/S atau Panah Atas/Bawah
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D atau Panah Kiri/Kanan

        // Hitung vektor gerakan berdasarkan input
        Vector3 moveDirection = transform.forward * verticalInput;

        // Aplikasikan gerakan menggunakan CharacterController
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Aplikasikan rotasi (berputar di tempat)
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);


        // --- Gerakan Kamera (Mouse Look) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotasi Kiri/Kanan (Yaw) -> diterapkan pada seluruh player
        transform.Rotate(Vector3.up * mouseX);

        // Rotasi Atas/Bawah (Pitch) -> diterapkan hanya pada kamera
        cameraPitch -= mouseY;
        // Batasi sudut pandang agar tidak bisa berputar terbalik
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        // Terapkan rotasi pitch ke kamera
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}