using UnityEngine;

public class EnemyHard : Enemy
{
    public void Start()
    {
        base.DoStartStuff();
        health = 100;
    }
}