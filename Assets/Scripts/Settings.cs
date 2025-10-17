using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq; // FindObjectOfType için gerekmese de PlayerMovement'ı bulmak için kullanışlı olabilir.

public class Settings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] private Slider m_Slider; // İsim eski haline döndürüldü
    [SerializeField] private Slider sens; // İsim eski haline döndürüldü

    // Panel referansları
    public GameObject settingsPanel;
    public GameObject mainPanel;

    // PlayerMovement referansı
    private PlayerMovement plyr;

    // Sabit Anahtarlar
    private const string VolumeKey = "SoundVolume";
    private const string SensitivityKey = "MouseSensitivity";
    private const float DefaultSensitivity = 100f; // Varsayılan değer 
    private const float DefaultVolume = 1f; // Varsayılan değer

    private void Awake()
    {
        // PlayerMovement'ı sahnede bul (Player objesinin üzerinde olduğunu varsayıyoruz)
        // Oyuncu objesi sahnede tek olmalı.
        plyr = FindObjectOfType<PlayerMovement>();

        if (plyr == null)
        {
            Debug.LogError("PlayerMovement script'i sahnede bulunamadı! Mouse hassasiyeti ayarlanmayacak.");
        }
    }

    private void Start()
    {
        // Slider referanslarını Start yerine Awake'te atanmış gibi varsayarak ilerliyoruz.
        // Eğer bu slider'lar Settings objesinin üstündeyse, inspector'da atanmış olmalıdır.

        LoadSettings();
    }

    // Ayarları PlayerPrefs'ten yükleyen ana metot
    private void LoadSettings()
    {
        // --- SES AYARLARI ---
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);

        // Slider değerini yükle
        m_Slider.value = savedVolume;

        // Ses mikserine uygula (Bu adım eksikti)
        SetVolume(savedVolume);

        // Slider'a Event Listener ekle
        m_Slider.onValueChanged.AddListener(SetVolume);


        // --- HASSASİYET AYARLARI ---
        float savedSensitivityMultiplier = PlayerPrefs.GetFloat(SensitivityKey, DefaultSensitivity);

        // Mouse hassasiyetini PlayerMovement script'ine uygula
        if (plyr != null)
        {
            // Kaydedilen değerin PlayerMovement'a atanması
            plyr.mouseSensitivity = savedSensitivityMultiplier;
        }

        // Slider değerini yükle (PlayerMovement'a atanan değeri slider'ın aralığına göre ölçeklendir)
        // Varsayılan PlayerMovement hassasiyeti 100, slider 1-10 arasıysa:
        // sens.value = savedSensitivityMultiplier / 10f;
        // Eğer PlayerMovement hassasiyeti ve Slider değeri aynı aralıktaysa (örn. 1-100):
        sens.value = savedSensitivityMultiplier;

        // Slider'a Event Listener ekle
        sens.onValueChanged.AddListener(SetSensitivity);
    }

    // Sesi ayarlar ve kaydeder
    public void SetVolume(float volume)
    {
        // Ses mikserine uygulama (0 değeri log10(0)*20 ile hataya neden olacağı için kontrol)
        if (volume <= 0.0001f)
        {
            audioMixer.SetFloat("Sound", -80f); // Minimum ses seviyesi
        }
        else
        {
            audioMixer.SetFloat("Sound", Mathf.Log10(volume) * 20);
        }

        // Değeri kaydet
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save(); // Kaydı hemen diske yaz
    }

    // Hassasiyeti ayarlar ve kaydeder
    public void SetSensitivity(float sensitivityValue)
    {
        if (plyr != null)
        {
            // Slider değerini PlayerMovement script'ine ata
            plyr.mouseSensitivity = sensitivityValue;
        }

        // Değeri kaydet
        PlayerPrefs.SetFloat(SensitivityKey, sensitivityValue);
        PlayerPrefs.Save(); // Kaydı hemen diske yaz
    }

    public void SettingsClosed()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // Editörde kolaylık için değişken isimlerini düzeltelim
    // Eski isimler m_Slider ve sens idi.
    public void SetVolume()
    {
        // SetVolume(float volume) metoduna yönlendiriyoruz
        SetVolume(m_Slider.value);
    }
    public void Setsens()
    {
        // SetSensitivity(float sensitivityValue) metoduna yönlendiriyoruz
        SetSensitivity(sens.value);
    }
}