using UnityEngine;
using Cinemachine;

public class CameraThirdView_FollowState : ZoomableCamera, ICameraThirdViewState
{
    private readonly float cameraOffsetValueMax, cameraOffsetValueMin, cameraTiltValueMax, cameraTiltValueMin;
    private readonly CinemachineCameraOffset cameraOffcet;
    private readonly CinemachineRecomposer recomposer;
    private readonly CinemachineSameAsFollowTarget sameAsFollowTarget;

    public CameraThirdView_FollowState(CameraController cameraController, CinemachineVirtualCamera camera,
        float scrollWheelRange, float zoomingSpeed, float distanceMin, float distanceMax,
        float cameraOffsetValueMin, float cameraOffsetValueMax, float cameraTiltValueMin, float cameraTiltValueMax)
        : base(cameraController, camera, scrollWheelRange, zoomingSpeed, distanceMin, distanceMax)
    {
        cameraOffcet = camera.GetComponent<CinemachineCameraOffset>();
        recomposer = camera.GetComponent<CinemachineRecomposer>();
        sameAsFollowTarget = camera.GetCinemachineComponent<CinemachineSameAsFollowTarget>();

        this.cameraOffsetValueMin = cameraOffsetValueMin;
        this.cameraOffsetValueMax = cameraOffsetValueMax;
        this.cameraTiltValueMin = cameraTiltValueMin;
        this.cameraTiltValueMax = cameraTiltValueMax;
    }

    public void Initialize()
    {
        camera.Priority = 0;
        ResetDistance();
        UpdateDamping();
        InputManager.instance.OnZoom_started += HandleScrollWheelInput;
        InputManager.instance.OnZoom_performed += HandleScrollWheelInput;
        InputManager.instance.OnZoom_canceled += HandleScrollWheelInput;
        Preferences.OnCamDamping_BodyXChanged += UpdateDamping_BodyX;
        Preferences.OnCamDamping_BodyYChanged += UpdateDamping_BodyY;
        Preferences.OnCamDamping_BodyZChanged += UpdateDamping_BodyZ;
        Preferences.OnCamDamping_AimChanged += UpdateDamping_Aim;
    }

    public void Enter()
    {
        camera.Priority = 1;
    }

    public void Exit()
    {
        camera.Priority = 0;
    }

    protected override void ResetDistance()
    {
        base.ResetDistance();
        UpdateFollowOffset(distanceMid);
    }

    protected override void HandleScrollWheelInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (cameraController.StateMachine.CurrentState.Equals(cameraController.StateMachine.thirdViewState))
        {
            base.HandleScrollWheelInput(context);
        }
    }

    protected override void PerformZoom()
    {
        base.PerformZoom();
        UpdateFollowOffset(framingTransposer.m_CameraDistance);
    }

    public void LateUpdate()
    {
        PerformZoom();
    }

    private void UpdateFollowOffset(float distance)
    {
        float distance_from0to1 = (distance - distanceMin) / distanceRange;
        cameraOffcet.m_Offset.y = Mathf.Lerp(cameraOffsetValueMin, cameraOffsetValueMax, distance_from0to1);
        recomposer.m_Tilt = Mathf.Lerp(cameraTiltValueMin, cameraTiltValueMax, distance_from0to1);
    }

    private void UpdateDamping()
    {
        UpdateDamping_BodyX();
        UpdateDamping_BodyY();
        UpdateDamping_BodyZ();
        UpdateDamping_Aim();
    }

    public void UpdateDamping_BodyX()
    {
        framingTransposer.m_XDamping = Preferences.camDamping_BodyX;
    }

    public void UpdateDamping_BodyY()
    {
        framingTransposer.m_YDamping = Preferences.camDamping_BodyY;
    }

    public void UpdateDamping_BodyZ()
    {
        framingTransposer.m_ZDamping = Preferences.camDamping_BodyZ;
    }

    public void UpdateDamping_Aim()
    {
        sameAsFollowTarget.m_Damping = Preferences.camDamping_Aim;
    }

    public override string ToString()
    {
        return "ThirdV: Follow";
    }

    public void Destroy()
    {
        InputManager.instance.OnZoom_started -= HandleScrollWheelInput;
        InputManager.instance.OnZoom_performed -= HandleScrollWheelInput;
        InputManager.instance.OnZoom_canceled -= HandleScrollWheelInput;
        Preferences.OnCamDamping_BodyXChanged -= UpdateDamping_BodyX;
        Preferences.OnCamDamping_BodyYChanged -= UpdateDamping_BodyY;
        Preferences.OnCamDamping_BodyZChanged -= UpdateDamping_BodyZ;
        Preferences.OnCamDamping_AimChanged -= UpdateDamping_Aim;
    }
}
