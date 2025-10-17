using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    [SerializeField] TMP_Text currentText;
    [SerializeField] TMP_Text bestText;

    void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnScoreChanged += Handle;
        Handle(GameManager.Instance.Score); // 초기 동기화
    }
    void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnScoreChanged -= Handle;
    }

    void Handle(int s)
    {
        if (currentText) currentText.text = s.ToString();
        if (bestText) bestText.text = $"BEST {GameManager.Instance.BestScore}";
    }
}
