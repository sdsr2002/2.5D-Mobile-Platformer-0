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
        GameManager.Instance.lostUI = this;
        gameObject.SetActive(false);
    }
    private void OnLost()
    {
        Parent.gameObject.SetActive(true);
    }
}
