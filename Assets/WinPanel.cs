using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public Button [] mainmenu;

    private void Start()
    {
        MainMenu();
    }
    public void MainMenu()
    {
        mainmenu[0].onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }
}
