using UnityEngine;

public class EnemyMedium : Enemy
{
    public void Start()
    {
        base.DoStartStuff();
        health = 50;
    }
}