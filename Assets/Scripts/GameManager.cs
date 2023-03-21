using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Singelton
    private static GameManager _instance;
    public static GameManager Instance { 
        get 
        { 
            if (_instance != null)
            {
                return _instance;
            }

            GameManager newInstance = (Instantiate(Resources.Load("Managers/GameManager") as GameObject)).GetComponent<GameManager>();
            _instance = newInstance;
            _instance.Init();
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
    private void Update()
    {
        if (!_init) return;

        UpdateBackground();
    }

    private void UpdateBackground()
    {
        if (updateBackground != null && PlayerHandler != null) updateBackground(PlayerHandler.Position.x);

    }

}
