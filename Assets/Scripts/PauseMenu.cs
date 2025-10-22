using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button[] pauseButtons;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsPanel;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        ButtonFunctions();
    }

    void ResumeButton()
    {
        gameManager.ResumeGame();
    }

    public void SettingsPanel()
    {
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // YENİ FONKSİYON: Çıkış yap ve ana menüye dön
    public void Logout()
    {
        // Oyun zamanını tekrar normale döndür, çünkü sahne değişse de Time.timeScale=0 kalabilir.
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ButtonFunctions()
    {
        pauseButtons[0].onClick.AddListener(() => ResumeButton());
        pauseButtons[1].onClick.AddListener(() => SettingsPanel());
        // GÜNCELLENDİ: 3. buton artık direkt sahne yüklemek yerine Logout fonksiyonunu çağırıyor.
        pauseButtons[2].onClick.AddListener(() => Logout());
    }
}