using UnityEngine;
using System.Collections;

public class EnemyBoss : Enemy
{
    public void Start()
    {
        base.DoStartStuff();
        health = 200;
    }

    public override IEnumerator Die()
    {
        ScoreKeeper.UpdateScore(300);
        StartCoroutine(base.Die());
        yield return null;
    }
}