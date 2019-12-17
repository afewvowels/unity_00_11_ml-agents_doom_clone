using UnityEngine;

public class EnemyBoss : Enemy
{
    public void Start()
    {
        base.DoStartStuff();
        health = 200;
    }
}