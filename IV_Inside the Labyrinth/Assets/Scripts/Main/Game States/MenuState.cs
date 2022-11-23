using UnityEngine;

public class GameMenuState : IGameState
{
    public GameMenuState() { }

    public void Enter()
    {
        UpdatePause();
        Cursor.visible = true;
    }

    public void MenuPerformed()
    {
        GameManager.instance.StateMachine.TransitionTo(GameManager.instance.StateMachine.activeState);
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
