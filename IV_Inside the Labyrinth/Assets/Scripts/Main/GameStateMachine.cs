using System;

[Serializable]
public class GameStateMachine
{
    public IState CurrentState { get; private set; }

    public ActiveState activeState;
    public MenuState menuState;
    public SettingsState settingsState;
    public ExitState exitState;

    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }

    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
    }

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}
