using UnityEngine;
using UnityEngine.UI;

public class LostUI : MonoBehaviour
{
    public Image Parent;
    private EmptyDelegate action;
    private void Awake()
    {
        action = OnLost;
        GameManager.Instance.AddLostEvent(ref action);
        Parent.gameObject.SetActive(false);
    }

    private void OnLost()
    {
        if (!Parent) return;
        Parent.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (GameManager.Exist)
            GameManager.Instance.RemoveLostEvent(ref action);
    }
}
