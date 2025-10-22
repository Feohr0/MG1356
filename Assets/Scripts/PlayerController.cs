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
    public float mouseSensitivity = 100f;

    // DEĞİŞİKLİK 1: Header ve Vector3 değeri birinci şahıs (FPS) için güncellendi.
    [Header("1. Şahıs Kamera Konumu")]
    private CharacterController controller;
    public float currentSpeed;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        Time.timeScale = 1f;
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

        // Kamera yukarı-aşağı dönüşü
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Karakter sağa-sola dönüşü
        transform.Rotate(Vector3.up * mouseX);

        // DEĞİŞİKLİK 2: Bu satır kamerayı sürekli arkaya itiyordu. 
        // Artık birinci şahıs kamerasında olduğumuz için bu satırı siliyoruz.
        // cameraTransform.localPosition = cameraOffset; 

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