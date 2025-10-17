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
            if (sr != null) backgroundWidth = sr.bounds.size.x;
            else Debug.LogError("MapController: SpriteRenderer가 없습니다.");
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
                    if (other.position.x > maxX) maxX = other.position.x;

                bg.position = new Vector3(maxX + backgroundWidth, bg.position.y, bg.position.z);
            }
        }
    }

    // GameManager에서 호출
    public void Begin() => running = true;
    public void Stop() => running = false;
}
