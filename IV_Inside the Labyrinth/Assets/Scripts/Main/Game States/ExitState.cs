using UnityEngine;

public class GameExitState : IGameState
{
    private readonly GameObject exitScreen;

    public GameExitState(GameObject exitScreen)
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
        GameManager.instance.StateMachine.TransitionTo(GameManager.instance.StateMachine.menuState);
    }

    public void UpdatePause()
    {
        if (Preferences.IsPausedWhileInMenu)
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
