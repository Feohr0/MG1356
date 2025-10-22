using UnityEngine;
using TMPro; // TextMeshPro kullanmak i�in bu sat�r� ekle

public class PlayerKnifeController : MonoBehaviour
{
    // De�i�kenler
    [Header("B��ak Ayarlar�")]
    private int currentKnives = 0; // Mevcut b��ak say�s�

    [Header("Sald�r� Ayarlar�")]
    public Transform attackPoint; // Sald�r�n�n merkez noktas� (Oyuncunun �n�nde bo� bir nesne)
    public float attackRange = 1.5f; // Sald�r� alan� yar��ap�
    public LayerMask enemyLayer; // Sadece d��manlara hasar vermek i�in katman maskesi

    [Header("UI Elemanlar�")]
    // Bu de�i�keni art�k Inspector'dan atamana gerek yok.
    private TextMeshProUGUI knifeCountText;

    void Start()
    {
        // --- YEN� EKLENEN KOD BA�LANGICI ---

        // "KnifeCountTextUI" etiketine sahip GameObject'i bul.
        GameObject knifeTextObject = GameObject.FindWithTag("KnifeCountTextUI");

        // Obje bulunduysa, onun TextMeshProUGUI bile�enini al.
        if (knifeTextObject != null)
        {
            knifeCountText = knifeTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            // E�er etiketli obje bulunamazsa, hata mesaj� ver.
            // Bu, etiketi atamay� unuttu�unda sorunu kolayca bulman� sa�lar.
            Debug.LogError("Sahnedeki 'KnifeCountTextUI' etiketli UI Text nesnesi bulunamad�! L�tfen kontrol et.");
        }

        // --- YEN� EKLENEN KOD B�T��� ---

        UpdateKnifeUI(); // Oyun ba��nda UI'� g�ncelle
    }

    void Update()
    {
        // E�er Mouse Sol Tu�una bas�l�rsa ve en az 1 b��ak varsa
        if (Input.GetMouseButtonDown(0) && currentKnives > 0)
        {
            Attack();
        }
    }

    // B��ak Toplama Mekani�i
    private void OnTriggerEnter(Collider other)
    {
        // E�er temas edilen nesnenin etiketi "Bicak" ise
        if (other.CompareTag("Knife")) // Etiketi "Bicak" olarak g�ncelledim.
        {
            currentKnives++; // B��ak say�s�n� 1 art�r
            UpdateKnifeUI(); // UI'� g�ncelle
            Destroy(other.gameObject); // Toplanan b��ak nesnesini yok et
            Debug.Log("B��ak topland�! Toplam b��ak: " + currentKnives);
        }
    }

    // Sald�r� Mekani�i
    private void Attack()
    {
        // Her sald�r� bir b��ak harcar
        currentKnives--;
        UpdateKnifeUI();
        Debug.Log("Sald�r� yap�ld�! Kalan b��ak: " + currentKnives);

        // Belirtilen noktada, belirtilen yar��apta bir alan taramas� yap
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        // E�er alanda bir d��man bulunduysa
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log(enemy.name + " vuruldu!");
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1); // 1 hasar ver
            }
        }
    }

    // UI G�ncelleme Fonksiyonu
    private void UpdateKnifeUI()
    {
        if (knifeCountText != null)
        {
            knifeCountText.text = "B��ak: " + currentKnives;
        }
    }

    // Sald�r� alan�n� edit�rde g�rebilmek i�in (opsiyonel)
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}