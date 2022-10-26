using System;
using UnityEngine;

[Serializable]
public class GameStateMachine
{
    public IGameState CurrentState { get; private set; }

    public ActiveState activeState;
    public MenuState menuState;
    public SettingsState settingsState;
    public ExitState exitState;

    public GameStateMachine(GameObject healthAndSanityPanel, GameObject menuScreen, 
        GameObject settingsScreen, GameObject exitScreen)
    {
        activeState = new ActiveState(healthAndSanityPanel);
        menuState = new MenuState(menuScreen);
        settingsState = new SettingsState(settingsScreen);
        exitState = new ExitState(exitScreen);
    }

    public void Initialize(IGameState startingState)
    {
        activeState.Exit();
        menuState.Exit();
        settingsState.Exit();
        exitState.Exit();
        CurrentState = startingState;
        startingState.Enter();
    }

    public void TransitionTo(IGameState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
    }

    public void MenuPerformed()
    {
        if (CurrentState != null)
        {
            CurrentState.MenuPerformed();
        }
    }

    public void UpdatePause()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdatePause();
        }
    }
}
