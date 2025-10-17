using UnityEngine;

[CreateAssetMenu(menuName = "Maze/Forest Factory")]
public class ForestMazeFactory : MazeFactory
{
    public GameObject prefab;

    public override GameObject CreateMaze(Vector3 position)
    {
        IMazeBuilder builder = new ForestMazeBuilder();
        MazeDirector director = new MazeDirector(builder);
        GameObject maze = director.Construct(prefab);
        maze.transform.position = position;
        return maze;
    }
}
