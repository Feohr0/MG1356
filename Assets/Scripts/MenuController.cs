using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button[] buttons;
    public GameObject settingsPanel;
    public GameObject mainPanel;
    GameManager gameManager;

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
    void Playbutton()
    {
        SceneManager.LoadScene("FirstMaze");
        gameManager.timer = 0;
    }
    public void ButtonFunctions()
    {
        buttons[0].onClick.AddListener(() => Playbutton());
        buttons[1].onClick.AddListener(() => SettingsPanel());
        buttons[2].onClick.AddListener(() => Application.Quit());

    }
   
}
