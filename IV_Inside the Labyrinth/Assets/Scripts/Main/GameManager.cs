using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public GameStateMachine StateMachine { get; private set; }

    [SerializeField] private GameObject settingsScreen;

    public static readonly string playerTag = "Player", mainCameraTag = "MainCamera", trapTag = "Trap", groundTag = "Ground", 
        torchTag = "Torch", sanityLight = "Sanity Light";

    private static int lastId = 0;

    public override void Awake()
    {
        base.Awake();
        StateMachine = new GameStateMachine(settingsScreen);
        InputManager.instance.OnMenu_performed += Menu_performed;
        Preferences.OnPauseBehaviorChanged += UpdatePause;
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

    public void OpenSettingsScreen()
    {
        StateMachine.TransitionTo(StateMachine.settingsState);
    }

    public void OpenExitScreen()
    {
        StateMachine.TransitionTo(StateMachine.exitState);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StayInGame()
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
