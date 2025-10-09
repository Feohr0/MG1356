using GLTFast;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject pauseMenu;
    public static GameManager Instance;
    public float timer;
    public bool isCounting = false;
    public TextMeshProUGUI timerText;
    void Awake()
    {
        timerText = TextMeshProUGUI.FindAnyObjectByType<TextMeshProUGUI>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Zaten varsa yenisini sil
        }
    }
    private void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        isCounting = true;
        SpawnPlayer();
    }
    private void OnEnable()
    {
        // Sahne yüklendiðinde SpawnPlayer çaðrýlacak
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnPlayer();
        GameObject tmpObj = GameObject.Find("timerText");
        if (tmpObj != null)
        {
            timerText = tmpObj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            timerText = null;
            Debug.LogWarning("timerText objesi sahnede bulunamadý!");
        }
    }
    void SpawnPlayer()
    {
        // Spawnpoint objesini bul
        GameObject spawnObj = GameObject.Find("SpawnPoint");
        if (spawnObj != null)
        {
            Instantiate(playerPrefab, spawnObj.transform.position, spawnObj.transform.rotation);
        }
        else
        {
            Debug.LogWarning("Spawnpoint objesi bulunamadý!");
        }
    }
    private void Update()
    {
        Timer();
        if (isCounting && Input.GetKeyDown(KeyCode.Escape))
        {
            isCounting=false;
            pauseMenu.SetActive(true);
        }
        if (!isCounting && Input.GetKeyDown(KeyCode.Escape))
        {
            isCounting = true;
            pauseMenu.SetActive(false);
        }
    }
    public void Timer()
    {
        if (isCounting)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Time.timeScale = 0f;
        }
            int min = Mathf.FloorToInt(timer / 60);
        int secs = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{0:00}:{1:00}", min, secs);
    }
}