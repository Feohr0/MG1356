using UnityEngine;

public abstract class MazeFactory : ScriptableObject
{
    // Pozisyon parametresi opsiyonel, gerekli ise al
    public abstract GameObject CreateMaze(Vector3 position);
}
