using UnityEngine;

[CreateAssetMenu(menuName = "Maze/Winter Factory")]
public class WinterMazeFactory : MazeFactory
{
    public GameObject prefab;

    public override GameObject CreateMaze(Vector3 position)
    {
        IMazeBuilder builder = new WinterMazeBuilder();
        MazeDirector director = new MazeDirector(builder);
        GameObject maze = director.Construct(prefab);
        maze.transform.position = position;
        return maze;
    }
}
