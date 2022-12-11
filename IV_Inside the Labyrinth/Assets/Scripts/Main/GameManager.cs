using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public GameStateMachine StateMachine { get; private set; }

    public static readonly string tag_player = "Player", tag_mainCamera = "MainCamera", 
        tag_trap = "Trap", tag_ground = "Ground", tag_torch = "Torch", tag_sanityLight = "Sanity Light";

    private static int lastId = 0;

    public override void Awake()
    {
        base.Awake();
        StateMachine = new GameStateMachine();
        StateMachine.Initialize(StateMachine.activeState);
    }

    void Start()
    {
        StateMachine.Start();
        InputManager.instance.OnMenu_performed += Menu_performed;
        Preferences.OnPauseBehaviorChanged += UpdatePause;
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
