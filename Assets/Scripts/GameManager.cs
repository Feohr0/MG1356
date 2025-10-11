using GLTFast;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject pauseMenu;
    public static GameManager Instance;
    public float timer =0;
    public bool isCounting = false;
    public TextMeshProUGUI timerText;

    [Header("Maze Prefabs")]
    public GameObject desertMazePrefab;
    public GameObject forestMazePrefab;
    public GameObject winterMazePrefab;

    private GameObject currentMaze;

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

    private void Start()
    {
        isCounting = true;
        SpawnPlayer();

        // İlk sahne yüklendiğinde UI referanslarını al
        FindUIReferences();
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
            Time.timeScale = 1f; // Menü sahnesinde zaman akışı normal olmalı
            Debug.Log("MainMenu sahnesi yüklendi, timer durduruldu.");
            return;
        }
        CreateMazeForScene(scene.name);
        SpawnPlayer();

        // Sahne değiştiğinde UI referanslarını yeniden bul
        FindUIReferences();

        // Timer'ı yeniden başlat
        isCounting = true;
        Time.timeScale = 1f;
    }

    // UI referanslarını bulma metodunu ayrı bir fonksiyona çıkardık
    private void FindUIReferences()
    {
        // Timer text'i bul
        GameObject tmpObj = GameObject.Find("timerText");
        if (tmpObj != null)
        {
            timerText = tmpObj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            timerText = FindAnyObjectByType<TextMeshProUGUI>();
            if (timerText == null)
            {
                Debug.LogWarning("timerText objesi sahnede bulunamadı!");
            }
        }

        // Pause menu'yü bul
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            Transform pauseTransform = canvas.transform.Find("PauseMenu");
            if (pauseTransform != null)
            {
                pauseMenu = pauseTransform.gameObject;
                pauseMenu.SetActive(false); // Başlangıçta kapalı olsun
                Debug.Log("PauseMenu bulundu!");
            }
            else
            {
                Debug.LogWarning("Canvas içinde 'PauseMenu' bulunamadı!");
            }
        }
        else
        {
            Debug.LogWarning("Canvas bulunamadı!");
        }
    }

    private void CreateMazeForScene(string sceneName)
    {
        if (currentMaze != null)
            Destroy(currentMaze);

        GameObject prefabToSpawn = null;

        switch (sceneName)
        {
            case "FirstMaze":
                prefabToSpawn = winterMazePrefab;
                break;
            case "SecondMaze":
                prefabToSpawn = desertMazePrefab;
                break;
            case "ThirdMaze":
                prefabToSpawn = forestMazePrefab;
                break;
            default:
                Debug.LogWarning("Bu sahneye uygun maze prefab bulunamadı: " + sceneName);
                return;
        }

        if (prefabToSpawn != null)
        {
            currentMaze = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
            currentMaze.name = sceneName + "_Maze";
            Debug.Log($"Maze oluşturuldu: {currentMaze.name}");
        }
    }

    void SpawnPlayer()
    {
        GameObject spawnObj = GameObject.Find("SpawnPoint");
        if (spawnObj != null)
        {
            Instantiate(playerPrefab, spawnObj.transform.position, spawnObj.transform.rotation);
        }
        else
        {
            Debug.LogWarning("Spawnpoint objesi bulunamadı!");
        }
    }

    private void Update()
    {
        
        // ESC tuşu kontrolü
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            Cursor.lockState = CursorLockMode.None;
        }

        Timer();
    }

    // Pause/Resume işlemini tek bir metodda topladık
    private void TogglePause()
    {
        if (pauseMenu == null)
        {
            Debug.LogError("PauseMenu referansı null! Lütfen Canvas > PauseMenu yapısını kontrol edin.");
            return;
        }

        isCounting = !isCounting;
        pauseMenu.SetActive(!isCounting);
        Time.timeScale = isCounting ? 1f : 0f;

        Debug.Log($"Oyun durumu: {(isCounting ? "Devam" : "Durdu")}");
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

    // Pause menüden çağrılabilecek metodlar
    public void ResumeGame()
    {
        isCounting = true;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        
    }

    public void PauseGame()
    {
        isCounting = false;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
}