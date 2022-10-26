using UnityEngine;

public class ActiveState : IGameState
{
    private readonly GameObject healthAndSanityPanel;

    public ActiveState(GameObject healthAndSanityPanel)
    {
        this.healthAndSanityPanel = healthAndSanityPanel;
    }

    public void Enter()
    {
        healthAndSanityPanel.SetActive(true);
        Cursor.visible = false;
        UpdatePause();
    }

    public void MenuPerformed()
    {
        GameManager.instance.gameStateMachine.TransitionTo(GameManager.instance.gameStateMachine.menuState);
    }

    public void UpdatePause()
    {
        Time.timeScale = 1;
    }

    public void Exit()
    {
        healthAndSanityPanel.SetActive(false);
    }
}
