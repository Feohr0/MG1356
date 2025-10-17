using UnityEngine;

public class MazeDirector
{
    private IMazeBuilder builder;

    public MazeDirector(IMazeBuilder builder)
    {
        this.builder = builder;
    }

    public GameObject Construct(GameObject prefab)
    {
        builder.Reset();
        builder.SetMazePrefab(prefab);
        builder.ConfigureEnvironment();
        builder.ConfigureEnemies();
        return builder.GetResult();
    }
}
