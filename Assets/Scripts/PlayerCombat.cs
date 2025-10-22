using UnityEngine;
using TMPro;

public class PlayerCombat : MonoBehaviour
{
    [Header("B��ak Ayarlar�")]
    public int knifeCount = 0;
    public GameObject knifeVisual; // Oyuncunun elindeki b��ak g�rseli
    public Transform knifeHolder;  // B��a��n tutulaca�� pozisyon (�rn: sa� el)

    [Header("Sald�r� Ayarlar�")]
    // DE���T�: De�i�kenin ad�n� daha anla��l�r olmas� i�in 'attackRadius' yapt�k.
    // Bu, sald�r�n�n etki edece�i k�resel alan�n yar��ap�n� belirler.
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
            Debug.LogError("Sahnedeki 'KnifeCountTextUI' etiketine sahip UI objesi bulunamad�! L�tfen kontrol edin.");
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
    // Fare sol t�k ile sald�r� (sadece b��ak varsa)
    if (Input.GetMouseButtonDown(0) && canAttack)
    {
            Debug.Log("Atak �nput Yap�ld�");
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
                Debug.Log("Attak ��leme girmeli");
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
        Debug.Log($"B��ak topland�! Toplam: {knifeCount}");
    }

    // DE���T�: Sald�r� metodu tamamen yeniden yaz�ld�.
    void Attack()
    {
        // B��ak yoksa veya sald�r� yap�lam�yorsa fonksiyondan ��k.
        if (knifeCount <= 0 || !canAttack) return;
        Debug.Log("Atak Yap�ld�");
        // YEN�: Sald�r� tu�una bas�ld��� an b��ak say�s�n� azalt.
        knifeCount--;
        Debug.Log("Sald�r� yap�ld�! Kalan b��ak: " + knifeCount);

        // YEN�: Sald�r� alan�n�n merkezini belirle (oyuncunun biraz �n�).
        Vector3 attackCenter = transform.position + transform.forward * (attackRadius * 0.5f);

        // YEN�: Belirtilen alandaki t�m d��manlar� bulmak i�in OverlapSphere kullan.
        Collider[] hitEnemies = Physics.OverlapSphere(attackCenter, attackRadius, enemyLayer);

        // Bulunan her d��mana hasar ver.
        foreach (Collider enemyCollider in hitEnemies)
        {
            Debug.Log($"Alan�n i�inde d��man bulundu: {enemyCollider.name}");
            EnemyHealth health = enemyCollider.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(1);
                Debug.Log($"{enemyCollider.name} hasar ald�!");
            }
        }

        // YEN�: B��ak bittiyse sald�r� yetene�ini kapat ve g�rseli gizle.
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
            // Aray�z� g�ncelle.
            UpdateKnifeUI();
    }

    void UpdateKnifeUI()
    {
        if (knifeCountText != null)
        {
            knifeCountText.text = $"Knives: {knifeCount}";
        }
    }

    // DE���T�: Debug g�rseli yeni sald�r� alan�n� g�sterecek �ekilde g�ncellendi.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Oyuncunun �n�ndeki sald�r� alan�n� bir k�re olarak �iz.
        Vector3 attackCenter = transform.position + transform.forward * (attackRadius * 0.5f);
        Gizmos.DrawWireSphere(attackCenter, attackRadius);
    }
}