using System;
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
        GameManager.Instance.wonUI = this;
        gameObject.SetActive(false);
    }
    private void OnWin()
    {
        Parent.gameObject.SetActive(true);
    }
}
