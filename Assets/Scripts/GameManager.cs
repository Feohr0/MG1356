using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // List kullanmak için eklendi
using System.Text; // StringBuilder kullanmak için eklendi

// Serileştirme için yardımcı sınıf (PlayerPrefs'e liste kaydetmek için JSON kullanılır)
[System.Serializable]
public class BestTimesData
{
    public List<float> times = new List<float>();
}

public class GameManager : MonoBehaviour
{
    [Header("Player ve UI")]
    public GameObject playerPrefab;
    public GameObject pauseMenu;
    public TextMeshProUGUI timerText;
    public GameObject failPanel;
    public GameObject winPanel;
    // Fail Panelindeki süre gösterimi için Text bileşeni
    public TextMeshProUGUI finalTimeText;

    // YENİ: En iyi 3 süreyi göstermek için Text bileşeni
    public TextMeshProUGUI bestTimesText;

    [Header("Maze Factories")]
    public MazeFactory forestFactory;
    public MazeFactory desertFactory;
    public MazeFactory winterFactory;

    [Header("Enemy Ayarları")]
    public ColoredEnemy[] enemyPrefabs; // 3 farklı renkli düşman prefabı

    private EnemyFactory enemyFactory;
    private GameObject currentMaze;
    private GameObject[] enemySpawnPoints; // tag'li objeler buraya otomatik eklenecek

    // YENİ: En iyi süreleri kaydetmek için anahtar
    private const string BestTimesKey = "Top3Times";

    public static GameManager Instance;
    public float timer = 0;
    public bool isCounting = false;

    // Oyunun bitip bitmediğini kontrol etmek için
    private bool isGameOver = false;
    public bool isWon = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        isCounting = true;
        SpawnPlayer();
        FindUIReferences();

        // Factory yalnızca bir kere oluşturulur
        enemyFactory = new RandomEnemyFactory(enemyPrefabs);

        // Sahnedeki "EnemySpawn" tag'li objeleri topla (Start'ta)
        enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");

        // Tek bir düşman spawn et
        SpawnRandomEnemy();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Retry()
    {
        Scene activescene = SceneManager.GetActiveScene();

        // Sahneyi ismini kullanarak yeniden yükle
        SceneManager.LoadScene(activescene.name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            isCounting = false;
            isGameOver = false; // Oyun durumu sıfırlanır
            isWon = false;      // Kazanma durumu sıfırlanır
            timer = 0;
            Time.timeScale = 1f;
            return;
        }

        CreateMazeForScene(scene.name);
        StartCoroutine(SpawnPlayerNextFrame());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FindUIReferences(); // UI referanslarını bul ve ata
        isCounting = true;
        isGameOver = false; // Oyun durumu sıfırlanır
        isWon = false;      // Kazanma durumu sıfırlanır
        Time.timeScale = 1f;

        // Yeni sahne yüklendiğinde spawn noktalarını güncelle
        enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");

        // Factory zaten varsa yeniden yaratmana gerek yok ama güvenlik için yeniden ata:
        enemyFactory = new RandomEnemyFactory(enemyPrefabs);

        // Tek bir düşman spawn et (burada da yalnızca 1 çağrı yapıyoruz)
        SpawnRandomEnemy();
    }

    private IEnumerator SpawnPlayerNextFrame()
    {
        yield return null;
        SpawnPlayer();
    }

