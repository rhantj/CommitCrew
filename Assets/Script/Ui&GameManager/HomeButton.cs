using UnityEngine;
public class HomeButton : MonoBehaviour
{
    public void OnClickHome()
    {
        GameManager.Instance.GoToMainMenu(); // �� ��������
    }
}
