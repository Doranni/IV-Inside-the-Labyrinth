using UnityEngine;

public class JumpingUpState : IMovementState
{
    private readonly MovementController plMovement;
    private readonly CharacterController characterController;

    public JumpingUpState(MovementController plMovement, CharacterController characterController)
    {
        this.plMovement = plMovement;
        this.characterController = characterController;
    }
    public void Enter() { }
    public void Exit() { }
    public void Update() 
    {
        characterController.Move(plMovement.SpeedEffectMultiplier * Time.deltaTime * plMovement.moveDirectionGlobal);
        plMovement.moveDirectionGlobal.y += plMovement.MoveGravity * Time.deltaTime;
    }
    public void JumpUp_pressed() { }
}
