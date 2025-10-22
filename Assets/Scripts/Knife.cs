using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("Görsel Efektler")]
    public float rotationSpeed = 100f; // Býçaðýn dönme hýzý
    public float hoverHeight = 0.5f;   // Yukarý-aþaðý hareket mesafesi
    public float hoverSpeed = 2f;      // Yukarý-aþaðý hareket hýzý

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Býçaðý Y ekseninde döndür
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Yukarý-aþaðý hareket efekti
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // Player ile temas edince toplanýr
        if (other.CompareTag("Player"))
        {
            PlayerCombat combat = other.GetComponent<PlayerCombat>();
            if (combat != null)
            {
                combat.CollectKnife();
                Destroy(gameObject); // Býçaðý sahneden kaldýr
            }
        }
    }
}