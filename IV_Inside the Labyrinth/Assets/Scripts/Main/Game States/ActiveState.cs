using UnityEngine;

public class GameActiveState : IGameState
{
    public GameActiveState() { }

    public void Enter()
    {
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

    public void Exit() { }
}
