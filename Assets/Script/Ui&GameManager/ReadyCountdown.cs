using UnityEngine;
using TMPro;
using System.Collections;

public class ReadyCountdown : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText; // UI 텍스트
    [SerializeField] private float countdownSeconds = 3f; // 원하는 카운트 시간

    private void OnEnable()
    {
        // GameManager가 준비상태(Ready)일 때만 카운트 시작
        if (GameManager.Instance != null && GameManager.Instance.State == GameState.Ready)
        {
            StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown()
    {
        float t = countdownSeconds;
        while (t > 0)
        {
            if (countdownText) countdownText.text = Mathf.Ceil(t).ToString();
            t -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (countdownText) countdownText.text = "";
        GameManager.Instance?.StartGame(); // ✅ 카운트 끝나면 바로 게임 시작
    }
}
