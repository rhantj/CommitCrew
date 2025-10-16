using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private Transform[] backgrounds;
    private float backgroundWidth;

    void Start()
    {
        // ��� �ʺ� �� ���� �ڵ� ���
        if (backgrounds.Length > 0)
        {
            SpriteRenderer spriteRenderer = backgrounds[0].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                backgroundWidth = spriteRenderer.bounds.size.x * backgrounds[0].localScale.x; // ������ �ݿ�
            }
            else
            {
                Debug.LogError("��׶��尡 ����");
            }
        }
    }

    void Update()
    {
        ScrollBackgrounds();
    }

    void ScrollBackgrounds()
    {
        foreach (Transform bg in backgrounds)
        {
            // �������� �̵�
            bg.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // ȭ�� ������ ������ �� �ڷ� ���ġ
            if (bg.position.x <= -backgroundWidth)
            {
                float maxX = -Mathf.Infinity;
                foreach (Transform otherBg in backgrounds)
                {
                    if (otherBg.position.x > maxX)
                        maxX = otherBg.position.x;
                }
                bg.position = new Vector3(Mathf.Round(maxX + backgroundWidth), bg.position.y, bg.position.z);
            }
        }
    }
}
