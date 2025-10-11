using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Zaten varsa yenisini sil
        }
    }





    // Start is called once before the first execution of Update after the MonoBehaviour is created
}

