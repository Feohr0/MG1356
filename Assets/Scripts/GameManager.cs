using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public float time;
}

[System.Serializable]
public class BestTimesData
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

public class GameManager : MonoBehaviour
{
    [Header("Player ve UI")]
    public GameObject playerPrefab;
    public GameObject pauseMenu;
    public TextMeshProUGUI timerText;
    public GameObject failPanel;
    public GameObject winPanel;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI bestTimesText;

    [Header("Maze Factories")]
    public MazeFactory forestFactory;
    public MazeFactory desertFactory;
    public MazeFactory winterFactory;

    [Header("Enemy Ayarları")]
    public ColoredEnemy[] enemyPrefabs;

    [Header("Bıçak Ayarları")]
    public GameObject knifePrefab; // Bıçak prefab'ı
    public int totalKnifesToSpawn = 15; // Spawn edilecek toplam bıçak sayısı

    private EnemyFactory enemyFactory;
    private GameObject currentMaze;
    private GameObject[] enemySpawnPoints;
    private GameObject[] knifeSpawnPoints;

    private const string BestTimesKey = "TopScores";

    public static GameManager Instance;
    public static string currentPlayerName = "Oyuncu";

    public float timer = 0;
    public bool isCounting = false;
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

        enemyFactory = new RandomEnemyFactory(enemyPrefabs);
        enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");
        SpawnRandomEnemy();

