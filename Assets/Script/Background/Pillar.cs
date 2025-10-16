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
        // BoxCollider2D�� ũ��� �������� �������� ����
        float colliderHeight = Random.Range(minColliderSize, maxColliderSize);
        float colliderOffset = Random.Range(minOffset, maxOffset);

        scoreCollider.size = new Vector2(scoreCollider.size.x, colliderHeight);
        scoreCollider.offset = new Vector2(scoreCollider.offset.x, colliderOffset);

        // ���ھ� �ݶ��̴��� ���� �� ��ġ
        float colliderTop = colliderOffset + colliderHeight / 2f;
        // ���ھ� �ݶ��̴��� �Ʒ��� �� ��ġ
        float colliderBottom = colliderOffset - colliderHeight / 2f;

        // TopPillar ��ġ ����
        // TopPillar�� "�Ʒ��� ��"�� colliderTop�� ������ ����
        // SpriteRenderer�� ���� ���̸� ������
        SpriteRenderer topRenderer = topPillar.GetComponent<SpriteRenderer>();
        float topPillarHeight = topRenderer != null ? topRenderer.bounds.size.y : topPillar.localScale.y;
        topPillar.localPosition = new Vector3(0, colliderTop + topPillarHeight / 2f, 0);

        // BottomPillar ��ġ ����
        // BottomPillar�� "���� ��"�� colliderBottom�� ������ ����
        SpriteRenderer bottomRenderer = bottomPillar.GetComponent<SpriteRenderer>();
        float bottomPillarHeight = bottomRenderer != null ? bottomRenderer.bounds.size.y : bottomPillar.localScale.y;
        bottomPillar.localPosition = new Vector3(0, colliderBottom - bottomPillarHeight / 2f, 0);
    }

    private void OnEnable()
    {
        // ���� �� �ݶ��̴� �ٽ� Ȱ��ȭ
        if (scoreCollider != null)
            scoreCollider.enabled = true;
    }


    // ����� ��ġ�� �ݶ��̴� ��ġ ǥ��
    private void OnDrawGizmos()
    {
        if (scoreCollider != null)
        {
            float colliderHeight = scoreCollider.size.y;
            float colliderOffset = scoreCollider.offset.y;

            float colliderTop = colliderOffset + colliderHeight / 2f;
            float colliderBottom = colliderOffset - colliderHeight / 2f;

            // ���� �� (������)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, colliderTop, 0), 0.1f);

            // �Ʒ��� �� (�Ķ���)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, colliderBottom, 0), 0.1f);
        }
    }
}