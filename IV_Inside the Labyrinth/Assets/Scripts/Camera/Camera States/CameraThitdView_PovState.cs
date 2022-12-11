using Cinemachine;

public class CameraThitdView_PovState : ZoomableCamera, ICameraThirdViewState
{
    private readonly CinemachinePOV pov;

    public CameraThitdView_PovState(CameraController cameraController, CinemachineVirtualCamera camera,
        float scrollWheelRange, float zoomingSpeed, float distanceMin, float distanceMax)
        : base (cameraController, camera, scrollWheelRange, zoomingSpeed, distanceMin, distanceMax)
    {
        pov = camera.GetCinemachineComponent<CinemachinePOV>();
    }

    public void Initialize()
    {
        camera.Priority = 0;
    }

    public void Start()
    {
        ResetDistance();
        UpdateRotation();
        UpdateDamping();
        InputManager.instance.OnZoom_started += HandleScrollWheelInput;
        InputManager.instance.OnZoom_performed += HandleScrollWheelInput;
        InputManager.instance.OnZoom_canceled += HandleScrollWheelInput;
        Preferences.OnCamRotSpeed_HorizontalChanged += UpdateHorizontalRotation;
        Preferences.OnCamRotSpeed_VerticalChanged += UpdateVerticalRotation;
        Preferences.OnCamDamping_BodyXChanged += UpdateDamping_BodyX;
        Preferences.OnCamDamping_BodyYChanged += UpdateDamping_BodyY;
        Preferences.OnCamDamping_BodyZChanged += UpdateDamping_BodyZ;
    }

    public void Enter()
    {
        camera.ForceCameraPosition(cameraController.transform.position, cameraController.transform.rotation);
        camera.Priority = 1;
    }

    public void Exit()
    {
        camera.Priority = 0;
    }

    protected override void HandleScrollWheelInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (cameraController.StateMachine.CurrentState.Equals(cameraController.StateMachine.thirdViewState))
        {
            base.HandleScrollWheelInput(context);
        }
    }

    public void LateUpdate() 
    {
        PerformZoom();
    }

    public void UpdateRotation()
    {
        UpdateHorizontalRotation();
        UpdateVerticalRotation();
    }

    public void UpdateHorizontalRotation()
    {
        pov.m_HorizontalAxis.m_MaxSpeed = Preferences.camRot_HorizontalSpeed;
    }

    public void UpdateVerticalRotation()
    {
        pov.m_VerticalAxis.m_MaxSpeed = Preferences.camRot_VerticalSpeed;
    }

    public void UpdateDamping()
    {
        UpdateDamping_BodyX();
        UpdateDamping_BodyY();
        UpdateDamping_BodyZ();
    }

    private void UpdateDamping_BodyZ()
    {
        framingTransposer.m_ZDamping = Preferences.camDamping_BodyZ;
    }

    private void UpdateDamping_BodyY()
    {
        framingTransposer.m_YDamping = Preferences.camDamping_BodyY;
    }

    private void UpdateDamping_BodyX()
    {
        framingTransposer.m_XDamping = Preferences.camDamping_BodyX;
    }

    public override string ToString()
    {
        return "ThirdV: POV";
    }

    public void Destroy()
    {
        InputManager.instance.OnZoom_started -= HandleScrollWheelInput;
        InputManager.instance.OnZoom_performed -= HandleScrollWheelInput;
        InputManager.instance.OnZoom_canceled -= HandleScrollWheelInput;
        Preferences.OnCamRotSpeed_HorizontalChanged -= UpdateHorizontalRotation;
        Preferences.OnCamRotSpeed_VerticalChanged -= UpdateVerticalRotation;
        Preferences.OnCamDamping_BodyXChanged -= UpdateDamping_BodyX;
        Preferences.OnCamDamping_BodyYChanged -= UpdateDamping_BodyY;
        Preferences.OnCamDamping_BodyZChanged -= UpdateDamping_BodyZ;
    }
}
