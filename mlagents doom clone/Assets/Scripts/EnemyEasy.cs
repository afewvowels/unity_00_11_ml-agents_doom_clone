using UnityEngine;

public class EnemyEasy : Enemy
{
    public void Start()
    {
        base.DoStartStuff();
        health = 25;
    }
}