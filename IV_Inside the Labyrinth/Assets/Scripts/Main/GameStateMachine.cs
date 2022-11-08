using System;
using UnityEngine;

[Serializable]
public class GameStateMachine
{
    public IGameState CurrentState { get; private set; }

    public GameActiveState activeState;
    public GameMenuState menuState;
    public GameSettingsState settingsState;
    public GameExitState exitState;

    public GameStateMachine(GameObject healthAndSanityPanel, GameObject menuScreen, 
        GameObject settingsScreen, GameObject exitScreen)
    {
        activeState = new GameActiveState(healthAndSanityPanel);
        menuState = new GameMenuState(menuScreen);
        settingsState = new GameSettingsState(settingsScreen);
        exitState = new GameExitState(exitScreen);
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
