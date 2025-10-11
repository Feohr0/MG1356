using UnityEngine;

[CreateAssetMenu(fileName = "WinterMazeFactory", menuName = "Factories/WinterMazeFactory")]
public class WinterMazeFactory : MazeFactory
{
    public GameObject WintermazePrefab; // inspector'da assign et

    public override GameObject CreateMaze(Vector3 position)
    {
        if (WintermazePrefab == null) return null;
        return Object.Instantiate(WintermazePrefab, position, Quaternion.identity);
    }
}
