using UnityEngine;

public class RandomEnemyFactory : EnemyFactory
{
    private ColoredEnemy[] _enemyPrefabs;

    public RandomEnemyFactory(ColoredEnemy[] prefabs)
    {
        _enemyPrefabs = prefabs;
    }

    public override Enemy CreateEnemy()
    {
        if (_enemyPrefabs == null || _enemyPrefabs.Length == 0)
        {
            Debug.LogError("Enemy prefabs boþ!");
            return null;
        }

        int index = Random.Range(0, _enemyPrefabs.Length);
        return _enemyPrefabs[index].Clone();
    }
}

