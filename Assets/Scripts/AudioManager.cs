using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq; // FindObjectsByType kullanmak için

public class AudioManager : MonoBehaviour
{
    // Singleton (Tekil) deseni
    public static AudioManager Instance;

    [Header("Buton Sesi Ayarlarý")]
    [Tooltip("Tüm butonlar týklandýðýnda çalacak ses klibi.")]
    public AudioClip clickSound;

    private AudioSource clickSource;

    // --- YENÝ MÜZÝK AYARLARI ---
    [Header("Müzik Ayarlarý")]
    [Tooltip("Arka plan müziði klibi.")]
    public AudioClip bgMusic;

    private AudioSource musicSource;
    private bool isMusicPlaying = false; // Müziðin aktif çalýp çalmadýðýný takip eder
    // ---------------------------
    
    void Awake()
    {

        // Singleton uygulamasýnýn temeli
        if (Instance == null)
        {
            Instance = this;
            // Sahne yüklemelerinde yok olmamasýný saðla
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 1. Click Ses Kaynaðý (Mevcut AudioSource'u kullan)
        // GameManager nesnesi üzerinde bir AudioSource olduðu varsayýlýr.
        AudioSource[] existingSources = GetComponents<AudioSource>();
        clickSource = existingSources.Length > 0 ? existingSources[0] : gameObject.AddComponent<AudioSource>();
        clickSource.playOnAwake = false;

        // 2. Müzik Ses Kaynaðý (Ýkinci bir AudioSource ekle)
        if (existingSources.Length < 2)
        {
            // Eðer sadece bir tane varsa, ikincisini ekle
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            // Ýki tane varsa, ikincisini kullan
            musicSource = existingSources[1];
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        // Sahne yüklendiðinde buton dinleyicilerini kurmak için abone ol
        SceneManager.sceneLoaded += OnSceneLoaded;
        musicSource.loop = true;
    }

    private void OnDisable()
    {
        // Abonelikten çýk
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Sahne yüklendiðinde çaðrýlýr
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Yeni sahnede buton ses dinleyicilerini kur
        SetupButtonSoundListeners();

        // Sahne yüklendiðinde müziði baþlatmaya çalýþ (Update'te kontrol edilecek)
        if (bgMusic != null)
        {
            musicSource.clip = bgMusic;
            // musicSource.Play() burada çaðrýlmaz, çünkü Update() içindeki MusicPlayer() kontrol edecektir.
        }
    }

    void Update()
    {
        MusicPlayer();
    }

    /// <summary>
    /// GameManager'ýn isCounting durumuna göre müziði oynatýr/durdurur.
    /// </summary>
    public void MusicPlayer()
    {
        
        // GameManager'a eriþim saðla (Eðer DontDestroyOnLoad ile taþýnýyorsa bu kontrol önemli)
        if (GameManager.Instance == null || bgMusic == null)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
                isMusicPlaying = false;
            }
            return;
        }

        // Müzik çalmasý GEREKÝYORSA (isCounting == true)
        if (GameManager.Instance.isCounting == true)
        {
            if (!isMusicPlaying)
            {
                // Müzik klibi atanmýþsa baþlat
                if (musicSource.clip != bgMusic)
                {
                    musicSource.clip = bgMusic;
                }
                musicSource.Play();
                isMusicPlaying = true;
            }
        }
        // Müzik DURMASI GEREKÝYORSA (isCounting == false, yani duraklatýldý, kaybedildi veya kazanýldý)
        else // GameManager.Instance.isCounting == false
        {
            if (isMusicPlaying)
            {
                musicSource.Pause();
                isMusicPlaying = false;
            }
        }
    }

    /// <summary>
    /// Sahnede bulunan tüm UI Button bileþenlerine týklama sesi dinleyicisini ekler.
    /// </summary>
    public void SetupButtonSoundListeners()
    {
        if (clickSound == null)
        {
            Debug.LogWarning("AudioManager: ClickSound atanmamýþ. Buton sesleri çalýnmayacak.");
            return;
        }

        // Sahnede aktif olan tüm Button bileþenlerini bul
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        int listenerCount = 0;

        foreach (Button btn in buttons)
        {
            // Butona dinleyici ekle.
            btn.onClick.AddListener(PlayButtonClick);
            listenerCount++;
        }

        Debug.Log($"AudioManager: Yeni yüklenen sahnede {listenerCount} butona ses dinleyicisi eklendi.");
    }

    /// <summary>
    /// Buton týklamasý olayýnda çaðrýlacak olan ses çalma metodu.
    /// </summary>
    public void PlayButtonClick()
    {
        if (clickSource != null && clickSound != null)
        {
            clickSource.PlayOneShot(clickSound);
        }
    }
}
