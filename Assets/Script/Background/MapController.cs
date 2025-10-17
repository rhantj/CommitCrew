using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private Transform[] backgrounds;
    private float backgroundWidth;
    private bool running = false;

    void Start()
    {
        if (backgrounds.Length > 0)
        {
            var sr = backgrounds[0].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 배경 너비를 정확히 계산
                backgroundWidth = sr.bounds.size.x * backgrounds[0].localScale.x;
            }
            else
            {
                Debug.LogError("Error : MapControll SrptieRender Missing");
            }
        }
    }

    void Update()
    {
        if (!running) return;
        ScrollBackgrounds();
    }

    void ScrollBackgrounds()
    {
        foreach (Transform bg in backgrounds)
        {
            bg.position += Vector3.left * scrollSpeed * Time.deltaTime;

            if (bg.position.x <= -backgroundWidth)
            {
                float maxX = float.NegativeInfinity;
                foreach (Transform other in backgrounds)
                {
                    if (other.position.x > maxX)
                        maxX = other.position.x;
                }

                // 위치를 정수로 고정하여 부동소수점 오차 방지
                bg.position = new Vector3(Mathf.Round(maxX + backgroundWidth), bg.position.y, bg.position.z);
            }
        }
    }

    // GameManager에서 호출
    public void Begin() => running = true;
    public void Stop() => running = false;
}
