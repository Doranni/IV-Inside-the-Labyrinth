using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public GameStateMachine StateMachine { get; private set; }

    [SerializeField] private GameObject menuScreen, settingsScreen, exitScreen;
    [SerializeField] private Button settingsButton, exitButton, quitButton, stayButton;

    public static readonly string playerTag = "Player", mainCameraTag = "MainCamera", trapTag = "Trap", groundTag = "Ground", 
        torchTag = "Torch", sanityLight = "Sanity Light";

    private static int lastId = 0;

    public override void Awake()
    {
        base.Awake();
        StateMachine = new GameStateMachine(menuScreen, settingsScreen, exitScreen);
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