    private void FindUIReferences()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Sahnede Canvas nesnesi bulunamadı. UI atamaları yapılamadı.");
            return;
        }

        // --- TIMER TEXT ---
        GameObject tmpObj = GameObject.Find("timerText");
        if (tmpObj != null)
        {
            timerText = tmpObj.GetComponent<TextMeshProUGUI>();
        }

        // --- PAUSE MENU ---
        Transform pauseTransform = canvas.transform.Find("PauseMenu");
        if (pauseTransform != null)
        {
            pauseMenu = pauseTransform.gameObject;
            pauseMenu.SetActive(false);
        }

        // --- FAIL PANEL ---
        Transform failTransform = canvas.transform.Find("failPanel");
        if (failTransform != null)
        {
            failPanel = failTransform.gameObject;
            failPanel.SetActive(false);

            // Fail Panelinin altındaki final time text'i bul (Eğer oradaysa)
            Transform failFinalTimeTransform = failTransform.Find("finalTimeText");
            if (failFinalTimeTransform != null)
            {
                finalTimeText = failFinalTimeTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        // --- WIN PANEL (Kazandın Paneli) ---
        Transform winTransform = canvas.transform.Find("winPanel");
        if (winTransform != null)
        {
            winPanel = winTransform.gameObject;
            winPanel.SetActive(false);

            // Win Panelinin altındaki final time text'i bul ve ata.
            Transform winFinalTimeTransform = winTransform.Find("finalTimeText");
            if (winFinalTimeTransform != null)
            {
                // finalTimeText referansını Win Panelinin altındaki nesneye atar
                finalTimeText = winFinalTimeTransform.GetComponent<TextMeshProUGUI>();
            }

            // YENİ: Win Panelinin altındaki Best Times Text bileşenini bul ve ata.
            Transform bestTimesTransform = winTransform.Find("bestTimesText");
            if (bestTimesTransform != null)
            {
                bestTimesText = bestTimesTransform.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("WinPanel altında 'bestTimesText' adlı bileşen bulunamadı. Lütfen Win Panelinde bir TextMeshProUGUI ekleyin.");
            }
        }
    }

    private void CreateMazeForScene(string sceneName)
    {
        if (currentMaze != null)
            Destroy(currentMaze);

        switch (sceneName)
        {
            case "FirstMaze":
                currentMaze = winterFactory.CreateMaze(Vector3.zero);
                break;
            case "SecondMaze":
                currentMaze = desertFactory.CreateMaze(Vector3.zero);
                break;
            case "ThirdMaze":
                currentMaze = forestFactory.CreateMaze(Vector3.zero);
                break;
            default:
                Debug.LogWarning("Bu sahneye uygun factory bulunamadı: " + sceneName);
                return;
        }

        currentMaze.name = sceneName + "_Maze";
        Debug.Log($"Factory + Builder ile Maze oluşturuldu: {currentMaze.name}");
    }

    private void SpawnPlayer()
    {
        GameObject spawnObj = GameObject.Find("SpawnPoint");
        if (spawnObj != null)
        {
            // Eğer oyuncu zaten sahnede varsa (DontDestroyOnLoad'dan gelen), onu yok et ve yenisini oluştur.
            GameObject existingPlayer = GameObject.FindWithTag("Player");
            if (existingPlayer != null)
            {
                Destroy(existingPlayer);
            }

            // Yeni oyuncuyu oluştur
            GameObject newPlayer = Instantiate(playerPrefab, spawnObj.transform.position, spawnObj.transform.rotation);
            // Player prefab'ının tag'inin "Player" olduğundan emin olun!
            if (!newPlayer.CompareTag("Player"))
            {
                newPlayer.tag = "Player";
            }
        }
        else
        {
            Debug.LogWarning("Spawnpoint objesi bulunamadı!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        Timer();
    }

    private void TogglePause()
    {
        if (isGameOver) return; // Oyun bittiyse duraklatma menüsü açılmaz.

        if (pauseMenu == null)
        {
            Debug.LogError("PauseMenu referansı null!");
            return;
        }

        isCounting = !isCounting;
        pauseMenu.SetActive(!isCounting);
        Time.timeScale = isCounting ? 1f : 0f;

        Cursor.lockState = isCounting ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCounting;
    }

    public void Timer()
    {
        if (isCounting)
        {
            timer += Time.deltaTime;
        }

        int min = Mathf.FloorToInt(timer / 60);
        int secs = Mathf.FloorToInt(timer % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", min, secs);
        }
    }

    public void ResumeGame()
    {
        if (pauseMenu == null) return;
        isCounting = true;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        if (pauseMenu == null) return;
        isCounting = false;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Oyunu Durduran ve Son Süreyi Ekran Yazdıran Metot (Kaybetme)
    public void GameOver()
    {
        if (isGameOver) return; // Zaten bittiyse tekrar çalıştırma

        isGameOver = true;
        isCounting = false;

        // 1. Timer dursun ve oyun dondurulsun
        Time.timeScale = 0f;

        // 2. Cursor görünür olsun
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Süreyi hesapla
        int min = Mathf.FloorToInt(timer / 60);
        int secs = Mathf.FloorToInt(timer % 60);

        // 3. Süreyi Fail Panelindeki Text bileşenine yazdır.
        if (finalTimeText != null)
        {
            finalTimeText.text = string.Format("Geçirdiğin Süre: {0:00}:{1:00}", min, secs);
        }

        // 4. failPanel aktif olsun
        if (failPanel != null)
        {
            failPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("FailPanel referansı null. Süre yazılamadı.");
        }
    }

    // 🥇 Güncellenen GameWon metodu (En İyi Süre Kaydı Eklendi)
    public void GameWon()
    {
        if (isWon) return;
        isWon = true;
        isCounting = false;
        

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // YENİ: Süreyi kaydet ve listeyi güncelle
        SaveBestTime(timer);

        // Süre ve En İyi Süreleri Ekrana Yazdırma
        DisplayFinalTime();
        DisplayBestTimes();

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("winPanel bulunamadı!");
        }
    }

    // YENİ METOT: Oyuncunun o anki süresini Win Panelindeki Text'e yazdırır.
    private void DisplayFinalTime()
    {
        int min = Mathf.FloorToInt(timer / 60);
        int secs = Mathf.FloorToInt(timer % 60);
        int ms = Mathf.FloorToInt((timer * 100) % 100);

        if (finalTimeText != null)
        {
            finalTimeText.text = string.Format("Geçirdiğin Süre: {0:00}:{1:00}:{2:00}", min, secs, ms);
        }
        else
        {
            Debug.LogError("FinalTimeText referansı null. Süre yazılamadı.");
        }
    }

    // YENİ METOT: En iyi süreyi listeye ekler, sıralar ve kaydeder.
    private void SaveBestTime(float newTime)
    {
        // 1. Kayıtlı veriyi yükle
        BestTimesData data = LoadBestTimes();

        // 2. Yeni süreyi listeye ekle
        data.times.Add(newTime);

        // 3. Listeyi küçükten büyüğe sırala (en kısa süre ilk sırada)
        data.times.Sort();

        // 4. Listeyi en fazla 3 elemanla sınırla (Top 3)
        if (data.times.Count > 3)
        {
            data.times.RemoveRange(3, data.times.Count - 3);
        }

        // 5. Güncel listeyi JSON olarak kaydet
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(BestTimesKey, json);
        PlayerPrefs.Save();

        Debug.Log($"Yeni en iyi süre kaydedildi. Top 3: {json}");
    }

    // YENİ METOT: Kayıtlı en iyi süreleri PlayerPrefs'ten yükler.
    private BestTimesData LoadBestTimes()
    {
        if (PlayerPrefs.HasKey(BestTimesKey))
        {
            string json = PlayerPrefs.GetString(BestTimesKey);
            return JsonUtility.FromJson<BestTimesData>(json);
        }

        // Kayıt yoksa boş bir liste döndür
        return new BestTimesData();
    }

    // YENİ METOT: En iyi süreleri UI'a 1, 2, 3 sıralı olarak yazdırır.
    private void DisplayBestTimes()
    {
        if (bestTimesText == null) return;

        BestTimesData data = LoadBestTimes();
        StringBuilder sb = new StringBuilder(); // System.Text.StringBuilder kullanıldı
        sb.AppendLine("EN İYİ 3 SÜRE:");

        // Listede kaç öğe olursa olsun, 1'den 3'e kadar gösterim yapılır.
        for (int i = 0; i < 3; i++)
        {
            if (i < data.times.Count)
            {
                float time = data.times[i];
                int min = Mathf.FloorToInt(time / 60);
                int secs = Mathf.FloorToInt(time % 60);
                int ms = Mathf.FloorToInt((time * 100) % 100);

                // 1., 2. ve 3. sıra numaraları ile gösterim
                sb.AppendLine($"{i + 1}. {min:00}:{secs:00}:{ms:00}");
            }
            else
            {
                // İlk 3'te yer kalmadıysa "-" ile boşluk gösterimi
                sb.AppendLine($"{i + 1}. --:--:--");
            }
        }

        bestTimesText.text = sb.ToString();
    }

    // 🧟‍♂️ Tek bir rastgele düşman spawn eden metod
    private void SpawnRandomEnemy()
    {
        //  ❗ Ön kontrol: spawn noktaları var mı?
        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogError("Sahnede 'EnemySpawn' tag'li obje bulunamadı! Tek düşman spawn edilemiyor.");
            return;
        }

        // 1) Rastgele spawn noktası seç (sadece 1 tane)
        int randomSpawnIndex = Random.Range(0, enemySpawnPoints.Length);
        Transform chosenSpawn = enemySpawnPoints[randomSpawnIndex].transform;

        // 2) Fabrikadan düşmanı oluştur (Clone() -> Instantiate yapıyor)
        Enemy createdEnemy = enemyFactory.CreateEnemy();
        if (createdEnemy == null)
        {
            Debug.LogError("EnemyFactory CreateEnemy null döndü!");
            return;
        }

        // ÖNEMLİ: createdEnemy zaten Instantiate edilmiş bir nesne olmalı.
        // Bu yüzden burada yeniden Instantiate yapmıyoruz; sadece pozisyon/rotation ayarları yapıyoruz.
        createdEnemy.transform.position = chosenSpawn.position;
        createdEnemy.transform.rotation = chosenSpawn.rotation;

        Debug.Log($"Tek düşman spawn edildi: {createdEnemy.name} @ {chosenSpawn.name}");
    }
}