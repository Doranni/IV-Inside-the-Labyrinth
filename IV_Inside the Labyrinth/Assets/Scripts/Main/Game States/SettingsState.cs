using UnityEngine;

public class GameSettingsState : IGameState
{
    private readonly GameObject settingsScreen;

    public GameSettingsState(GameObject settingsScreen)
    {
        this.settingsScreen = settingsScreen;
    }

    public void Enter()
    {
        UpdatePause();
        settingsScreen.SetActive(true);
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
        settingsScreen.SetActive(false);
        TooltipController.HideTooltip();
    }
}
