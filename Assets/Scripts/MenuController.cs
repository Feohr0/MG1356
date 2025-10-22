using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // TextMeshPro k�t�phanesi

public class MenuController : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject nameInputPanel; // �sim girme paneli
    public GameObject mainPanel;      // Ana men� paneli (Play, Settings, Quit butonlar�)
    public GameObject settingsPanel;  // Ayarlar paneli

    [Header("UI Elementleri")]
    public Button[] buttons;
    public TMP_InputField nameInputField;

    private void Awake()
    {
        ButtonDeactive();
    }
    private void Start()
    {
        // Oyun ba�lang�c�nda sadece isim girme panelini aktif et

        nameInputPanel.SetActive(true);
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
       
        // Butonlar�n t�klama olaylar�n� ayarla
        ButtonFunctions();

        // InputField'de Enter'a bas�ld���nda (veya d�zenleme bitti�inde)
        // SubmitName fonksiyonunu �a��rmak i�in bir dinleyici ekle.
        // Bu, kodla ekleme y�ntemidir. Edit�rden de yap�labilir.
        nameInputField.onSubmit.AddListener(SubmitName);
    }

    /// <summary>
    /// InputField'e isim girilip Enter'a bas�ld���nda bu fonksiyon �al���r.
    /// </summary>
    /// <param name="playerName">InputField'den gelen metin.</param>
    public void SubmitName(string playerName)
    {
        // E�er isim bo� b�rak�lm��sa, varsay�lan bir isim ata
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Oyuncu";
        }

        // Oyuncu ad�n� GameManager'daki statik de�i�kene kaydet
        GameManager.currentPlayerName = playerName;
        Debug.Log("Oyuncu ad� ayarland�: " + GameManager.currentPlayerName);

        // �sim girme panelini kapat, ana men�y� a�
        nameInputPanel.SetActive(false);
        ButtonActive();
        mainPanel.SetActive(true);
    }

    void ButtonDeactive()
    {
        buttons[0].interactable = false;
        buttons[1].interactable = false;
        buttons[2].interactable = false;
    }
    void ButtonActive()
    {
        buttons[0].interactable = true;
        buttons[1].interactable = true;
        buttons[2].interactable = true;
    }
    /// <summary>
    /// Play butonu art�k sadece oyunu ba�lat�r. �sim alma i�i bitti.
    /// </summary>
    void Playbutton()
    {
        // GameManager'daki sayac� s�f�rla
        if (GameManager.Instance != null)
        {
            GameManager.Instance.timer = 0;
        }
        SceneManager.LoadScene("FirstMaze");
    }

    public void SettingsPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ButtonFunctions()
    {
        // Butonlara ilgili fonksiyonlar� ata
        buttons[0].onClick.AddListener(Playbutton);
        buttons[1].onClick.AddListener(SettingsPanel);
        buttons[2].onClick.AddListener(() => Application.Quit());
    }
}