using System;
using UnityEngine;

public delegate void EmptyDelegate();

public class GameManager : MonoBehaviour
{

    #region Singelton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = Instantiate(Resources.Load<GameObject>("Managers/GameManager")).GetComponent<GameManager>();
                _instance.Init();
            }

            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    private bool _init = false;
    [ContextMenu("Initilize")]
    public void Init()
    {
        if (_init) return;
        _init = true;
        if (_instance != this)
        {
            this.enabled = false;
            return;
        }
        DontDestroyOnLoad(this);
    }
    #endregion

    public Action<float> updateBackground;
    public PlayerHandler PlayerHandler;
    private Camera _mainCamera => Camera.main;
    public static bool Exist => _instance;

    private EmptyDelegate OnWin;
    private EmptyDelegate OnLost;

    private void Awake()
    {
        if (Application.isMobilePlatform && Application.targetFrameRate != 60)
            Application.targetFrameRate = 60;
        //OnWin += WonUI;
        //OnLost += LostUI;
        OnWin += () => PlayerHandler.PlayIdleAnimation(4);
        OnWin += () => PlayerHandler.SetDead(true);
    }

    private void Update()
    {
        if (!_init) return;

        UpdateBackground();
    }

    private void UpdateBackground()
    {
        if (updateBackground != null && _mainCamera) updateBackground(_mainCamera.transform.position.x);
        return;
    }

    public static void Won()
    {
        Instance.OnWin();
    }

    public static void Lost()
    {
        Instance.OnLost();
    }

    public void AddWinEvent(ref EmptyDelegate action)
    {
        OnWin += action;
    }

    public void AddLostEvent(ref EmptyDelegate action)
    {
        OnLost += action;
    }

    public void RemoveWinEvent(ref EmptyDelegate action)
    {
        OnWin -= action;
    }

    public void RemoveLostEvent(ref EmptyDelegate action)
    {
        OnLost -= action;
    }
}
