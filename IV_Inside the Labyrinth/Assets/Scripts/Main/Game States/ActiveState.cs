using UnityEngine;

public class GameActiveState : IGameState
{
    private readonly GameObject healthAndSanityPanel;

    public GameActiveState(GameObject healthAndSanityPanel)
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
        GameManager.instance.StateMachine.TransitionTo(GameManager.instance.StateMachine.menuState);
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
