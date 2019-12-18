using System.Collections;

public class EnemyMedium : Enemy
{
    public void Start()
    {
        base.DoStartStuff();
        health = 50;
    }
    public override IEnumerator Die()
    {
        ScoreKeeper.UpdateScore(100);
        StartCoroutine(base.Die());
        yield return null;
    }
}