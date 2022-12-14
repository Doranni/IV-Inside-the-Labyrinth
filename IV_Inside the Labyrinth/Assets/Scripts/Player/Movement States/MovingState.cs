using UnityEngine;

public class MovingState : GroundedState, IMovementState
{
    private readonly CameraController cameraController;
    private readonly Transform mainCamera;

    private float plCamAngle;

    public MovingState(MovementController plMovement, CharacterController characterController, 
        Animator animator, CameraController cameraController) 
        : base (plMovement, characterController, animator)
    {
        this.cameraController = cameraController;
        mainCamera = Camera.main.transform;
    }
    public void Enter() 
    {
        plMovement.RunningAudioStart();
    }
    public void Exit() 
    {
        plMovement.RunningAudioFinish();
    }

    public override void Update()
    {
        base.Update();
    }

    private void UpdateCameraAngle()
    {
        plCamAngle = Vector3.SignedAngle(plMovement.transform.forward, mainCamera.forward, Vector3.up);
    }

    protected override void HandleRotation()
    {
        switch (Preferences.PlRotStyle)
        {
            case Preferences.PlayerRotationStyle.withMouse:
                {
                    if (Preferences.CamRotStyle == Preferences.CameraRotationStyle.followPlayer
                        && cameraController.StateMachine.CurrentState.Equals(cameraController.StateMachine.thirdViewState))
                    {
                        UpdateCameraAngle();
                        float angleClamped = Mathf.Clamp(plCamAngle, -plMovement.PlCamMinAngleForRotation,
                            plMovement.PlCamMinAngleForRotation);
                        if (angleClamped == -plMovement.PlCamMinAngleForRotation 
                            || angleClamped == plMovement.PlCamMinAngleForRotation)
                        {
                            plMovement.transform.Rotate(Vector3.up, Mathf.Lerp(0,
                            angleClamped / plMovement.PlCamMinAngleForRotation * plMovement.CurrentRotationSpeed 
                            * plMovement.RotationAcceleration,
                            Time.deltaTime), Space.Self);
                        }
                    }
                    else
                    {
                        plMovement.transform.Rotate(Vector3.up, Mathf.Lerp(0,
                            plMovement.RotationMouseInput * plMovement.CurrentRotationSpeed * plMovement.RotationAcceleration,
                            Time.deltaTime), Space.Self);
                    }
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    plMovement.transform.Rotate(Vector3.up, Mathf.Lerp(0, plMovement.RotationKeyboardInput 
                        * plMovement.CurrentRotationSpeed * plMovement.RotationAcceleration,
                        Time.deltaTime), Space.Self);
                    break;
                }
        }
    }

    public void MoveForward_canceled() 
    {
        if (Preferences.PlRotStyle == Preferences.PlayerRotationStyle.withMouse) {
            if (plMovement.RotationKeyboardInput == 0)
            {
                plMovement.StateMachine.TransitionTo(plMovement.StateMachine.stillState);
            }
        }
        else
        {
            plMovement.StateMachine.TransitionTo(plMovement.StateMachine.stillState);
        }
        
    }

    public void MoveAside_canceled()
    {
        if (Preferences.PlRotStyle == Preferences.PlayerRotationStyle.withMouse
            && plMovement.MovementForwardInput == 0)
        {
            plMovement.StateMachine.TransitionTo(plMovement.StateMachine.stillState);
        }
    }

    public override string ToString()
    {
        return "Moving";
    }
}
