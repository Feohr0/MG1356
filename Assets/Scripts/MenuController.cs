using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button[] buttons;
    public GameObject settingsPanel;
    public GameObject mainPanel;

  
    private void Start()
    {
        ButtonFunctions();
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
    public void SettingsPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        
    }
    public void SettingsClosed()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    public void ButtonFunctions()
    {
        buttons[0].onClick.AddListener(() => SceneManager.LoadScene("FirstMaze"));
        buttons[1].onClick.AddListener(() => SettingsPanel());
        buttons[2].onClick.AddListener(() => Application.Quit());
        buttons[3].onClick.AddListener(() => SettingsClosed());
    }
}
