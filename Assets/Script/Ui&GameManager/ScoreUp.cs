using UnityEngine;
using TMPro;

public class ScoreUp : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;

    void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnScoreChanged += UpdateScoreText;
    }

    void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnScoreChanged -= UpdateScoreText;
    }

    void UpdateScoreText(int newScore)
    {
        if (scoreText)
            scoreText.text = newScore.ToString();
    }
}
