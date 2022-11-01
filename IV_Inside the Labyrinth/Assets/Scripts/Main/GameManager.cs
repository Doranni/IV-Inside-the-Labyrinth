using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    public GameStateMachine StateMachine { get; private set; }

    [SerializeField] private GameObject menuScreen, settingsScreen, exitScreen, healthAndSanityPanel;
    [SerializeField] private Button settingsButton, exitButton, quitButton, stayButton;
    [SerializeField] private TextMeshProUGUI healthValueTmp, sanityValueTmp, effectsTmp;

    
    public static string playerTag = "Player", trapTag = "Trap", groundTag = "Ground", 
        torchTag = "Torch", sanityLight = "Sanity Light";

    private static int lastId = 0;

    public override void Awake()
    {
        base.Awake();
        StateMachine = new GameStateMachine(healthAndSanityPanel, menuScreen, settingsScreen, exitScreen);
        InputManager.instance.OnMenu_performed += Menu_performed;
        Preferences.OnPauseBehaviorChanged += UpdatePause;
        settingsButton.onClick.AddListener(OpenSettingsScreen);
        exitButton.onClick.AddListener(OpenExitScreen);
        quitButton.onClick.AddListener(QuitGame);
        stayButton.onClick.AddListener(StayInGame);
    }

    void Start()
    {
        StateMachine.Initialize(StateMachine.activeState);
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Menu_performed(InputAction.CallbackContext obj)
    {
        StateMachine.MenuPerformed();
    }

    public void UpdatePause()
    {
        StateMachine.UpdatePause();
    }

    private void OpenSettingsScreen()
    {
        StateMachine.TransitionTo(StateMachine.settingsState);
    }

    private void OpenExitScreen()
    {
        StateMachine.TransitionTo(StateMachine.exitState);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void StayInGame()
    {
        StateMachine.TransitionTo(StateMachine.menuState);
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

    private void OnDestroy()
    {
        InputManager.instance.OnMenu_performed -= Menu_performed;
        Preferences.OnPauseBehaviorChanged -= UpdatePause;
    }
}
