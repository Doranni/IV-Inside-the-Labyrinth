using UnityEngine;

public class FallingState : IMovementState
{
    private readonly MovementController plMovement;
    private readonly CharacterController characterController;
    private readonly Animator animator;

    public FallingState(MovementController plMovement, CharacterController characterController, Animator animator)
    {
        this.plMovement = plMovement;
        this.characterController = characterController;
        this.animator = animator;
    }

    public void Enter() 
    {
        plMovement.moveDirectionGlobal.y = plMovement.MoveGravity;
        float rotAngle = 0;
        if (plMovement.moveDirectionLocal.magnitude > 0)
        {
            // Falling Forward
            if (plMovement.moveDirectionLocal.z >= 0)
            {
                if (Preferences.PlRotStyle == Preferences.PlayerRotationStyle.withMouse)
                {
                    rotAngle = Vector3.SignedAngle(Vector3.forward, plMovement.moveDirectionLocal, Vector3.up);
                }
                animator.SetFloat(plMovement.AnimHash_FallingVelocity_Float, 1);
            }
            // Falling Backward
            else
            {
                if (Preferences.PlRotStyle == Preferences.PlayerRotationStyle.withMouse)
                {
                    rotAngle = Vector3.SignedAngle(Vector3.back, plMovement.moveDirectionLocal, Vector3.up);
                }
                animator.SetFloat(plMovement.AnimHash_FallingVelocity_Float, -1);
            }
        }
        // Falling Up
        else
        {
            animator.SetFloat(plMovement.AnimHash_FallingVelocity_Float, 0);
        }
        animator.SetFloat(plMovement.AnimHash_Angle_Float, rotAngle);
        animator.SetTrigger(plMovement.AnimHash_Falling_Trigger);
    }
    public void Exit() { }
    public void Update() 
    {
        if (characterController.isGrounded)
        {
            plMovement.StateMachine.TransitionTo(plMovement.StateMachine.underAnimationState);
            animator.SetTrigger(plMovement.AnimHash_Landing_Trigger);
        }
        else
        {
            characterController.Move(plMovement.moveDirectionGlobal * Time.deltaTime);
            plMovement.moveDirectionGlobal.y += plMovement.MoveGravity * Time.deltaTime;
        }
    }
    public void JumpUp_pressed() { }
}
