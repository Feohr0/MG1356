using UnityEngine;



public class DusmanAI : MonoBehaviour

{

    [Header("Hareket Ayarları")]

    public float gezinmeHizi = 2f;

    public float takipHizi = 4f;

    public float donusHizi = 3f;



    [Header("Algılama Ayarları")]

    public float gorusAlani = 10f;

    public float gorusAcisi = 120f;



    [Header("Duvar Tespit")]

    public float duvarTespitMesafesi = 1.5f;

    public float ongoruMesafesi = 3f;

    public LayerMask duvarLayer;

    public bool duvarTakipModu = true;

    public float duvarTakipHizi = 2.5f;



    [Header("Hareket Sınırları")]

    public float sinirGuvenligi = 3f;

    public bool otomatikSinirHesapla = true;



    private Vector3 minSinir;

    private Vector3 maxSinir;

    private Transform player;

    private Vector3 randomHedef;

    private float yonDegistirmeSuresi;

    private bool playerGorunuyor = false;

    private bool duvarTakipEdiliyor = false;

    private Vector3 duvarNormali;

    private float duvarTakipSuresi = 0f;

    private Vector3 sonPozisyon;

    private float sikismaSuresi = 0f;

    private float sonYonDegisimi = 0f;



    void Start()

    {

        // Player'ı bul
        gezinmeHizi = 2f;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)

        {

            player = playerObj.transform;

        }



        // Sınırları otomatik hesapla

        if (otomatikSinirHesapla)

        {

            HaritaSinirlariniHesapla();

        }



        // İlk random hedef belirle

        YeniRandomHedefBelirle();



        // Başlangıç pozisyonu kaydet

