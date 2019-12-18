using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    public static Text scoreText;
    public static int score;


    void Start()
    {
        score = 0;
        scoreText = gameObject.GetComponent<Text>();
    }

    public static void UpdateScore(int score)
    {
        ScoreKeeper.score += score;
        ScoreKeeper.scoreText.text = ScoreKeeper.score.ToString();
    }
}
