using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    // Update metodunu tamamen kald�r, gerek yok

    public void Rest()
    {
        // Oyunu normale d�nd�r
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}