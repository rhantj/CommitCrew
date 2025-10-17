using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] public float jumpForce = 5f;

    [Header("State Flags (legacy)")]
    [SerializeField] public bool isDie = false;   // 충돌 시 true
    [SerializeField] public bool isStart = false; // 게임 시작 전/후 구분

    [Header("Spawn")]
    [SerializeField] public Transform spawnPoint; // ▶ GameManager가 넘겨줌(없으면 startPos 사용)

    private Rigidbody2D playerRb;
    private Animator playerAnim;
    private Vector3 startPos; // Awake 시점의 초기 위치(스폰포인트 없을 때 fallback)

    SoundManager soundManager;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        startPos = transform.position;

        // 게임 시작 전에는 중력 비활성화 + 속도 0
        playerRb.gravityScale = 0f;
        playerRb.velocity = Vector2.zero;
        playerRb.simulated = true; // 물리 시뮬 ON
    }

    private void Start()
    {
        soundManager = SoundManager.Instance;
    }

    void Update()
    {
        // 시작 전/죽은 상태면 입력 무시 + 고정
        if (!isStart || isDie)
        {
            playerRb.gravityScale = 0f;
            playerRb.velocity = Vector2.zero;
            return;
        }

        // 게임 시작 시 중력 활성화
        playerRb.gravityScale = 2f;

        // 클릭 입력으로 점프
        if (Input.GetMouseButtonDown(0))
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
            if (playerAnim) playerAnim.enabled = true;
            transform.rotation = Quaternion.Euler(0, 0, 30);

            soundManager.PlaySFXSound("sfx_wing");
        }

        // 하강 시 살짝 회전(최소 -60도까지)
        float currentZRotation = transform.eulerAngles.z;
        if (currentZRotation > 180f) currentZRotation -= 360f; // -180~180으로 변환
        if (currentZRotation > -60f)
            transform.Rotate(0, 0, -0.3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pipe") || collision.gameObject.CompareTag("Ground"))
        {
            isDie = true;
            if (playerAnim) playerAnim.enabled = false;
            soundManager.PlaySFXSound("sfx_die");
            // 바로 게임오버 처리까지 연결 (원하면 주석 해제)
            if (GameManager.Instance) GameManager.Instance.GameOver();

            Debug.Log("Game Over");
        }
    }

    // =============== 외부에서 호출(메인/레디/시작) ===============

    /// <summary>메인/준비 화면에서 ‘제자리 고정’ 상태로 만들기</summary>
    public void PrepareIdle()
    {
        // 초기화 순서 이슈 방지용 null 방어
        if (!playerRb) playerRb = GetComponent<Rigidbody2D>();
        if (!playerAnim) playerAnim = GetComponent<Animator>();

        gameObject.SetActive(true); // 혹시 꺼져 있으면 켜기

        // 스폰포인트가 있으면 그 위치, 없으면 Awake의 startPos
        transform.position = spawnPoint ? spawnPoint.position : startPos;
        transform.rotation = Quaternion.identity;

        isDie = false;
        isStart = false;

        playerRb.velocity = Vector2.zero;
        playerRb.gravityScale = 0f;
        playerRb.simulated = true; // 물리 켜두기(중력만 0)
    }

    /// <summary>게임 시작 직전에 완전 초기화</summary>
    public void ResetForGame()
    {
        if (!playerRb) playerRb = GetComponent<Rigidbody2D>();
        if (!playerAnim) playerAnim = GetComponent<Animator>();

        gameObject.SetActive(true);

        transform.position = spawnPoint ? spawnPoint.position : startPos;
        transform.rotation = Quaternion.identity;

        isDie = false;
        isStart = true;

        playerRb.velocity = Vector2.zero;
        playerRb.gravityScale = 2f;   // 시작 시 중력 ON
        playerRb.simulated = true;
        if (playerAnim) playerAnim.enabled = true;
    }

    // 에디터에서 실수 방지(컴포넌트 자동 참조 + Z=0 유지)
    void OnValidate()
    {
        if (!playerRb) playerRb = GetComponent<Rigidbody2D>();
        if (!playerAnim) playerAnim = GetComponent<Animator>();
        if (Mathf.Abs(transform.position.z) > 0.0001f)
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }
    // 트리거 통과시 점수증가
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 파이프 사이의 트리거 통과
        if (other.CompareTag("Score"))
        {
            GameManager.Instance?.AddScore(10);
        }
    }
}
