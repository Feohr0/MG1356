using UnityEngine;

[CreateAssetMenu(fileName = "DesertMazeFactory", menuName = "Factories/DesertFactory")]
public  class DesertMazeFactory : MazeFactory
{
    public GameObject DesertmazePrefab; // inspector'da assign et

    public override GameObject CreateMaze(Vector3 position)
    {
        if (DesertmazePrefab == null) return null;
        return Object.Instantiate(DesertmazePrefab, position, Quaternion.identity);
    }
}