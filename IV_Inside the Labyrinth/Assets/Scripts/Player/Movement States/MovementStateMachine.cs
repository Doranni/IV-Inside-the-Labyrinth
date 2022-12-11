using System;
using UnityEngine;

public class MovementStateMachine 
{
    public IMovementState CurrentState { get; private set; }

    public StillState stillState;
    public MovingState movingState;
    public FallingState fallingState;
    public JumpingUpState jumpingUpState;
    public UnderAnimationState underAnimationState;

    public event Action OnMoving_started, OnMoving_canceled, OnStateChanged;

    public MovementStateMachine(MovementController plMovement, CharacterController characterController, 
        Animator animator, CameraController cameraController)
    {
        stillState = new StillState(plMovement, characterController, animator);
        movingState = new MovingState(plMovement, characterController, animator, cameraController);
        fallingState = new FallingState(plMovement, characterController, animator);
        jumpingUpState = new JumpingUpState(plMovement, characterController);
        underAnimationState = new UnderAnimationState();
    }

    public void Initialize(IMovementState startingState)
    {
        CurrentState = startingState;
        OnStateChanged?.Invoke();
    }

    public void Start()
    {
        CurrentState.Enter();
    }

    public void TransitionTo(IMovementState nextState)
    {
        var prevState = CurrentState;
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
        if (!prevState.Equals(movingState) && CurrentState.Equals(movingState))
        {
            OnMoving_started?.Invoke();
        }
        if (!prevState.Equals(stillState) && CurrentState.Equals(stillState))
        {
            OnMoving_canceled?.Invoke();
        }
        OnStateChanged?.Invoke();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void JumpUp_pressed()
    {
        CurrentState?.JumpUp_pressed();
    }

    public void MoveForward_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) 
    {
        CurrentState?.MoveForward_started();
    }

    public void MoveForward_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) 
    {
        CurrentState?.MoveForward_canceled();
    }

    public void MoveAside_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) 
    {
        CurrentState?.MoveAside_started();
    }

    public void MoveAside_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) 
    {
        CurrentState?.MoveAside_canceled();
    }
}
