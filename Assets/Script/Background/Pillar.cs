using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField] private Transform topPillar;
    [SerializeField] private Transform bottomPillar;
    [SerializeField] private BoxCollider2D scoreCollider;

    public void SetupPillars(float minColliderSize, float maxColliderSize, float minOffset, float maxOffset)
    {
        // BoxCollider2D의 크기와 오프셋을 랜덤으로 설정
        float colliderHeight = Random.Range(minColliderSize, maxColliderSize);
        float colliderOffset = Random.Range(minOffset, maxOffset);

        scoreCollider.size = new Vector2(scoreCollider.size.x, colliderHeight);
        scoreCollider.offset = new Vector2(scoreCollider.offset.x, colliderOffset);

        // 스코어 콜라이더의 위쪽 끝 위치
        float colliderTop = colliderOffset + colliderHeight / 2f;
        // 스코어 콜라이더의 아래쪽 끝 위치
        float colliderBottom = colliderOffset - colliderHeight / 2f;

        // TopPillar 위치 설정
        // TopPillar의 "아래쪽 끝"이 colliderTop에 오도록 설정
        // SpriteRenderer의 실제 높이를 가져옴
        SpriteRenderer topRenderer = topPillar.GetComponent<SpriteRenderer>();
        float topPillarHeight = topRenderer != null ? topRenderer.bounds.size.y : topPillar.localScale.y;
        topPillar.localPosition = new Vector3(0, colliderTop + topPillarHeight / 2f, 0);

        // BottomPillar 위치 설정
        // BottomPillar의 "위쪽 끝"이 colliderBottom에 오도록 설정
        SpriteRenderer bottomRenderer = bottomPillar.GetComponent<SpriteRenderer>();
        float bottomPillarHeight = bottomRenderer != null ? bottomRenderer.bounds.size.y : bottomPillar.localScale.y;
        bottomPillar.localPosition = new Vector3(0, colliderBottom - bottomPillarHeight / 2f, 0);
    }

    private void OnEnable()
    {
        // 재사용 시 콜라이더 다시 활성화
        if (scoreCollider != null)
            scoreCollider.enabled = true;
    }


    // 기즈모 위치로 콜라이더 위치 표기
    private void OnDrawGizmos()
    {
        if (scoreCollider != null)
        {
            float colliderHeight = scoreCollider.size.y;
            float colliderOffset = scoreCollider.offset.y;

            float colliderTop = colliderOffset + colliderHeight / 2f;
            float colliderBottom = colliderOffset - colliderHeight / 2f;

            // 위쪽 끝 (빨간색)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, colliderTop, 0), 0.1f);

            // 아래쪽 끝 (파란색)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, colliderBottom, 0), 0.1f);
        }
    }
}