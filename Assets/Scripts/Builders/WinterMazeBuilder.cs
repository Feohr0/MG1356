using UnityEngine;

public class WinterMazeBuilder : IMazeBuilder
{
    private GameObject mazeInstance;

    public void Reset() => mazeInstance = null;

    public void SetMazePrefab(GameObject prefab)
    {
        mazeInstance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        mazeInstance.name = "WinterMaze_Built";
    }

    public void ConfigureEnvironment()
    {
        RenderSettings.fogColor = new Color(0.7f, 0.9f, 1f);
        RenderSettings.fogDensity = 0.015f;
    }

    public void ConfigureEnemies()
    {
        Debug.Log("Kýþ düþmanlarý eklendi (örnek).");
    }

    public GameObject GetResult() => mazeInstance;
}
