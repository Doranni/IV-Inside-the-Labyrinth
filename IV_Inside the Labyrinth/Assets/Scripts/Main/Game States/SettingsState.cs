using UnityEngine;

public class GameSettingsState : IGameState
{
    public GameSettingsState() { }

    public void Enter()
    {
        Cursor.visible = true;
        UpdatePause();
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
