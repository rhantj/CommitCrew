using UnityEngine;

public class UIStateBinder : MonoBehaviour
{
    [SerializeField] GameObject mainMenuCanvas; // Start
    [SerializeField] GameObject readyCanvas;    // Ready
    [SerializeField] GameObject inGameCanvas;   // Playing (HUD)
    [SerializeField] GameObject gameOverCanvas; // GameOver

    void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnStateChanged += Handle;
        Handle(GameManager.Instance.State);
    }
    void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnStateChanged -= Handle;
    }

    void Handle(GameState st)
    {
        bool isStart = st == GameState.Start;
        bool isReady = st == GameState.Ready;
        bool isPlay = st == GameState.Playing;
        bool isOver = st == GameState.GameOver;
        Debug.Log($"[UI] {st}  Main:{mainMenuCanvas?.activeSelf} Ready:{readyCanvas?.activeSelf} Play:{inGameCanvas?.activeSelf} Over:{gameOverCanvas?.activeSelf}");

        if (mainMenuCanvas) mainMenuCanvas.SetActive(isStart);
        if (readyCanvas) readyCanvas.SetActive(isReady);
        if (inGameCanvas) inGameCanvas.SetActive(isPlay);
        if (gameOverCanvas) gameOverCanvas.SetActive(isOver);
    }
}
