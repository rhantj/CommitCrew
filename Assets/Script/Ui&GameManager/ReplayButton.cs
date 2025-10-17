using UnityEngine;

public class ReplayButton : MonoBehaviour
{
    [SerializeField, Range(0.5f, 3f)] float readySeconds = 1.2f;

    public void OnClickReplay()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.ReplayViaMainThenReady(readySeconds);
    }
}
