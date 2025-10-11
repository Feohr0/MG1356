using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hız Ayarları")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Kamera Ayarları")]
    public Transform cameraTransform;
    public float mouseSensitivity = 100;
    

    private CharacterController controller;
    private float currentSpeed;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        
        // Fare imlecini gizle ve ortala
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        PlayerPrefs.SetFloat("currentSensitivity", mouseSensitivity);
        // 🔁 Fare hareketi ile kamera dönüşü
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Yukarı-aşağı sınır

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 🏃‍♂️ Koşma / yürüme
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;

        // 🎮 Hareket girişleri (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }
}
