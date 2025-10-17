using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Refs - Runtime")]
    [SerializeField] private PlayerController player;       // 런타임에 인스턴스 생성 후 담김
    [SerializeField] private MapController map;             // Begin()/Stop()
    [SerializeField] private PillarSpawner spawner;         // Begin()/Stop()/Clear()

    [Header("Spawn")]
    [SerializeField] private PlayerController playerPrefab; // ✅ 프리팹 드래그
    [SerializeField] private Transform playerSpawnPoint;    // ✅ 스폰 위치(없으면 (0,0,0))

    [Header("UI (Direct Toggle)")]
    [SerializeField] private GameObject mainMenuCanvas;     // 메인
    [SerializeField] private GameObject readyCanvas;        // 준비(카운트다운)
    [SerializeField] private GameObject inGameCanvas;       // 인게임 HUD
    [SerializeField] private GameObject gameOverCanvas;     // 게임오버

    [Header("State/Score")]
    [SerializeField] private GameState state = GameState.Start;
    public GameState State => state;

    [SerializeField] private int score = 0;
    public int Score => score;

    [SerializeField] private int bestScore = 0;
    public int BestScore => bestScore;

    public event Action<int> OnScoreChanged;
    public event Action<GameState> OnStateChanged;
    public event Action<float> OnReadyBegin;

    const string BEST_KEY = "BEST_SCORE";
    Coroutine startFlowCo;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        bestScore = PlayerPrefs.GetInt(BEST_KEY, 0);
    }

    void Start()
    {
        StartCoroutine(InitAfterAwake()); // 모든 Awake 이후 안전하게 초기화
    }

    IEnumerator InitAfterAwake()
    {
        yield return null;
        EnsurePlayerSpawned();
        GoToMainMenu();  // ▶ 실행하면 메인 화면부터
    }

    // ========== Public API ==========

    public void StartWithReady(float seconds = 1.5f)
    {
        if (startFlowCo != null) StopCoroutine(startFlowCo);
        startFlowCo = StartCoroutine(StartFlow(seconds));
    }

    IEnumerator StartFlow(float seconds)
    {
        score = 0; OnScoreChanged?.Invoke(score);
        spawner?.Clear();
        map?.Stop();

        EnsurePlayerSpawned();
        player?.PrepareIdle();

        SetState(GameState.Ready);
        float wait = Mathf.Max(0.5f, seconds);
        OnReadyBegin?.Invoke(wait);
        yield return new WaitForSecondsRealtime(wait);

        StartGame();
        startFlowCo = null;
    }

    public void StartGame()
    {
        EnsurePlayerSpawned();
        player?.ResetForGame();
        map?.Begin();
        spawner?.Begin();
        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        if (state != GameState.Playing) return;
        spawner?.Stop();
        map?.Stop();
        SetState(GameState.GameOver);
    }

    public void GoToMainMenu()
    {
        spawner?.Stop();
        spawner?.Clear();
        map?.Stop();
        score = 0; OnScoreChanged?.Invoke(score);

        EnsurePlayerSpawned();
        player?.PrepareIdle();

        SetState(GameState.Start);
    }

    public void RestartScene()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int v)
    {
        if (state != GameState.Playing) return;
        score = Mathf.Max(0, score + v);
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BEST_KEY, bestScore);
        }
        OnScoreChanged?.Invoke(score);
    }

    // ========== Internal ==========

    void SetState(GameState next)
    {
        state = next;
        Time.timeScale = 1f;

        // ✅ 여기서 UI를 직접 토글 (참조만 제대로 연결되면 100% 동작)
        if (mainMenuCanvas) mainMenuCanvas.SetActive(next == GameState.Start);
        if (readyCanvas) readyCanvas.SetActive(next == GameState.Ready);
        if (inGameCanvas) inGameCanvas.SetActive(next == GameState.Playing);
        if (gameOverCanvas) gameOverCanvas.SetActive(next == GameState.GameOver);

        OnStateChanged?.Invoke(state);
    }

    void EnsurePlayerSpawned()
    {
        if (player == null)
        {
            if (playerPrefab == null)
            {
                Debug.LogError("GameManager: playerPrefab이 비어있습니다. 인스펙터에 드래그하세요.");
                return;
            }
            Vector3 pos = playerSpawnPoint ? playerSpawnPoint.position : Vector3.zero;
            player = Instantiate(playerPrefab, pos, Quaternion.identity);
        }

        if (playerSpawnPoint)
            player.spawnPoint = playerSpawnPoint; // 컨트롤러에도 스폰포인트 전달
    }
    public void ReplayToReady(float seconds = 1.2f)
    {
        // GameOver에서만 동작(원하면 이 가드 제거 가능)
        if (state != GameState.GameOver) Debug.LogWarning("ReplayToReady: not in GameOver");
        if (startFlowCo != null) StopCoroutine(startFlowCo);
        startFlowCo = StartCoroutine(ReplayFlow(seconds));
    }

    private IEnumerator ReplayFlow(float seconds)
    {
        // 1) 진행 중 요소 정지/정리
        spawner?.Stop();
        spawner?.Clear();
        map?.Stop();

        // 2) 점수 초기화
        score = 0;
        OnScoreChanged?.Invoke(score);

        // 3) 플레이어 대기자세(화면 중앙 고정, 중력 0)
        EnsurePlayerSpawned();
        player?.PrepareIdle();

        // 4) Ready 화면으로 전환(Ready UI 켜짐)
        SetState(GameState.Ready);

        // Ready 카운트(표시용 이벤트도 발행)
        float wait = Mathf.Max(0.5f, seconds);
        OnReadyBegin?.Invoke(wait);
        yield return new WaitForSecondsRealtime(wait);

        // 5) 실제 게임 재시작
        StartGame();        // 내부에서 map.Begin(), spawner.Begin(), SetState(Playing)

        startFlowCo = null;
    }
    public void ReplayViaMainThenReady(float seconds = 1.2f)
    {
        // ▶ GameOver → 메인으로 복귀
        GoToMainMenu();

        // ▶ 곧바로 Ready → Playing으로 전환
        if (startFlowCo != null) StopCoroutine(startFlowCo);
        startFlowCo = StartCoroutine(StartFlow(seconds));
    }
}
