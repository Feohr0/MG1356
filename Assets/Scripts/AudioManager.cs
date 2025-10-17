using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq; // FindObjectsByType kullanmak i�in

public class AudioManager : MonoBehaviour
{
    // Singleton (Tekil) deseni
    public static AudioManager Instance;

    [Header("Buton Sesi Ayarlar�")]
    [Tooltip("T�m butonlar t�kland���nda �alacak ses klibi.")]
    public AudioClip clickSound;

    private AudioSource clickSource;

    // --- YEN� M�Z�K AYARLARI ---
    [Header("M�zik Ayarlar�")]
    [Tooltip("Arka plan m�zi�i klibi.")]
    public AudioClip bgMusic;

    private AudioSource musicSource;
    private bool isMusicPlaying = false; // M�zi�in aktif �al�p �almad���n� takip eder
    // ---------------------------
    
    void Awake()
    {

        // Singleton uygulamas�n�n temeli
        if (Instance == null)
        {
            Instance = this;
            // Sahne y�klemelerinde yok olmamas�n� sa�la
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 1. Click Ses Kayna�� (Mevcut AudioSource'u kullan)
        // GameManager nesnesi �zerinde bir AudioSource oldu�u varsay�l�r.
        AudioSource[] existingSources = GetComponents<AudioSource>();
        clickSource = existingSources.Length > 0 ? existingSources[0] : gameObject.AddComponent<AudioSource>();
        clickSource.playOnAwake = false;

        // 2. M�zik Ses Kayna�� (�kinci bir AudioSource ekle)
        if (existingSources.Length < 2)
        {
            // E�er sadece bir tane varsa, ikincisini ekle
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            // �ki tane varsa, ikincisini kullan
            musicSource = existingSources[1];
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        // Sahne y�klendi�inde buton dinleyicilerini kurmak i�in abone ol
        SceneManager.sceneLoaded += OnSceneLoaded;
        musicSource.loop = true;
    }

    private void OnDisable()
    {
        // Abonelikten ��k
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Sahne y�klendi�inde �a�r�l�r
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Yeni sahnede buton ses dinleyicilerini kur
        SetupButtonSoundListeners();

        // Sahne y�klendi�inde m�zi�i ba�latmaya �al�� (Update'te kontrol edilecek)
        if (bgMusic != null)
        {
            musicSource.clip = bgMusic;
            // musicSource.Play() burada �a�r�lmaz, ��nk� Update() i�indeki MusicPlayer() kontrol edecektir.
        }
    }

    void Update()
    {
        MusicPlayer();
    }

    /// <summary>
    /// GameManager'�n isCounting durumuna g�re m�zi�i oynat�r/durdurur.
    /// </summary>
    public void MusicPlayer()
    {
        
        // GameManager'a eri�im sa�la (E�er DontDestroyOnLoad ile ta��n�yorsa bu kontrol �nemli)
        if (GameManager.Instance == null || bgMusic == null)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
                isMusicPlaying = false;
            }
            return;
        }

        // M�zik �almas� GEREK�YORSA (isCounting == true)
        if (GameManager.Instance.isCounting == true)
        {
            if (!isMusicPlaying)
            {
                // M�zik klibi atanm��sa ba�lat
                if (musicSource.clip != bgMusic)
                {
                    musicSource.clip = bgMusic;
                }
                musicSource.Play();
                isMusicPlaying = true;
            }
        }
        // M�zik DURMASI GEREK�YORSA (isCounting == false, yani duraklat�ld�, kaybedildi veya kazan�ld�)
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
    /// Sahnede bulunan t�m UI Button bile�enlerine t�klama sesi dinleyicisini ekler.
    /// </summary>
    public void SetupButtonSoundListeners()
    {
        if (clickSound == null)
        {
            Debug.LogWarning("AudioManager: ClickSound atanmam��. Buton sesleri �al�nmayacak.");
            return;
        }

        // Sahnede aktif olan t�m Button bile�enlerini bul
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        int listenerCount = 0;

        foreach (Button btn in buttons)
        {
            // Butona dinleyici ekle.
            btn.onClick.AddListener(PlayButtonClick);
            listenerCount++;
        }

        Debug.Log($"AudioManager: Yeni y�klenen sahnede {listenerCount} butona ses dinleyicisi eklendi.");
    }

    /// <summary>
    /// Buton t�klamas� olay�nda �a�r�lacak olan ses �alma metodu.
    /// </summary>
    public void PlayButtonClick()
    {
        if (clickSource != null && clickSound != null)
        {
            clickSource.PlayOneShot(clickSound);
        }
    }
}
