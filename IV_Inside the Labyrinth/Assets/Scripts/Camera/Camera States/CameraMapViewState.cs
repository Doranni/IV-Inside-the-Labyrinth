using Cinemachine;

public class CameraMapViewState : ZoomableCamera, ICameraState
{
    public CameraMapViewState (CameraController cameraController, CinemachineVirtualCamera camera,
        float scrollWheelRange, float zoomingSpeed, float distanceMin, float distanceMax)
        : base (cameraController, camera, scrollWheelRange, zoomingSpeed, distanceMin, distanceMax)
    {

    }

    public void Initialize()
    {
        ResetDistance();
        camera.Priority = 0;
        InputManager.instance.OnZoom_started += HandleScrollWheelInput;
        InputManager.instance.OnZoom_performed += HandleScrollWheelInput;
        InputManager.instance.OnZoom_canceled += HandleScrollWheelInput;
    }

    public void Enter()
    {
        camera.Priority = 1;
    }

    public void Exit()
    {
        camera.Priority = 0;
    }

    protected override void HandleScrollWheelInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (cameraController.StateMachine.CurrentState.Equals(cameraController.StateMachine.mapViewState))
        {
            base.HandleScrollWheelInput(context);
        }
    }

    public void LateUpdate() 
    {
        PerformZoom();
    }

    public void MapViewToggle_performed()
    {
        cameraController.StateMachine.TransitionTo(cameraController.StateMachine.thirdViewState);
    }

    public void FirstViewToggle_performed()
    {
        cameraController.StateMachine.TransitionTo(cameraController.StateMachine.firstViewState);
    }

    public void Destroy() 
    {
        InputManager.instance.OnZoom_started -= HandleScrollWheelInput;
        InputManager.instance.OnZoom_performed -= HandleScrollWheelInput;
        InputManager.instance.OnZoom_canceled -= HandleScrollWheelInput;
    }
}