        sonPozisyon = transform.position;

    }



    void HaritaSinirlariniHesapla()

    {

        // Wall tag'li tüm objeleri bul

        GameObject[] duvarlar = GameObject.FindGameObjectsWithTag("Wall");



        if (duvarlar.Length == 0)

        {

            Debug.LogWarning("Hiç 'Wall' tag'li obje bulunamadı! Varsayılan sınırlar kullanılıyor.");

            minSinir = new Vector3(-50, 0, -50);

            maxSinir = new Vector3(50, 0, 50);

            return;

        }



        // Başlangıç değerleri

        float minX = float.MaxValue;

        float maxX = float.MinValue;

        float minZ = float.MaxValue;

        float maxZ = float.MinValue;



        // Tüm duvarların bounds'larını kontrol et

        foreach (GameObject duvar in duvarlar)

        {

            Renderer renderer = duvar.GetComponent<Renderer>();

            Collider collider = duvar.GetComponent<Collider>();



            Bounds bounds;



            if (renderer != null)

            {

                bounds = renderer.bounds;

            }

            else if (collider != null)

            {

                bounds = collider.bounds;

            }

            else

            {

                continue;

            }



            // Min ve max değerleri güncelle

            if (bounds.min.x < minX) minX = bounds.min.x;

            if (bounds.max.x > maxX) maxX = bounds.max.x;

            if (bounds.min.z < minZ) minZ = bounds.min.z;

            if (bounds.max.z > maxZ) maxZ = bounds.max.z;

        }



        // Sınırları ayarla

        minSinir = new Vector3(minX, 0, minZ);

        maxSinir = new Vector3(maxX, 0, maxZ);



        Debug.Log($"Harita sınırları hesaplandı: Min({minX}, {minZ}) - Max({maxX}, {maxZ})");

    }



    void Update()

    {

        // Player kontrolü

        if (player != null)

        {

            playerGorunuyor = PlayerGorunuyorMu();

        }



        if (playerGorunuyor)

        {

            // Player'ı takip et

            PlayerTakipEt();

        }

        else

        {

            // Random gezin

            RandomGezin();

        }



        // Duvar kontrolü

        DuvarKontrol();

    }



    bool PlayerGorunuyorMu()

    {

        Vector3 playerYonu = player.position - transform.position;

        float mesafe = playerYonu.magnitude;



        // Mesafe kontrolü

        if (mesafe > gorusAlani)

            return false;



        // Açı kontrolü

        float aci = Vector3.Angle(transform.forward, playerYonu);

        if (aci > gorusAcisi / 2f)

            return false;



        // Raycast ile engel kontrolü

        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, playerYonu.normalized, out hit, mesafe))

        {

            if (hit.collider.CompareTag("Player"))

            {

                return true;

            }

        }



        return false;

    }



    void PlayerTakipEt()

    {

        // Player'a doğru dön

        Vector3 yon = (player.position - transform.position).normalized;

        yon.y = 0;



        if (yon != Vector3.zero)

        {

            Quaternion hedefRotasyon = Quaternion.LookRotation(yon);

            transform.rotation = Quaternion.Slerp(transform.rotation, hedefRotasyon, donusHizi * Time.deltaTime);

        }



        // Player'a doğru ilerle

        transform.position += transform.forward * takipHizi * Time.deltaTime;

    }



    void RandomGezin()

    {

        // Hedefe yaklaştıysa veya süre dolduysa yeni hedef belirle

        float mesafe = Vector3.Distance(transform.position, randomHedef);

        yonDegistirmeSuresi -= Time.deltaTime;



        if (mesafe < 1f || yonDegistirmeSuresi <= 0)

        {

            YeniRandomHedefBelirle();

        }



        // Hedefe doğru dön

        Vector3 yon = (randomHedef - transform.position).normalized;

        yon.y = 0;



        if (yon != Vector3.zero)

        {

            Quaternion hedefRotasyon = Quaternion.LookRotation(yon);

            transform.rotation = Quaternion.Slerp(transform.rotation, hedefRotasyon, donusHizi * Time.deltaTime);

        }



        // İlerle

        transform.position += transform.forward * gezinmeHizi * Time.deltaTime;

    }



    void YeniRandomHedefBelirle()

    {

        int deneme = 0;

        bool gecerliHedefBulundu = false;



        while (!gecerliHedefBulundu && deneme < 10)

        {

            // Mevcut pozisyondan rastgele bir yöne hedef belirle

            Vector3 randomYon = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

            Vector3 potansiyelHedef = transform.position + randomYon * Random.Range(5f, 10f);



            // Hedef sınırlar içinde mi kontrol et

            if (potansiyelHedef.x > minSinir.x + sinirGuvenligi &&

                potansiyelHedef.x < maxSinir.x - sinirGuvenligi &&

                potansiyelHedef.z > minSinir.z + sinirGuvenligi &&

                potansiyelHedef.z < maxSinir.z - sinirGuvenligi)

            {

                randomHedef = potansiyelHedef;

                gecerliHedefBulundu = true;

            }



            deneme++;

        }



        // Geçerli hedef bulunamadıysa, haritanın ortasına doğru git

        if (!gecerliHedefBulundu)

        {

            Vector3 merkez = (minSinir + maxSinir) / 2f;

            randomHedef = merkez;

        }



        yonDegistirmeSuresi = Random.Range(3f, 6f);

    }



    void DuvarKontrol()

    {

        RaycastHit hit;

        Vector3 basla = transform.position + Vector3.up * 0.5f;



        // Önde duvar var mı kontrol et

        if (Physics.Raycast(basla, transform.forward, out hit, duvarTespitMesafesi, duvarLayer))

        {

            // Duvardan kaç - yeni random hedef belirle

            YeniRandomHedefBelirle();



            // Hemen ters yöne dön

            transform.Rotate(0, Random.Range(90f, 180f), 0);

        }



        // Sağ tarafta duvar kontrolü

        if (Physics.Raycast(basla, transform.right, out hit, duvarTespitMesafesi * 0.7f, duvarLayer))

        {

            transform.Rotate(0, -30f * Time.deltaTime * 60f, 0);

        }



        // Sol tarafta duvar kontrolü

        if (Physics.Raycast(basla, -transform.right, out hit, duvarTespitMesafesi * 0.7f, duvarLayer))

        {

            transform.Rotate(0, 30f * Time.deltaTime * 60f, 0);

        }

    }



    void OnTriggerEnter(Collider other)

    {

        // Player ile çarpışma kontrolü

        if (other.CompareTag("Player"))

        {

            // GameManager'daki GameOver metodunu çağır

            GameManager gameManager = FindObjectOfType<GameManager>();

            if (gameManager != null)

            {

                gameManager.GameOver();

            }

        }

    }



    // Debug çizimi (opsiyonel)

    void OnDrawGizmosSelected()

    {

        // Görüş alanı

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, gorusAlani);



        // Duvar tespit mesafesi

        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * duvarTespitMesafesi);

    }

}