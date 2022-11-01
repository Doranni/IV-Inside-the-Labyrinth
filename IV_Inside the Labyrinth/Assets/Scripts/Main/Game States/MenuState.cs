using UnityEngine;

public class GameMenuState : IGameState
{
    private readonly GameObject menuScreen;

    public GameMenuState(GameObject menuScreen)
    {
        this.menuScreen = menuScreen;
    }

    public void Enter()
    {
        UpdatePause();
        menuScreen.SetActive(true);
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

    public void Exit()
    {
        menuScreen.SetActive(false);
    }
}
