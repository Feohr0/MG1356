using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] private Slider m_Slider;
    [SerializeField] private Slider sens;
    
     public GameObject settingsPanel;
     public GameObject mainPanel;
    PlayerMovement plyr;
    private void Start()
    {
        plyr.mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 100);
        sens.value = plyr.mouseSensitivity / 10;
        if (PlayerPrefs.HasKey("soundVolume"))
        {
            Volume();
        }
        else
        {
            SetVolume();
        }
        if (PlayerPrefs.HasKey("plyr.mouseSensitivity"))
        {
            Sensivity();
        }
        else
        {
            Setsens();
        }
            
    }
    public void SetVolume()
    {
        float volume = m_Slider.value;
        audioMixer.SetFloat("Sound", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("soundVolume",volume);
    }
    void Volume()
    {
        m_Slider.value = PlayerPrefs.GetFloat("soundVolume");
    }
    public void Setsens()
    {
       float mousesens = sens.value;
        plyr.mouseSensitivity = mousesens;
        PlayerPrefs.SetFloat("plyr.mouseSensitivity", mousesens);
        
    }
    void Sensivity()
    {
        sens.value = PlayerPrefs.GetFloat("plyr.mouseSensitivity");
    }
    public void SettingsClosed()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
