using UnityEngine;

public abstract class GroundedState 
{
    protected readonly MovementController plMovement;
    protected readonly CharacterController characterController;
    private readonly Animator animator;

    public GroundedState(MovementController plMovement, CharacterController characterController, Animator animator)
    {
        this.plMovement = plMovement;
        this.characterController = characterController;
        this.animator = animator;
    }

    public virtual void Update()
    {
        if (!characterController.isGrounded)
        {
            (Vector3 p1, Vector3 p2) = plMovement.GetColliderPoints();

            // Falling with animation (from high distance)
            if (!Physics.CapsuleCast(p1, p2, characterController.radius,
                Vector3.down, 1.5f, plMovement.FloorLM))
            {
                plMovement.StateMachine.TransitionTo(plMovement.StateMachine.fallingState);
            }
            else
            {
                characterController.Move(plMovement.moveDirectionGlobal * Time.deltaTime);
                plMovement.moveDirectionGlobal.y += plMovement.MoveGravity * Time.deltaTime;
            }
        }
        else
        {
            HandleRotation();
            plMovement.SetAcceleration();
            plMovement.SetMoveDirection();
            HandleMovement();
        }
    }

    protected virtual void HandleRotation() { }

    private void HandleMovement()
    {
        plMovement.moveDirectionGlobal.y = plMovement.MoveGravity;
        plMovement.moveDirectionGlobal *= plMovement.MovementSpeed * plMovement.Acceleration 
            * plMovement.SpeedEffectMultiplier * Time.deltaTime;
        characterController.Move(plMovement.moveDirectionGlobal);
    }

    public void JumpUp_pressed()
    {
        if (characterController.isGrounded)
        {
            plMovement.SetMoveDirection();
            float rotAngle = 0;
            if (plMovement.moveDirectionLocal.magnitude > 0)
            {
                plMovement.moveDirectionGlobal = plMovement.moveDirectionGlobal * plMovement.JumpForwardForce 
                    + Vector3.up * plMovement.JumpUpForce;

                // Jumping Forward
                if (plMovement.moveDirectionLocal.z >= 0)
                {
                    if (Preferences.PlRotStyle == Preferences.PlayerRotationStyle.withMouse)
                    {
                        rotAngle = Vector3.SignedAngle(Vector3.forward, plMovement.moveDirectionLocal, Vector3.up);
                    }
                    animator.SetFloat(plMovement.AnimHash_FallingVelocity_Float, 1);
                }
                // Jumping Backward
                else
                {
                    if (Preferences.PlRotStyle == Preferences.PlayerRotationStyle.withMouse)
                    {
                        rotAngle = Vector3.SignedAngle(Vector3.back, plMovement.moveDirectionLocal, Vector3.up);
                    }
                    animator.SetFloat(plMovement.AnimHash_FallingVelocity_Float, -1);
                }
            }
            // Jumping Up
            else
            {
                plMovement.moveDirectionGlobal = Vector3.up * plMovement.JumpUpForce;
                animator.SetFloat(plMovement.AnimHash_FallingVelocity_Float, 0);
            }
            animator.SetFloat(plMovement.AnimHash_Angle_Float, rotAngle);
            animator.SetTrigger(plMovement.AnimHash_Jump_Trigger);

            plMovement.StateMachine.TransitionTo(plMovement.StateMachine.underAnimationState);
        }
    }
}
