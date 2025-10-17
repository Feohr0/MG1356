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
        gameManager = GameManager.Instance; // 🔹 GameManager referansını al
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

    public void ButtonFunctions()
    {
        pauseButtons[0].onClick.AddListener(() => ResumeButton());
        pauseButtons[1].onClick.AddListener(() => SettingsPanel());
        pauseButtons[2].onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

    }
}