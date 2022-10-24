using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsState : IState
{
    [SerializeField]
    private GameObject settingsScreen;

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
