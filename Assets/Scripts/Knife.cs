using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("G�rsel Efektler")]
    public float rotationSpeed = 100f; // B��a��n d�nme h�z�
    public float hoverHeight = 0.5f;   // Yukar�-a�a�� hareket mesafesi
    public float hoverSpeed = 2f;      // Yukar�-a�a�� hareket h�z�

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // B��a�� Y ekseninde d�nd�r
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Yukar�-a�a�� hareket efekti
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // Player ile temas edince toplan�r
        if (other.CompareTag("Player"))
        {
            PlayerCombat combat = other.GetComponent<PlayerCombat>();
            if (combat != null)
            {
                combat.CollectKnife();
                Destroy(gameObject); // B��a�� sahneden kald�r
            }
        }
    }
}