using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitState : IState
{
    [SerializeField]
    private GameObject exitScreen;

    public void Enter()
    {
        if (Preferences.isPausedWhileInMenu)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
