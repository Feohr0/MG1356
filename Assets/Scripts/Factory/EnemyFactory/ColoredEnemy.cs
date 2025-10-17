using UnityEngine;

public class ColoredEnemy : Enemy
{
    public override Enemy Clone()
    {
        return Instantiate(this);
    }
}
