using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraInput cameraInput;

    [SerializeField] private CinemachineFreeLook firstViewCamera, thirdViewCamera;
    public CinemachineVirtualCamera mapViewCamera;

    [SerializeField] private float thirdViewZoomingSpeed;
    [SerializeField] private float thirdViewHeightMin, thirdViewHeightMax,
        thirdViewRadiusMin, thirdViewRadiusMax;
    private float thirdViewHeightStep, thirdViewHeightMid,
        thirdViewRadiusStep, thirdViewRadiusMid;
    private float neededThirdViewRadius, neededThirdViewHeight;
    private CinemachineVirtualCamera[] thirdViewRigs = new CinemachineVirtualCamera[3];
    private CinemachineOrbitalTransposer[] thirdViewTransposers = new CinemachineOrbitalTransposer[3];
    private CinemachineComposer[] thirdViewComposers = new CinemachineComposer[3];

    [SerializeField] private float mapViewZoomingSpeed;
    [SerializeField] private float mapViewOffsetMin, mapViewOffsetMax;
    private float mapViewOffsetStep, mapViewOffsetMid;
    private float neededmapViewOffset;
    private CinemachineFramingTransposer mapViewTranspoder;

    private float scrollWheelInput = 0;
    [SerializeField] private float scrollWheelMin, scrollWheelMax;
    private bool needToZoomCamera = false;

    ViewMode viewMode = ViewMode.thirdViewMode;

    private void Awake()
    {
        cameraInput = new CameraInput();
        cameraInput.Camera.Zoom.started += context => HandleScrollWheelInput(context);
        cameraInput.Camera.Zoom.performed += context => HandleScrollWheelInput(context);
        cameraInput.Camera.Zoom.canceled += context => HandleScrollWheelInput(context);

        cameraInput.Camera.FirstViewToggle.performed += FirstViewToggle_performed;
        cameraInput.Camera.MapViewToggle.performed += MapViewToggle_performed;

        cameraInput.Camera.StartRotation.started += StartRotation_started;
        cameraInput.Camera.StartRotation.canceled += StartRotation_canceled;

        float scrollStep = scrollWheelMax - scrollWheelMin;
        thirdViewHeightStep = (thirdViewHeightMax - thirdViewHeightMin) / scrollStep;
        thirdViewRadiusStep = (thirdViewRadiusMax - thirdViewRadiusMin) / scrollStep;
        thirdViewHeightMid = (thirdViewHeightMax + thirdViewHeightMin) / 2;
        thirdViewRadiusMid = (thirdViewRadiusMax + thirdViewRadiusMin) / 2;
        mapViewOffsetStep = (mapViewOffsetMax - mapViewOffsetMin) / scrollStep;
        mapViewOffsetMid = (mapViewOffsetMax + mapViewOffsetMin) / 2;

        mapViewTranspoder = mapViewCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        for (int i = 0; i < thirdViewTransposers.Length; i++)
        {
            thirdViewRigs[i] = thirdViewCamera.GetRig(i);
            thirdViewTransposers[i] = thirdViewRigs[i].GetCinemachineComponent<CinemachineOrbitalTransposer>();
            thirdViewComposers[i] = thirdViewRigs[i].GetCinemachineComponent<CinemachineComposer>();
        }
    }

    private void StartRotation_canceled(InputAction.CallbackContext obj)
    {
        if (Preferences.camRotStyle == Preferences.CameraRotationStyle.withRightClickMouse)
        {
            thirdViewCamera.m_XAxis.m_MaxSpeed = 0;
        }
    }

    private void StartRotation_started(InputAction.CallbackContext obj)
    {
        if (Preferences.camRotStyle == Preferences.CameraRotationStyle.withRightClickMouse)
        {
            thirdViewCamera.m_XAxis.m_MaxSpeed = Preferences.camRotSpeed;
        }
    }

    private void MapViewToggle_performed(InputAction.CallbackContext obj)
    {
        if (viewMode == ViewMode.mapViewMode)
        {
            thirdViewCamera.enabled = true;
            mapViewCamera.enabled = false;
            viewMode = ViewMode.thirdViewMode;
        }
        else
        {
            if (viewMode == ViewMode.thirdViewMode)
            {
                thirdViewCamera.enabled = false;
            }
            else if (viewMode == ViewMode.firstViewMode)
            {
                firstViewCamera.enabled = false;
            }

            mapViewCamera.enabled = true;
            viewMode = ViewMode.mapViewMode;
        }
    }

    private void FirstViewToggle_performed(InputAction.CallbackContext obj)
    {
        if (viewMode == ViewMode.firstViewMode)
        {
            thirdViewCamera.enabled = true;
            firstViewCamera.enabled = false;
            viewMode = ViewMode.thirdViewMode;
        }
        else
        {
            if (viewMode == ViewMode.thirdViewMode)
            {
                thirdViewCamera.enabled = false;
            }
            else if (viewMode == ViewMode.mapViewMode)
            {
                mapViewCamera.enabled = false;
            }
            firstViewCamera.enabled = true;
            viewMode = ViewMode.firstViewMode;
        }
    }

    private void HandleScrollWheelInput(InputAction.CallbackContext context)
    {
        if (viewMode == ViewMode.firstViewMode)
        {
            return;
        }
        float input = scrollWheelInput + context.ReadValue<float>();
        if (input > scrollWheelMax)
        {
            scrollWheelInput = scrollWheelMax;
        }
        else if (input < scrollWheelMin)
        {
            scrollWheelInput = scrollWheelMin;
        }
        else
        {
            scrollWheelInput = input;
        }
        if (viewMode == ViewMode.thirdViewMode)
        {
            neededThirdViewHeight = thirdViewHeightMid + thirdViewHeightStep * scrollWheelInput;
            neededThirdViewRadius = thirdViewRadiusMid + thirdViewRadiusStep * scrollWheelInput; 
        }
        else if (viewMode == ViewMode.mapViewMode)
        {
            neededmapViewOffset = mapViewOffsetMid + mapViewOffsetStep * scrollWheelInput;
        }
        needToZoomCamera = true;
    }

    void Start()
    {
        UpdateRotation();
        UpdateDamping();
        viewMode = ViewMode.thirdViewMode;
        thirdViewCamera.enabled = true;
        firstViewCamera.enabled = false;
        mapViewCamera.enabled = false;
    }

    private void LateUpdate()
    {
        // Camera zoom due to ScrollWheel input
        if (needToZoomCamera)
        {
            if (viewMode == ViewMode.thirdViewMode)
            {
                for (int i = 0; i < thirdViewCamera.m_Orbits.Length; i++)
                {
                    thirdViewCamera.m_Orbits[i].m_Height =
                        Mathf.MoveTowards(thirdViewCamera.m_Orbits[i].m_Height,
                        neededThirdViewHeight, Time.deltaTime * thirdViewZoomingSpeed);
                    thirdViewCamera.m_Orbits[i].m_Radius =
                        Mathf.MoveTowards(thirdViewCamera.m_Orbits[i].m_Radius,
                        neededThirdViewRadius, Time.deltaTime * thirdViewZoomingSpeed);
                }
                needToZoomCamera = CheckThirdViewCameraNeedToZoom();
            }
            else if (viewMode == ViewMode.mapViewMode)
            {
                mapViewTranspoder.m_TrackedObjectOffset.y =
                    Mathf.MoveTowards(mapViewTranspoder.m_TrackedObjectOffset.y,
                    neededmapViewOffset, Time.deltaTime * mapViewZoomingSpeed);
                needToZoomCamera = CheckMapViewCameraNeedToZoom();
            }
        }
    }

    private bool CheckThirdViewCameraNeedToZoom()
    {
        for (int i = 0; i < thirdViewCamera.m_Orbits.Length; i++)
        {
            if (thirdViewCamera.m_Orbits[i].m_Height != neededThirdViewHeight||
                thirdViewCamera.m_Orbits[i].m_Radius != neededThirdViewRadius)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckMapViewCameraNeedToZoom()
    {
        return mapViewTranspoder.m_TrackedObjectOffset.y != neededmapViewOffset;
    }

    enum ViewMode
    {
        firstViewMode,
        thirdViewMode,
        mapViewMode
    }

    private void OnEnable()
    {
        cameraInput.Enable();
    }

    private void OnDisable()
    {
        cameraInput.Disable();
    }

    public void UpdateRotation()
    {
        switch (Preferences.camRotStyle)
        {
            case Preferences.CameraRotationStyle.followPlayer:
                {
                    thirdViewCamera.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
                    thirdViewCamera.m_XAxis.m_MaxSpeed = 0;
                    break;
                }
            case Preferences.CameraRotationStyle.withMouse:
                {
                    thirdViewCamera.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
                    thirdViewCamera.m_XAxis.m_MaxSpeed = Preferences.camRotSpeed;
                    break;
                }
            case Preferences.CameraRotationStyle.withRightClickMouse:
                {
                    thirdViewCamera.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
                    thirdViewCamera.m_XAxis.m_MaxSpeed = 0;
                    break;
                }
        }
    }

    public void UpdateDamping()
    {
        foreach (CinemachineOrbitalTransposer transposer in thirdViewTransposers)
        {
            transposer.m_XDamping = Preferences.camFollowDamping_X;
            transposer.m_YDamping = Preferences.camFollowDamping_Y;
            transposer.m_ZDamping = Preferences.camFollowDamping_Z;
            transposer.m_YawDamping = Preferences.camFollowDamping_Yaw;
        }
        foreach (CinemachineComposer composer in thirdViewComposers)
        {
            composer.m_HorizontalDamping = Preferences.camRotDamping_Horizontal;
            composer.m_VerticalDamping = Preferences.camRotDamping_Vertical;
        }
    }
}
