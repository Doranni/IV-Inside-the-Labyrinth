using System;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;

public class CameraThirdViewState : ICameraState
{
    public ICameraThirdViewState CurrentState { get; private set; }

    public CameraThitdView_PovState povState;
    public CameraThirdView_FollowState followState;

    private readonly CameraController cameraController;
    private readonly MovementController player;

    private bool rightClickInput;

    public event Action OnStateChanged;

    public CameraThirdViewState(CameraController cameraController, MovementController player, 
        CinemachineVirtualCamera thirdViewCameraPOV, CinemachineVirtualCamera thirdViewCameraFollow,
        float thirdViewFollowOffset_maxDistance, float thirdViewFollowOffset_minDistance,
        float thirdViewFollowTilt_maxDistance, float thirdViewFollowTilt_minDistance,
        float scrollWheelRange, float thirdViewZoomingSpeed, float thirdViewDistanceMin, float thirdViewDistanceMax)
    {
        this.cameraController = cameraController;
        this.player = player;

        povState = new CameraThitdView_PovState(cameraController: cameraController, camera: thirdViewCameraPOV,
            scrollWheelRange: scrollWheelRange, zoomingSpeed: thirdViewZoomingSpeed, 
            distanceMin: thirdViewDistanceMin, distanceMax: thirdViewDistanceMax);

        followState = new CameraThirdView_FollowState(cameraController: cameraController, camera: thirdViewCameraFollow,
            scrollWheelRange: scrollWheelRange, zoomingSpeed: thirdViewZoomingSpeed, 
            distanceMin: thirdViewDistanceMin, distanceMax: thirdViewDistanceMax,
            cameraOffsetValueMin: thirdViewFollowOffset_minDistance, cameraOffsetValueMax: thirdViewFollowOffset_maxDistance,
            cameraTiltValueMin: thirdViewFollowTilt_minDistance, cameraTiltValueMax: thirdViewFollowTilt_maxDistance);
    }

    public void Initialize()
    {
        SetCurrentState();
        povState.Initialize();
        followState.Initialize();
        Preferences.OnCamRotStyleChanged += SetCurrentState;
        player.StateMachine.OnMoving_started += SetCurrentState;
        player.StateMachine.OnMoving_canceled += SetCurrentState;
        InputManager.instance.OnStartRotation_started += StartRotation_started;
        InputManager.instance.OnStartRotation_canceled += StartRotation_canceled;
    }

    public void Enter()
    {
        if (CurrentState == null)
        {
            SetCurrentState();
        }
        CurrentState.Enter();
    }

    public void Exit()
    {
        CurrentState?.Exit();
    }

    private void SetCurrentState()
    {
        ICameraThirdViewState prevState = CurrentState;
        switch (Preferences.PlRotStyle)
        {
            case Preferences.PlayerRotationStyle.withMouse:
                {
                    switch (Preferences.CamRotStyle)
                    {
                        case Preferences.CameraRotationStyle.followPlayer:
                            {
                                CurrentState = povState;
                                break;
                            }
                        case Preferences.CameraRotationStyle.withRightClickMouse:
                            {
                                if (rightClickInput || 
                                    !player.StateMachine.CurrentState.Equals(player.StateMachine.movingState))
                                {
                                    CurrentState = povState;
                                }
                                else
                                {
                                    CurrentState = followState;
                                }
                                break;
                            }
                    }
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    switch (Preferences.CamRotStyle)
                    {
                        case Preferences.CameraRotationStyle.withMouse:
                            {
                                CurrentState = povState;
                                break;
                            }
                        case Preferences.CameraRotationStyle.followPlayer:
                            {
                                if (player.StateMachine.CurrentState.Equals(player.StateMachine.movingState))
                                {
                                    CurrentState = followState;
                                }
                                else
                                {
                                    CurrentState = povState;
                                }
                                break;
                            }
                        case Preferences.CameraRotationStyle.withRightClickMouse:
                            {
                                if (rightClickInput ||
                                    !player.StateMachine.CurrentState.Equals(player.StateMachine.movingState))
                                {
                                    CurrentState = povState;
                                }
                                else
                                {
                                    CurrentState = followState;
                                }
                                break;
                            }
                    }
                    break;
                }
        }
        if (cameraController.StateMachine.CurrentState.Equals(cameraController.StateMachine.thirdViewState)
            && (prevState == null || !prevState.Equals(CurrentState)))
        {
            CurrentState.Enter();
            prevState?.Exit();
            OnStateChanged?.Invoke();
        }
    }

    private void StartRotation_canceled(InputAction.CallbackContext obj)
    {
        rightClickInput = false;
        SetCurrentState();
    }

    private void StartRotation_started(InputAction.CallbackContext obj)
    {
        rightClickInput = true;
        SetCurrentState();
    }

    public void LateUpdate() 
    {
        CurrentState?.LateUpdate();
    }

    public void MapViewToggle_performed()
    {
        cameraController.StateMachine.TransitionTo(cameraController.StateMachine.mapViewState);
    }

    public void FirstViewToggle_performed()
    {
        cameraController.StateMachine.TransitionTo(cameraController.StateMachine.firstViewState);
    }

    public override string ToString()
    {
        return CurrentState.ToString();
    }

    public void Destroy()
    {
        Preferences.OnCamRotStyleChanged -= SetCurrentState;
        player.StateMachine.OnMoving_started -= SetCurrentState;
        player.StateMachine.OnMoving_canceled -= SetCurrentState;
        InputManager.instance.OnStartRotation_started -= StartRotation_started;
        InputManager.instance.OnStartRotation_canceled -= StartRotation_canceled;
        povState.Destroy();
        followState.Destroy();
    }
}
