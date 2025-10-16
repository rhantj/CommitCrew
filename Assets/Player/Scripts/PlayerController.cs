using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public bool isDie = false;
    public bool isStart = false;
    private Rigidbody2D playerRb;
    private Animator playerAnim;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        
        // 게임 시작 전에는 중력 비활성화
        if (!isStart)
        {
            playerRb.gravityScale = 0;
        }
    }
    void Update()
    {
        // isStart가 false일 때는 중력 비활성화하여 위치 고정
        if (!isStart)
        {
            playerRb.gravityScale = 0;
            playerRb.velocity = Vector2.zero; // 속도도 0으로 설정
            return;
        }
        else
        {
            // 게임 시작 시 중력 활성화
            playerRb.gravityScale = 2;
        }

        if (Input.GetMouseButtonDown(0) && !isDie)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
            playerAnim.enabled = true;
            transform.rotation = Quaternion.Euler(0, 0, 30);
        }
        
        // Z축 회전값을 -20도로 제한
        float currentZRotation = transform.eulerAngles.z;
        if (currentZRotation > 180) currentZRotation -= 360; // 각도를 -180~180 범위로 변환
        
        if (currentZRotation > -60)
        {
            transform.Rotate(0, 0, -0.3f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Pipe")
        || collision.gameObject.CompareTag("Ground"))
        {
            isDie = true;
            playerAnim.enabled = false;

            Debug.Log("Game Over");
        }
    }


}
