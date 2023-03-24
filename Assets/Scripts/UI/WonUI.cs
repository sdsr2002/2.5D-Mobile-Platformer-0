using UnityEngine;
using UnityEngine.UI;

public class WonUI : MonoBehaviour
{
    public Image Parent;
    private EmptyDelegate action;
    private void Awake()
    {
        action = OnWin;
        GameManager.Instance.AddWinEvent(ref action);
        Parent.gameObject.SetActive(false);
    }

    private void OnWin()
    {
        if (!Parent) return;
        Parent.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (GameManager.Exist)
        GameManager.Instance.RemoveWinEvent(ref action);
    }
}