        // Bıçakları spawn et
        SpawnKnives();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

  

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            isCounting = false;
            isGameOver = false;
            isWon = false;
            timer = 0;
            Time.timeScale = 1f;
            return;
        }

        CreateMazeForScene(scene.name);
        StartCoroutine(SpawnPlayerNextFrame());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FindUIReferences();
        isCounting = true;
        isGameOver = false;
        isWon = false;
        Time.timeScale = 1f;

        enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");
        enemyFactory = new RandomEnemyFactory(enemyPrefabs);
        SpawnRandomEnemy();

        // Yeni sahnede bıçakları spawn et
        SpawnKnives();
    }

    private IEnumerator SpawnPlayerNextFrame()
    {
        yield return null;
        SpawnPlayer();
    }

    // YENİ METOT: Bıçakları spawn eder
    private void SpawnKnives()
    {
        if (knifePrefab == null)
        {
            Debug.LogError("Knife Prefab atanmamış!");
            return;
        }

        // KnifeSpawnPoint tag'li objeleri bul
        knifeSpawnPoints = GameObject.FindGameObjectsWithTag("KnifeSpawnPoint");

        if (knifeSpawnPoints == null || knifeSpawnPoints.Length == 0)
        {
            Debug.LogError("Sahnede 'KnifeSpawnPoint' tag'li obje bulunamadı!");
            return;
        }

        // Spawn noktalarını karıştır
        List<GameObject> shuffledSpawnPoints = new List<GameObject>(knifeSpawnPoints);
        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            GameObject temp = shuffledSpawnPoints[i];
            int randomIndex = Random.Range(i, shuffledSpawnPoints.Count);
            shuffledSpawnPoints[i] = shuffledSpawnPoints[randomIndex];
            shuffledSpawnPoints[randomIndex] = temp;
        }

        // Belirtilen sayıda bıçak spawn et
        int knivesToSpawn = Mathf.Min(totalKnifesToSpawn, shuffledSpawnPoints.Count);

        for (int i = 0; i < knivesToSpawn; i++)
        {
            Transform spawnPoint = shuffledSpawnPoints[i].transform;
            GameObject knife = Instantiate(knifePrefab, spawnPoint.position, Quaternion.identity);
            knife.name = $"Knife_{i + 1}";
        }

        Debug.Log($"{knivesToSpawn} adet bıçak spawn edildi!");
    }

    
        public  void Retry()
        {
            if (GUILayout.Button("Retry"))
            {
                Time.timeScale = 1f;
                GameManager.Instance.timer = 0;
                
            }
        }

    private void FindUIReferences()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Sahnede Canvas nesnesi bulunamadı. UI atamaları yapılamadı.");
            return;
        }

        GameObject tmpObj = GameObject.Find("timerText");
        if (tmpObj != null)
        {
            timerText = tmpObj.GetComponent<TextMeshProUGUI>();
        }

        Transform pauseTransform = canvas.transform.Find("PauseMenu");
        if (pauseTransform != null)
        {
            pauseMenu = pauseTransform.gameObject;
            pauseMenu.SetActive(false);
        }

        Transform failTransform = canvas.transform.Find("failPanel");
        if (failTransform != null)
        {
            failPanel = failTransform.gameObject;
            failPanel.SetActive(false);

            Transform failFinalTimeTransform = failTransform.Find("finalTimeText");
            if (failFinalTimeTransform != null)
            {
                finalTimeText = failFinalTimeTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        Transform winTransform = canvas.transform.Find("winPanel");
        if (winTransform != null)
        {
            winPanel = winTransform.gameObject;
            winPanel.SetActive(false);

            Transform winFinalTimeTransform = winTransform.Find("finalTimeText");
            if (winFinalTimeTransform != null)
            {
                finalTimeText = winFinalTimeTransform.GetComponent<TextMeshProUGUI>();
            }

            Transform bestTimesTransform = winTransform.Find("bestTimesText");
            if (bestTimesTransform != null)
            {
                bestTimesText = bestTimesTransform.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("WinPanel altında 'bestTimesText' adlı bileşen bulunamadı.");
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
            GameObject existingPlayer = GameObject.FindWithTag("Player");
            if (existingPlayer != null)
            {
                Destroy(existingPlayer);
            }

            GameObject newPlayer = Instantiate(playerPrefab, spawnObj.transform.position, spawnObj.transform.rotation);
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
        if (isGameOver || isWon) return; // Oyun bittiyse pause açılmasın

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

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        isCounting = false;

        // Time.timeScale = 0f; ← BUNU KALDIR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Player ve düşmanları durdur
        StopGameplay();

        int min = Mathf.FloorToInt(timer / 60);
        int secs = Mathf.FloorToInt(timer % 60);

        if (finalTimeText != null)
        {
            finalTimeText.text = string.Format("Geçirdiğin Süre: {0:00}:{1:00}", min, secs);
        }

        if (failPanel != null)
        {
            failPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("FailPanel referansı null.");
        }
    }

    public void GameWon()
    {
        if (isWon) return;
        isWon = true;
        isCounting = false;

        // Time.timeScale = 0f; ← BUNU KALDIR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Player ve düşmanları durdur
        StopGameplay();

        SaveBestTime(timer, currentPlayerName);
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

    // YENİ METOT: Oyunu durdur
    private void StopGameplay()
    {
        // Player'ı durdur
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Player'daki hareket scriptlerini devre dışı bırak
            MonoBehaviour[] playerScripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in playerScripts)
            {
                if (script.GetType().Name != "Transform") // Transform hariç tüm scriptleri kapat
                {
                    script.enabled = false;
                }
            }

            // Rigidbody varsa durdur
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // Tüm düşmanları durdur
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.enabled = false;

            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity = Vector3.zero;
                enemyRb.angularVelocity = Vector3.zero;
                enemyRb.isKinematic = true;
            }
        }
    }
    private void SaveBestTime(float newTime, string playerName)
    {
        BestTimesData data = LoadBestTimes();
        ScoreEntry newEntry = new ScoreEntry { playerName = playerName, time = newTime };
        data.scores.Add(newEntry);
        data.scores.Sort((entry1, entry2) => entry1.time.CompareTo(entry2.time));

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(BestTimesKey, json);
        PlayerPrefs.Save();

        Debug.Log($"Yeni skor kaydedildi: {playerName} - {newTime}. Toplam kayıt: {data.scores.Count}");
    }

    private BestTimesData LoadBestTimes()
    {
        if (PlayerPrefs.HasKey(BestTimesKey))
        {
            string json = PlayerPrefs.GetString(BestTimesKey);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonUtility.FromJson<BestTimesData>(json);
            }
        }
        return new BestTimesData();
    }

    private void DisplayBestTimes()
    {
        if (bestTimesText == null) return;

        BestTimesData data = LoadBestTimes();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("EN İYİ 3 SÜRE:");

        int displayCount = Mathf.Min(data.scores.Count, 3);

        for (int i = 0; i < 3; i++)
        {
            if (i < displayCount)
            {
                ScoreEntry entry = data.scores[i];
                float time = entry.time;
                int min = Mathf.FloorToInt(time / 60);
                int secs = Mathf.FloorToInt(time % 60);
                int ms = Mathf.FloorToInt((time * 100) % 100);

                sb.AppendLine($"{i + 1}. {entry.playerName} - {min:00}:{secs:00}:{ms:00}");
            }
            else
            {
                sb.AppendLine($"{i + 1}. --:--:--");
            }
        }

        bestTimesText.text = sb.ToString();
    }

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
            Debug.LogError("FinalTimeText referansı null.");
        }
    }

    private void SpawnRandomEnemy()
    {
        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogError("Sahnede 'EnemySpawn' tag'li obje bulunamadı!");
            return;
        }

        int randomSpawnIndex = Random.Range(0, enemySpawnPoints.Length);
        Transform chosenSpawn = enemySpawnPoints[randomSpawnIndex].transform;

        Enemy createdEnemy = enemyFactory.CreateEnemy();
        if (createdEnemy == null)
        {
            Debug.LogError("EnemyFactory CreateEnemy null döndü!");
            return;
        }

        createdEnemy.transform.position = chosenSpawn.position;
        createdEnemy.transform.rotation = chosenSpawn.rotation;

        Debug.Log($"Tek düşman spawn edildi: {createdEnemy.name} @ {chosenSpawn.name}");
    }
}