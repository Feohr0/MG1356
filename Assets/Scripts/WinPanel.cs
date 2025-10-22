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
        // Time scale'i normale d�nd�r (�nemli!)
        Time.timeScale = 1f;

        // Cursor'u serbest b�rak
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // GameManager'� temizle

        GameManager.Instance.isCounting = false;
       


        // Main Menu sahnesini y�kle
        SceneManager.LoadScene("MainMenu");

        Debug.Log("Ana men�ye d�n�l�yor...");
    }
}