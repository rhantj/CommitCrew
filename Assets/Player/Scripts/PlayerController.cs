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
    }
    void Update()
    {
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
        
        if (!isStart) return;
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
