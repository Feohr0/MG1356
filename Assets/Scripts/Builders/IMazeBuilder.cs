using UnityEngine;


    public interface IMazeBuilder
    {
        void Reset();
        void SetMazePrefab(GameObject prefab);
        void ConfigureEnvironment();
        void ConfigureEnemies();
        GameObject GetResult();
    }

