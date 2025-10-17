using UnityEngine;

[CreateAssetMenu(menuName = "Maze/Desert Factory")]
public class DesertMazeFactory : MazeFactory
{
    public GameObject prefab;

    public override GameObject CreateMaze(Vector3 position)
    {
        IMazeBuilder builder = new DesertMazeBuilder();
        MazeDirector director = new MazeDirector(builder);
        GameObject maze = director.Construct(prefab);
        maze.transform.position = position;
        return maze;
    }
}
