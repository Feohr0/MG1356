using UnityEngine;

public class ForestMazeBuilder : IMazeBuilder
{
    private GameObject mazeInstance;

    public void Reset() => mazeInstance = null;

    public void SetMazePrefab(GameObject prefab)
    {
        mazeInstance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        mazeInstance.name = "ForestMaze_Built";
    }

    public void ConfigureEnvironment()
    {
        RenderSettings.fogColor = new Color(0.3f, 0.5f, 0.3f);
        RenderSettings.fogDensity = 0.01f;
    }

    public void ConfigureEnemies()
    {
        Debug.Log("Orman düþmanlarý eklendi (örnek).");
    }

    public GameObject GetResult() => mazeInstance;
}
