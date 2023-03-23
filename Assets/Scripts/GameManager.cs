using System;
using UnityEngine;

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

}
