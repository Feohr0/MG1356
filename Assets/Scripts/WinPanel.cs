using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public Button[] buttons; 
    private void Start()
    {
        Buttons();
    }

    public void MainMenu()
    {

        LoadMainMenu();
    }

   public void Buttons()
    {
        buttons[0].onClick.AddListener(LoadMainMenu);
        buttons[1].onClick.AddListener(LoadMainMenu);

    }
    public void LoadMainMenu()
    {
        // Time scale'i normale döndür (önemli!)
        Time.timeScale = 1f;

        // Cursor'u serbest býrak
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // GameManager'ý temizle

        GameManager.Instance.isCounting = false;
       


        // Main Menu sahnesini yükle
        SceneManager.LoadScene("MainMenu");

        Debug.Log("Ana menüye dönülüyor...");
    }
}