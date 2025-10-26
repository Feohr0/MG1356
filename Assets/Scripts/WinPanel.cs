using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    // Update metodunu tamamen kaldýr, gerek yok

    public void Rest()
    {
        // Oyunu normale döndür
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}