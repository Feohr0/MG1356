using UnityEngine;
using TMPro;

public class PlayerCombat : MonoBehaviour
{
    [Header("Býçak Ayarlarý")]
    public int knifeCount = 0;
    public GameObject knifeVisual; // Oyuncunun elindeki býçak görseli
    public Transform knifeHolder;  // Býçaðýn tutulacaðý pozisyon (örn: sað el)

    [Header("Saldýrý Ayarlarý")]
    // DEÐÝÞTÝ: Deðiþkenin adýný daha anlaþýlýr olmasý için 'attackRadius' yaptýk.
    // Bu, saldýrýnýn etki edeceði küresel alanýn yarýçapýný belirler.
    public float attackRadius = 2f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;

    private TextMeshProUGUI knifeCountText;
    private float lastAttackTime = 0f;
    private bool canAttack = false;

    void Awake()
    {

        GameObject knifeTextObject = GameObject.FindGameObjectWithTag("KnifeCountTextUI");
        if (knifeTextObject != null)
        {
            knifeCountText = knifeTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Sahnedeki 'KnifeCountTextUI' etiketine sahip UI objesi bulunamadý! Lütfen kontrol edin.");
        }
    }

    void Start()
    {
        if (knifeVisual != null)
            knifeVisual.SetActive(false);
        UpdateKnifeUI();
    }

    void Update()
{
    // Fare sol týk ile saldýrý (sadece býçak varsa)
    if (Input.GetMouseButtonDown(0) && canAttack)
    {
            Debug.Log("Atak ýnput Yapýldý");
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
                Debug.Log("Attak Ýþleme girmeli");
            lastAttackTime = Time.time;
        }
    }
}

    public void CollectKnife()
    {
        knifeCount++;
        if (knifeCount == 1 && knifeVisual != null)
        {
            knifeVisual.SetActive(true);
            canAttack = true;
        }
        UpdateKnifeUI();
        Debug.Log($"Býçak toplandý! Toplam: {knifeCount}");
    }

    // DEÐÝÞTÝ: Saldýrý metodu tamamen yeniden yazýldý.
    void Attack()
    {
        // Býçak yoksa veya saldýrý yapýlamýyorsa fonksiyondan çýk.
        if (knifeCount <= 0 || !canAttack) return;
        Debug.Log("Atak Yapýldý");
        // YENÝ: Saldýrý tuþuna basýldýðý an býçak sayýsýný azalt.
        knifeCount--;
        Debug.Log("Saldýrý yapýldý! Kalan býçak: " + knifeCount);

        // YENÝ: Saldýrý alanýnýn merkezini belirle (oyuncunun biraz önü).
        Vector3 attackCenter = transform.position + transform.forward * (attackRadius * 0.5f);

        // YENÝ: Belirtilen alandaki tüm düþmanlarý bulmak için OverlapSphere kullan.
        Collider[] hitEnemies = Physics.OverlapSphere(attackCenter, attackRadius, enemyLayer);

        // Bulunan her düþmana hasar ver.
        foreach (Collider enemyCollider in hitEnemies)
        {
            Debug.Log($"Alanýn içinde düþman bulundu: {enemyCollider.name}");
            EnemyHealth health = enemyCollider.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(1);
                Debug.Log($"{enemyCollider.name} hasar aldý!");
            }
        }

        // YENÝ: Býçak bittiyse saldýrý yeteneðini kapat ve görseli gizle.
        if (knifeCount <= 0)
        {
            canAttack = false;
            Debug.Log("Atak yapamaz");
            if (knifeVisual != null)
            {
                knifeVisual.SetActive(false);
            }
        }
        else
        {
            CollectKnife();
        }
            // Arayüzü güncelle.
            UpdateKnifeUI();
    }

    void UpdateKnifeUI()
    {
        if (knifeCountText != null)
        {
            knifeCountText.text = $"Knives: {knifeCount}";
        }
    }

    // DEÐÝÞTÝ: Debug görseli yeni saldýrý alanýný gösterecek þekilde güncellendi.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Oyuncunun önündeki saldýrý alanýný bir küre olarak çiz.
        Vector3 attackCenter = transform.position + transform.forward * (attackRadius * 0.5f);
        Gizmos.DrawWireSphere(attackCenter, attackRadius);
    }
}