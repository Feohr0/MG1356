using UnityEngine;
using TMPro; // TextMeshPro kullanmak için bu satýrý ekle

public class PlayerKnifeController : MonoBehaviour
{
    // Deðiþkenler
    [Header("Býçak Ayarlarý")]
    private int currentKnives = 0; // Mevcut býçak sayýsý

    [Header("Saldýrý Ayarlarý")]
    public Transform attackPoint; // Saldýrýnýn merkez noktasý (Oyuncunun önünde boþ bir nesne)
    public float attackRange = 1.5f; // Saldýrý alaný yarýçapý
    public LayerMask enemyLayer; // Sadece düþmanlara hasar vermek için katman maskesi

    [Header("UI Elemanlarý")]
    // Bu deðiþkeni artýk Inspector'dan atamana gerek yok.
    private TextMeshProUGUI knifeCountText;

    void Start()
    {
        // --- YENÝ EKLENEN KOD BAÞLANGICI ---

        // "KnifeCountTextUI" etiketine sahip GameObject'i bul.
        GameObject knifeTextObject = GameObject.FindWithTag("KnifeCountTextUI");

        // Obje bulunduysa, onun TextMeshProUGUI bileþenini al.
        if (knifeTextObject != null)
        {
            knifeCountText = knifeTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            // Eðer etiketli obje bulunamazsa, hata mesajý ver.
            // Bu, etiketi atamayý unuttuðunda sorunu kolayca bulmaný saðlar.
            Debug.LogError("Sahnedeki 'KnifeCountTextUI' etiketli UI Text nesnesi bulunamadý! Lütfen kontrol et.");
        }

        // --- YENÝ EKLENEN KOD BÝTÝÞÝ ---

        UpdateKnifeUI(); // Oyun baþýnda UI'ý güncelle
    }

    void Update()
    {
        // Eðer Mouse Sol Tuþuna basýlýrsa ve en az 1 býçak varsa
        if (Input.GetMouseButtonDown(0) && currentKnives > 0)
        {
            Attack();
        }
    }

    // Býçak Toplama Mekaniði
    private void OnTriggerEnter(Collider other)
    {
        // Eðer temas edilen nesnenin etiketi "Bicak" ise
        if (other.CompareTag("Knife")) // Etiketi "Bicak" olarak güncelledim.
        {
            currentKnives++; // Býçak sayýsýný 1 artýr
            UpdateKnifeUI(); // UI'ý güncelle
            Destroy(other.gameObject); // Toplanan býçak nesnesini yok et
            Debug.Log("Býçak toplandý! Toplam býçak: " + currentKnives);
        }
    }

    // Saldýrý Mekaniði
    private void Attack()
    {
        // Her saldýrý bir býçak harcar
        currentKnives--;
        UpdateKnifeUI();
        Debug.Log("Saldýrý yapýldý! Kalan býçak: " + currentKnives);

        // Belirtilen noktada, belirtilen yarýçapta bir alan taramasý yap
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        // Eðer alanda bir düþman bulunduysa
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

    // UI Güncelleme Fonksiyonu
    private void UpdateKnifeUI()
    {
        if (knifeCountText != null)
        {
            knifeCountText.text = "Býçak: " + currentKnives;
        }
    }

    // Saldýrý alanýný editörde görebilmek için (opsiyonel)
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}