using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField, Range(0.5f, 3f)] float readySeconds = 1.5f;
    public void OnClickStart()
    {
        Debug.Log("[UI] Start button clicked"); // 눌림 확인
        GameManager.Instance.StartWithReady(readySeconds);
    }
}
