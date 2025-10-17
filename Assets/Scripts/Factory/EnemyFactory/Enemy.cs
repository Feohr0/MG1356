using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public string enemyName;
    public float health;
    public float speed;

    public abstract Enemy Clone(); // Prototype
}
