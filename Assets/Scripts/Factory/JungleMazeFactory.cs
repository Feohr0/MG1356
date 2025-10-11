using UnityEngine;

[CreateAssetMenu(fileName = "JungleMazeFactory", menuName = "Factories/JungleFactory")]
public class JungleMazeFactory : MazeFactory
{
    public GameObject JunglemazePrefab; // inspector'da assign et

    public override GameObject CreateMaze(Vector3 position)
    {
        if (JunglemazePrefab == null) return null;
        return Object.Instantiate(JunglemazePrefab, position, Quaternion.identity);
    }
}