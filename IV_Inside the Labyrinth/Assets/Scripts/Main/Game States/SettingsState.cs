using UnityEngine;

public class SettingsState : IGameState
{
    private readonly GameObject settingsScreen;

    public SettingsState(GameObject settingsScreen)
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
        settingsScreen.SetActive(false);
        TooltipController.HideTooltip();
    }
}
