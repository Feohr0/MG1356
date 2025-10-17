using GLTFast.Schema;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndController : MonoBehaviour
{
    public GameObject end1;
    GameManager gameManager;
    private void Start()
    {
        SceneManager.GetActiveScene();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag("Player")) {
            if (SceneManager.GetActiveScene().name == ("FirstMaze"))
            {
                SceneManager.LoadScene("SecondMaze");
            }
            if (SceneManager.GetActiveScene().name == ("SecondMaze"))
            {
                SceneManager.LoadScene("ThirdMaze");
            }
            if (SceneManager.GetActiveScene().name == ("ThirdMaze"))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameWon();
                }
            }
        }
    }
}
