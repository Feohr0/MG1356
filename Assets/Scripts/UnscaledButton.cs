using UnityEngine;
using UnityEngine.UI;

public class UnscaledButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        // Buton Time.timeScale'den ba��ms�z �al��s�n
        if (Input.GetMouseButtonDown(0) && Time.timeScale == 0f)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                GetComponent<RectTransform>(),
                Input.mousePosition,
                null))
            {
                button.onClick.Invoke();
            }
        }
    }
}