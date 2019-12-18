using System.Collections;

public class EnemyHard : Enemy
{
    public void Start()
    {
        base.DoStartStuff();
        health = 100;
    }
    public override IEnumerator Die()
    {
        ScoreKeeper.UpdateScore(150);
        StartCoroutine(base.Die());
        yield return null;
    }
}