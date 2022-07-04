using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen, settingsScreen;
    [SerializeField] private Button settingsButton;

    private GameManagerInput gmInput;
    private GameStatus gameStatus;
    
    private void Awake()
    {
        gmInput = new GameManagerInput();
        gmInput.Game.Menu.performed += Menu_performed;
        settingsButton.onClick.AddListener(OpenSettingsScreen);
    }

    private void Menu_performed(InputAction.CallbackContext obj)
    {
        switch (gameStatus)
        {
            case GameStatus.active:
                {
                    if (Preferences.isPausedWhileInMenu)
                    {
                        Time.timeScale = 0;
                    }
                    menuScreen.SetActive(true);
                    gameStatus = GameStatus.onMenu;
                    Cursor.visible = true;
                    break;
                }
            case GameStatus.onMenu:
                {
                    menuScreen.SetActive(false);
                    gameStatus = GameStatus.active;
                    Cursor.visible = false;
                    Time.timeScale = 1;
                    break;
                }
            case GameStatus.onSettings:
                {
                    menuScreen.SetActive(true);
                    settingsScreen.SetActive(false);
                    gameStatus = GameStatus.onMenu;
                    break;
                }
        }
    }

    private void OpenSettingsScreen()
    {
        menuScreen.SetActive(false);
        settingsScreen.SetActive(true);
        gameStatus = GameStatus.onSettings;
    }

    void Start()
    {
        gameStatus = GameStatus.active;
        menuScreen.SetActive(false);
        settingsScreen.SetActive(false);
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        gmInput.Enable(); 
    }

    private void OnDisable()
    {
        gmInput.Disable();
    }

    enum GameStatus
    {
        active,
        onMenu,
        onSettings
    }


}
