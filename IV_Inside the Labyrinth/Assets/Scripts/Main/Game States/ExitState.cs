using UnityEngine;

public class GameExitState : IGameState
{
    public GameExitState() { }

    public void Enter()
    {
        UpdatePause();
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

    public void Exit() { }
}
