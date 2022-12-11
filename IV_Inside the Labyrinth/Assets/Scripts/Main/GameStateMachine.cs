using System;

[Serializable]
public class GameStateMachine
{
    public IGameState CurrentState { get; private set; }

    public GameActiveState activeState;
    public GameMenuState menuState;
    public GameSettingsState settingsState;
    public GameExitState exitState;

    public event Action OnStateChanged;

    public GameStateMachine()
    {
        activeState = new GameActiveState();
        menuState = new GameMenuState();
        settingsState = new GameSettingsState();
        exitState = new GameExitState();
    }

    public void Initialize(IGameState startingState)
    {
        CurrentState = startingState;
    }

    public void TransitionTo(IGameState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
        OnStateChanged?.Invoke();
    }

    public void MenuPerformed()
    {
        CurrentState?.MenuPerformed();
    }

    public void UpdatePause()
    {
        CurrentState?.UpdatePause();
    }

    public void Start()
    {
        CurrentState?.Enter();
    }
}
