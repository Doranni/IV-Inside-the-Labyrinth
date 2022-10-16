using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    enum GameStatus
    {
        active,
        onMenu,
        onSettings,
        onExit
    }

    [SerializeField]
    private GameObject menuScreen, settingsScreen, exitScreen, healthAndSanityPanel;
    [SerializeField] private Button settingsButton, exitButton, quitButton, stayButton;
    [SerializeField] private TextMeshProUGUI healthValueTmp, sanityValueTmp, effectsTmp;

    private InputManager inputManager;
    private GameStatus gameStatus;
    
    public static string playerTag = "Player", trapTag = "Trap", groundTag = "Ground", torchTag = "Torch", sanityLight = "Sanity Light";

    private static GameManager instance;
    private static int lastId = 0;

    private void Awake()
    {
        instance = this;
        inputManager = InputManager.instance;
        inputManager.OnMenu_performed += Menu_performed;
        settingsButton.onClick.AddListener(OpenSettingsScreen);
        exitButton.onClick.AddListener(OpenExitScreen);
        quitButton.onClick.AddListener(QuitGame);
        stayButton.onClick.AddListener(StayInGame);
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        gameStatus = GameStatus.active;
        menuScreen.SetActive(false);
        settingsScreen.SetActive(false);
        exitScreen.SetActive(false);
        healthAndSanityPanel.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
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
                    healthAndSanityPanel.SetActive(false);
                    gameStatus = GameStatus.onMenu;
                    Cursor.visible = true;
                    break;
                }
            case GameStatus.onMenu:
                {
                    menuScreen.SetActive(false);
                    healthAndSanityPanel.SetActive(true);
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
                    TooltipController.HideTooltip();
                    break;
                }
            case GameStatus.onExit:
                {
                    menuScreen.SetActive(true);
                    exitScreen.SetActive(false);
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

    private void OpenExitScreen()
    {
        menuScreen.SetActive(false);
        exitScreen.SetActive(true);
        gameStatus = GameStatus.onExit;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void StayInGame()
    {
        menuScreen.SetActive(true);
        exitScreen.SetActive(false);
        gameStatus = GameStatus.onMenu;
    }

    public static void UpdateHealth(float value, float maxValue)
    {
        instance.UpdateHealth_private(value, maxValue);
    }

    private void UpdateHealth_private(float value, float maxValue)
    {
        healthValueTmp.text = Mathf.Round(value) + "/" + maxValue;
    }

    public static void UpdateSanity(float currentSanity, float maxSanity)
    {
        instance.UpdateSanity_private(currentSanity, maxSanity);
    }

    private void UpdateSanity_private(float currentSanity, float maxSanity)
    {
        sanityValueTmp.text = Mathf.Round(currentSanity) + "/" + maxSanity;
    }

    public static void UpdateEffects(Dictionary<int, Effect> effects)
    {
        instance.UpdateEffects_private(effects);
    }
    private void UpdateEffects_private(Dictionary<int, Effect> effects)
    {
        effectsTmp.text = string.Empty;
        string res = string.Empty;
        
        foreach (KeyValuePair<int, Effect> effect in effects)
        {
            res += effect.Value.ToString() + "\n";
        }
        effectsTmp.SetText(res);
    }

    public static int NewId()
    {
        return lastId++;
    }
}
