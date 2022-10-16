using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public enum ViewMode
    {
        firstViewMode,
        thirdViewMode,
        mapViewMode
    }

    public enum ThirdVCameraMode
    {
        pov, 
        follow
    }

    private bool rightClickInput;

    private InputManager inputManager;
    [SerializeField] private AnimationAndMovementController plMovement;
    private CinemachineBrain cinemachineBrain;

    [SerializeField] private CinemachineVirtualCamera thirdViewCameraPOV, thirdViewCameraFollow, 
        mapViewCamera, firstViewCamera;

    private float scrollWheelInput = 0;
    private float scrollWheelInput_thirdView, scrollWheelInput_mapView;
    [SerializeField] private float scrollWheelMin, scrollWheelMax;
    [SerializeField] private bool needToZoomCamera = false;

    [SerializeField] private float thirdViewZoomingSpeed;
    [SerializeField] private float thirdViewDistanceMin, thirdViewDistanceMax;
    private float thirdViewDistanceRange, thirdViewDistanceStep, thirdViewDistanceMid;
    private float neededThirdViewDistance;
    private CinemachineFramingTransposer thirdViewTransposerPOV, thirdViewTransposerFollow;
    private CinemachinePOV thirdViewPOV;

    [SerializeField] private float mapViewZoomingSpeed;
    [SerializeField] private float mapViewDistanceMin, mapViewDistanceMax;
    private float mapViewDistanceStep, mapViewDistanceMid;
    private float neededMapViewDistance;
    private CinemachineFramingTransposer mapViewTransposer;

    [SerializeField]
    private float thirdViewFollowOffset_maxDistance, thirdViewFollowOffset_minDistance,
        thirdViewFollowTilt_maxDistance, thirdViewFollowTilt_minDistance;
    private CinemachineCameraOffset thirdViewFollowOffcet;
    private CinemachineRecomposer thirdViewFollowRecomposer;

    [SerializeField] private ViewMode vMode = ViewMode.thirdViewMode;
    public ViewMode VMode => vMode;

    [SerializeField] private ThirdVCameraMode thirdVCamMode;

    private Coroutine switchToPOV;

    private void Awake()

    {
        inputManager = InputManager.instance;

        inputManager.OnZoom_started += HandleScrollWheelInput;
        inputManager.OnZoom_performed += HandleScrollWheelInput;
        inputManager.OnZoom_canceled += HandleScrollWheelInput;

        // TODO: to fix first view camera
        //inputManager.OnFirstViewToggle_performed += FirstViewToggle_performed;
        inputManager.OnMapViewToggle_performed += MapViewToggle_performed;

        inputManager.OnStartRotation_started += StartRotation_started;
        inputManager.OnStartRotation_canceled += StartRotation_canceled;

        float scrollStep = scrollWheelMax - scrollWheelMin;
        thirdViewDistanceRange = (thirdViewDistanceMax - thirdViewDistanceMin);
        thirdViewDistanceStep = thirdViewDistanceRange / scrollStep;
        thirdViewDistanceMid = (thirdViewDistanceMax + thirdViewDistanceMin) / 2;
        neededThirdViewDistance = thirdViewDistanceMid;
        mapViewDistanceStep = (mapViewDistanceMax - mapViewDistanceMin) / scrollStep;
        mapViewDistanceMid = (mapViewDistanceMax + mapViewDistanceMin) / 2;
        neededMapViewDistance = mapViewDistanceMid;

        mapViewTransposer = mapViewCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        thirdViewTransposerPOV = thirdViewCameraPOV.GetCinemachineComponent<CinemachineFramingTransposer>();
        thirdViewTransposerFollow = thirdViewCameraFollow.GetCinemachineComponent<CinemachineFramingTransposer>();
        thirdViewPOV = thirdViewCameraPOV.GetCinemachineComponent<CinemachinePOV>();
        cinemachineBrain = GetComponent<CinemachineBrain>();
        thirdViewFollowOffcet = thirdViewCameraFollow.GetComponent<CinemachineCameraOffset>();
        thirdViewFollowRecomposer = thirdViewCameraFollow.GetComponent<CinemachineRecomposer>();
    }

    private void StartRotation_canceled(InputAction.CallbackContext obj)
    {
        rightClickInput = false;
        UpdateViewMode();
    }

    private void StartRotation_started(InputAction.CallbackContext obj)
    {
        rightClickInput = true;
        UpdateViewMode();
    }

    private void MapViewToggle_performed(InputAction.CallbackContext obj)
    {
        if (vMode == ViewMode.mapViewMode)
        {
            vMode = ViewMode.thirdViewMode;
            UpdateViewMode();
        }
        else
        {
            vMode = ViewMode.mapViewMode;
            UpdateViewMode();
        }
    }

    private void FirstViewToggle_performed(InputAction.CallbackContext obj)
    {
        if (vMode == ViewMode.firstViewMode)
        {
            vMode = ViewMode.thirdViewMode;
            UpdateViewMode();
        }
        else
        {
            vMode = ViewMode.firstViewMode;
            UpdateViewMode();
        }
    }

    private void HandleScrollWheelInput(InputAction.CallbackContext context)
    {
        if (vMode == ViewMode.firstViewMode)
        {
            return;
        }
        float input = scrollWheelInput + context.ReadValue<float>();
        scrollWheelInput = Mathf.Clamp(input, scrollWheelMin, scrollWheelMax);
        if (vMode == ViewMode.thirdViewMode)
        {
            neededThirdViewDistance = thirdViewDistanceMid + thirdViewDistanceStep * scrollWheelInput;
            scrollWheelInput_thirdView = scrollWheelInput;
        }
        else if (vMode == ViewMode.mapViewMode)
        {
            neededMapViewDistance = mapViewDistanceMid + mapViewDistanceStep * scrollWheelInput;
            scrollWheelInput_mapView = scrollWheelInput;
        }
        needToZoomCamera = true;
    }

    void Start()
    {
        ResetDistance();
        UpdateViewMode();
        UpdateRotation();
        UpdateDamping();
    }

    private void ResetDistance()
    {
        thirdViewTransposerPOV.m_CameraDistance = thirdViewDistanceMid;
        thirdViewTransposerFollow.m_CameraDistance = thirdViewDistanceMid;
        UpdateFollowOffset(thirdViewDistanceMid);
        mapViewTransposer.m_CameraDistance = mapViewDistanceMid;
        scrollWheelInput = 0;
    }

    private void SwitchCamera(ViewMode mode)
    {
        switch (mode)
        {
            case ViewMode.firstViewMode:
                {
                    firstViewCamera.Priority = 1;
                    thirdViewCameraPOV.Priority = 0;
                    thirdViewCameraFollow.Priority = 0;
                    mapViewCamera.Priority = 0;
                    break;
                }
            case ViewMode.thirdViewMode:
                {
                    scrollWheelInput = scrollWheelInput_thirdView;
                    if (thirdVCamMode == ThirdVCameraMode.pov)
                    {
                        thirdViewTransposerPOV.m_CameraDistance = neededThirdViewDistance;
                        thirdViewPOV.ForceCameraPosition(transform.position, transform.rotation);
                        thirdViewCameraPOV.Priority = 1;
                        thirdViewCameraFollow.Priority = 0;
                    }
                    else
                    {
                        if (thirdViewTransposerFollow.m_CameraDistance != neededThirdViewDistance)
                        {
                            thirdViewTransposerFollow.m_CameraDistance = neededThirdViewDistance;
                            UpdateFollowOffset(neededThirdViewDistance);
                        }
                        thirdViewCameraFollow.Priority = 1;
                        thirdViewCameraPOV.Priority = 0;
                    }
                    firstViewCamera.Priority = 0;
                    mapViewCamera.Priority = 0;
                    break;
                }
            case ViewMode.mapViewMode:
                {
                    mapViewTransposer.m_CameraDistance = neededMapViewDistance;
                    scrollWheelInput = scrollWheelInput_mapView;
                    mapViewCamera.Priority = 1;
                    firstViewCamera.Priority = 0;
                    thirdViewCameraPOV.Priority = 0;
                    thirdViewCameraFollow.Priority = 0;
                    break;
                }
        }
        vMode = mode;
    }

    private void UpdateThirdViewMode()
    {
        switch (Preferences.camRotStyle)
        {
            case Preferences.CameraRotationStyle.followPlayer:
                {
                    switch (Preferences.plRotStyle)
                    {
                        case Preferences.PlayerRotationStyle.withMouse:
                            {
                                if (thirdVCamMode != ThirdVCameraMode.pov)
                                {
                                    thirdVCamMode = ThirdVCameraMode.pov;
                                }
                                if (!cinemachineBrain.ActiveVirtualCamera.Equals(thirdViewCameraPOV))
                                {
                                    SwitchCamera(vMode);
                                }
                                break;
                            }
                        case Preferences.PlayerRotationStyle.withKeyboard:
                            {
                                if (plMovement.MoveState == AnimationAndMovementController.MovementState.moving)
                                {
                                    if (thirdVCamMode != ThirdVCameraMode.follow)
                                    {
                                        thirdVCamMode = ThirdVCameraMode.follow;
                                    }
                                    if (!cinemachineBrain.ActiveVirtualCamera.Equals(thirdViewCameraFollow))
                                    {
                                        SwitchCamera(vMode);
                                    }
                                }
                                else if (plMovement.MoveState == AnimationAndMovementController.MovementState.still)
                                {
                                    if (thirdVCamMode != ThirdVCameraMode.pov)
                                    {
                                        thirdVCamMode = ThirdVCameraMode.pov;
                                    }
                                    if (!cinemachineBrain.ActiveVirtualCamera.Equals(thirdViewCameraPOV))
                                    {
                                        SwitchCamera(vMode);
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
            case Preferences.CameraRotationStyle.withMouse:
                {
                    if (thirdVCamMode != ThirdVCameraMode.pov)
                    {
                        thirdVCamMode = ThirdVCameraMode.pov;
                    }
                    if (!cinemachineBrain.ActiveVirtualCamera.Equals(thirdViewCameraPOV))
                    {
                        SwitchCamera(vMode);
                    }
                    break;
                }
            case Preferences.CameraRotationStyle.withRightClickMouse:
                {
                    if (rightClickInput || plMovement.MoveState == AnimationAndMovementController.MovementState.still)
                    {
                        if (thirdVCamMode != ThirdVCameraMode.pov)
                        {
                            thirdVCamMode = ThirdVCameraMode.pov;
                        }
                        if (!cinemachineBrain.ActiveVirtualCamera.Equals(thirdViewCameraPOV))
                        {
                            SwitchCamera(vMode);
                        }
                    }
                    else
                    {
                        if (thirdVCamMode != ThirdVCameraMode.follow)
                        {
                            thirdVCamMode = ThirdVCameraMode.follow;
                        }
                        if (!cinemachineBrain.ActiveVirtualCamera.Equals(thirdViewCameraFollow))
                        {
                            SwitchCamera(vMode);
                        }
                    }
                    break;
                }
        }
    }

    public void UpdateViewMode()
    {
        switch (vMode)
        {
            case ViewMode.thirdViewMode:
                {
                    UpdateThirdViewMode();
                    break;
                }
            case ViewMode.firstViewMode:
                {
                    if (!cinemachineBrain.ActiveVirtualCamera.Equals(firstViewCamera))
                    {
                        SwitchCamera(ViewMode.firstViewMode);
                    }
                    break;
                }
            case ViewMode.mapViewMode:
                {
                    if (!cinemachineBrain.ActiveVirtualCamera.Equals(mapViewCamera))
                    {
                        SwitchCamera(ViewMode.mapViewMode);
                    }
                    break;
                }
        }
    }

    private void LateUpdate()
    {
        // Camera zoom due to ScrollWheel input
        if (needToZoomCamera)
        {
            if (vMode == ViewMode.thirdViewMode)
            {
                if (thirdVCamMode == ThirdVCameraMode.pov)
                {
                    thirdViewTransposerPOV.m_CameraDistance =
                        Mathf.MoveTowards(thirdViewTransposerPOV.m_CameraDistance,
                        neededThirdViewDistance, Time.deltaTime * thirdViewZoomingSpeed);
                }
                else
                {
                    float distance = Mathf.MoveTowards(thirdViewTransposerFollow.m_CameraDistance,
                        neededThirdViewDistance, Time.deltaTime * thirdViewZoomingSpeed);
                    thirdViewTransposerFollow.m_CameraDistance = distance;
                    UpdateFollowOffset(distance);
                }
                needToZoomCamera = CheckThirdViewCameraNeedToZoom();
            }
            else if (vMode == ViewMode.mapViewMode)
            {
                mapViewTransposer.m_CameraDistance =
                    Mathf.MoveTowards(mapViewTransposer.m_CameraDistance,
                    neededMapViewDistance, Time.deltaTime * mapViewZoomingSpeed);
                needToZoomCamera = CheckMapViewCameraNeedToZoom();
            }
        }
    }

    private void UpdateFollowOffset(float distance)
    {
        float distance_from0to1 = (distance - thirdViewDistanceMin) 
            / thirdViewDistanceRange;
        thirdViewFollowOffcet.m_Offset.y = Mathf.Lerp(thirdViewFollowOffset_minDistance,
                        thirdViewFollowOffset_maxDistance, distance_from0to1);
        thirdViewFollowRecomposer.m_Tilt = Mathf.Lerp(thirdViewFollowTilt_minDistance,
            thirdViewFollowTilt_maxDistance, distance_from0to1);
    }

    private bool CheckThirdViewCameraNeedToZoom()
    {
        if (thirdVCamMode == ThirdVCameraMode.pov)
        {
            return thirdViewTransposerPOV.m_CameraDistance != neededThirdViewDistance;
        }
        else
        {
            return thirdViewTransposerFollow.m_CameraDistance != neededThirdViewDistance;
        }
    }

    private bool CheckMapViewCameraNeedToZoom()
    {
        return mapViewTransposer.m_CameraDistance != neededMapViewDistance;
    }

    public void UpdateRotation()
    {
        thirdViewPOV.m_HorizontalAxis.m_MaxSpeed = Preferences.camRot_HorizontalSpeed;
        thirdViewPOV.m_VerticalAxis.m_MaxSpeed = Preferences.camRot_VerticalSpeed;
    }

    public void UpdateDamping()
    {
        //foreach (CinemachineOrbitalTransposer transposer in thirdViewTransposers)
        //{
        //    transposer.m_XDamping = Preferences.camFollowDamping_X;
        //    transposer.m_YDamping = Preferences.camFollowDamping_Y;
        //    transposer.m_ZDamping = Preferences.camFollowDamping_Z;
        //    transposer.m_YawDamping = Preferences.camFollowDamping_Yaw;
        //}
        //foreach (CinemachineComposer composer in thirdViewComposers)
        //{
        //    composer.m_HorizontalDamping = Preferences.camRotDamping_Horizontal;
        //    composer.m_VerticalDamping = Preferences.camRotDamping_Vertical;
        //}
    }
}
