using UnityEngine;

public class MenuState : IGameState
{
    private readonly GameObject menuScreen;

    public MenuState(GameObject menuScreen)
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
        GameManager.instance.gameStateMachine.TransitionTo(GameManager.instance.gameStateMachine.activeState);
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
        menuScreen.SetActive(false);
    }
}
