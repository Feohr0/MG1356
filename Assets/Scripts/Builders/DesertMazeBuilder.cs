using UnityEngine;

public class DesertMazeBuilder : IMazeBuilder
{
    private GameObject mazeInstance;

    public void Reset() => mazeInstance = null;

    public void SetMazePrefab(GameObject prefab)
    {
        mazeInstance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        mazeInstance.name = "DesertMaze_Built";
    }

    public void ConfigureEnvironment()
    {
        RenderSettings.fogColor = new Color(0.9f, 0.8f, 0.5f);
        RenderSettings.fogDensity = 0.02f;
    }

    public void ConfigureEnemies()
    {
        Debug.Log("Çöl düþmanlarý eklendi (örnek).");
    }

    public GameObject GetResult() => mazeInstance;
}
