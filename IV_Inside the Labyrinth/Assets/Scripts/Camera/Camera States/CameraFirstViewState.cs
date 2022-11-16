using Cinemachine;

public class CameraFirstViewState : ICameraState
{
    private readonly CameraController cameraController;
    private readonly CinemachineVirtualCamera camera;

    public CameraFirstViewState(CameraController cameraController, CinemachineVirtualCamera camera)
    {
        this.cameraController = cameraController;
        this.camera = camera;
    }

    public void Initialize() 
    {
        camera.Priority = 0;
    }

    public void Enter()
    {
        camera.Priority = 1;
    }

    public void Exit()
    {
        camera.Priority = 0;
    }

    public void LateUpdate() { }

    public void MapViewToggle_performed() 
    {
        cameraController.StateMachine.TransitionTo(cameraController.StateMachine.mapViewState);
    }

    public void FirstViewToggle_performed() 
    {
        cameraController.StateMachine.TransitionTo(cameraController.StateMachine.thirdViewState);
    }

    public override string ToString()
    {
        return "First View";
    }

    public void Destroy() { }
}
