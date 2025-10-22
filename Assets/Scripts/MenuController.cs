using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // TextMeshPro kütüphanesi

public class MenuController : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject nameInputPanel; // Ýsim girme paneli
    public GameObject mainPanel;      // Ana menü paneli (Play, Settings, Quit butonlarý)
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
        // Oyun baþlangýcýnda sadece isim girme panelini aktif et

        nameInputPanel.SetActive(true);
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
       
        // Butonlarýn týklama olaylarýný ayarla
        ButtonFunctions();

        // InputField'de Enter'a basýldýðýnda (veya düzenleme bittiðinde)
        // SubmitName fonksiyonunu çaðýrmak için bir dinleyici ekle.
        // Bu, kodla ekleme yöntemidir. Editörden de yapýlabilir.
        nameInputField.onSubmit.AddListener(SubmitName);
    }

    /// <summary>
    /// InputField'e isim girilip Enter'a basýldýðýnda bu fonksiyon çalýþýr.
    /// </summary>
    /// <param name="playerName">InputField'den gelen metin.</param>
    public void SubmitName(string playerName)
    {
        // Eðer isim boþ býrakýlmýþsa, varsayýlan bir isim ata
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Oyuncu";
        }

        // Oyuncu adýný GameManager'daki statik deðiþkene kaydet
        GameManager.currentPlayerName = playerName;
        Debug.Log("Oyuncu adý ayarlandý: " + GameManager.currentPlayerName);

        // Ýsim girme panelini kapat, ana menüyü aç
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
    /// Play butonu artýk sadece oyunu baþlatýr. Ýsim alma iþi bitti.
    /// </summary>
    void Playbutton()
    {
        // GameManager'daki sayacý sýfýrla
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
        // Butonlara ilgili fonksiyonlarý ata
        buttons[0].onClick.AddListener(Playbutton);
        buttons[1].onClick.AddListener(SettingsPanel);
        buttons[2].onClick.AddListener(() => Application.Quit());
    }
}