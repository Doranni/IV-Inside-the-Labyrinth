public interface IMovementState
{
    public void Enter() { }
    public void Exit() { }
    public void Update() { }
    public void JumpUp_pressed() { }
    public void MoveForward_started() { }
    public void MoveForward_canceled() { }
    public void MoveAside_started() { }
    public void MoveAside_canceled() { } 
}
