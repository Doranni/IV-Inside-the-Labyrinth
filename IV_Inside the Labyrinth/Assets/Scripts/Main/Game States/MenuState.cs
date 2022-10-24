using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuState : IState
{
    [SerializeField]
    private GameObject menuScreen;
    [SerializeField] 
    private Button settingsButton, exitButton;

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
