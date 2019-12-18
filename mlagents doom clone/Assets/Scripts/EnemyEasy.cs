using System.Collections;

public class EnemyEasy : Enemy
{
    public void Start()
    {
        base.DoStartStuff();
        health = 25;
    }

    public override IEnumerator Die()
    {
        ScoreKeeper.UpdateScore(50);
        StartCoroutine(base.Die());
        yield return null;
    }
}