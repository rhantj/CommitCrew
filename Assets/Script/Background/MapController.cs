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
        // 배경 너비 및 높이 자동 계산
        if (backgrounds.Length > 0)
        {
            SpriteRenderer spriteRenderer = backgrounds[0].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                backgroundWidth = spriteRenderer.bounds.size.x * backgrounds[0].localScale.x; // 스케일 반영
            }
            else
            {
                Debug.LogError("백그라운드가 없음");
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
            // 왼쪽으로 이동
            bg.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // 화면 밖으로 나가면 맨 뒤로 재배치
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
