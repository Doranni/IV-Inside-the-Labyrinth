using UnityEngine;

public class StillState : GroundedState, IMovementState
{
    public StillState(MovementController plMovement, CharacterController characterController, Animator animator) 
        : base (plMovement, characterController, animator)
    {

    }
    public void Enter() { }

    public void Exit() { }

    public override void Update() 
    {
        base.Update();
    }

    protected override void HandleRotation()
    {
        switch (Preferences.PlRotStyle)
        {
            case Preferences.PlayerRotationStyle.withMouse:
                {
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    plMovement.transform.Rotate(Vector3.up, Mathf.Lerp(0,
                        plMovement.RotationKeyboardInput * plMovement.CurrentRotationSpeed 
                        * plMovement.RotationAcceleration, Time.deltaTime), Space.Self);
                    break;
                }
        }
    }

    public void MoveForward_started()
    {
        plMovement.StateMachine.TransitionTo(plMovement.StateMachine.movingState);
    }

    public void MoveAside_started()
    {
        if (Preferences.PlRotStyle == Preferences.PlayerRotationStyle.withMouse)
        {
            plMovement.StateMachine.TransitionTo(plMovement.StateMachine.movingState);
        }

    }
}
