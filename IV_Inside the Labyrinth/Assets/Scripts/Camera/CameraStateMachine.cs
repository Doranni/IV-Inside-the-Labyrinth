using Cinemachine;
using System;

[Serializable]
public class CameraStateMachine
{
    public ICameraState CurrentState { get; private set; }

    public CameraThirdViewState thirdViewState;
    public CameraFirstViewState firstViewState;
    public CameraMapViewState mapViewState;

    public CameraStateMachine(CameraController cameraController, AnimationAndMovementController player,
        CinemachineVirtualCamera thirdViewCameraPOV, CinemachineVirtualCamera thirdViewCameraFollow,
        float scrollWheelRange, float thirdViewZoomingSpeed, float thirdViewDistanceMin, float thirdViewDistanceMax,
        float thirdViewFollowOffset_maxDistance, float thirdViewFollowOffset_minDistance,
        float thirdViewFollowTilt_maxDistance, float thirdViewFollowTilt_minDistance,
        CinemachineVirtualCamera firstViewCamera, 
        CinemachineVirtualCamera mapViewCamera, float mapViewZoomingSpeed,
        float mapViewDistanceMin, float mapViewDistanceMax)
    {
        thirdViewState = new CameraThirdViewState(cameraController: cameraController, player: player,
            thirdViewCameraPOV: thirdViewCameraPOV, thirdViewCameraFollow: thirdViewCameraFollow, 
            thirdViewFollowOffset_maxDistance: thirdViewFollowOffset_maxDistance,
            thirdViewFollowOffset_minDistance: thirdViewFollowOffset_minDistance,
            thirdViewFollowTilt_maxDistance: thirdViewFollowTilt_maxDistance,
            thirdViewFollowTilt_minDistance: thirdViewFollowTilt_minDistance,
            scrollWheelRange: scrollWheelRange, thirdViewZoomingSpeed: thirdViewZoomingSpeed,
            thirdViewDistanceMin: thirdViewDistanceMin, thirdViewDistanceMax: thirdViewDistanceMax);

        firstViewState = new CameraFirstViewState(cameraController, firstViewCamera);

        mapViewState = new CameraMapViewState(cameraController: cameraController, camera: mapViewCamera,
            scrollWheelRange: scrollWheelRange, zoomingSpeed: mapViewZoomingSpeed, 
            distanceMin: mapViewDistanceMin, distanceMax: mapViewDistanceMax);
    }

    public void Initialize(ICameraState startingState)
    {
        CurrentState = startingState;
        thirdViewState.Initialize();
        mapViewState.Initialize();
        firstViewState.Initialize();
        startingState.Enter();
    }

    public void TransitionTo(ICameraState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
    }

    public void LateUpdate()
    {
        CurrentState?.LateUpdate();
    }

    public void MapViewToggle_performed()
    {
        CurrentState?.MapViewToggle_performed();
    }

    public void FirstViewToggle_performed()
    {
        CurrentState?.FirstViewToggle_performed();
    }

    public void Destroy()
    {
        thirdViewState.Destroy();
        firstViewState.Destroy();
        mapViewState.Destroy();
    }
}
