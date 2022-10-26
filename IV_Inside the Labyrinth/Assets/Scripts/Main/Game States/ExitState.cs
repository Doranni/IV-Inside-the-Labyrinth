using UnityEngine;

public class ExitState : IGameState
{
    private readonly GameObject exitScreen;

    public ExitState(GameObject exitScreen)
    {
        this.exitScreen = exitScreen;
    }

    public void Enter()
    {
        UpdatePause();
        exitScreen.SetActive(true);
        Cursor.visible = true;
    }

    public void MenuPerformed()
    {
        GameManager.instance.gameStateMachine.TransitionTo(GameManager.instance.gameStateMachine.menuState);
    }

    public void UpdatePause()
    {
        if (Preferences.isPausedWhileInMenu)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Exit()
    {
        exitScreen.SetActive(false);
    }
}
