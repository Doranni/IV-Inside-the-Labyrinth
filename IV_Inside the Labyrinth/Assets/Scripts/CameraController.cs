using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraInput cameraInput;

    public CinemachineFreeLook firstViewCamera, thirdViewCamera;
    public CinemachineVirtualCamera mapViewCamera;

    public float thirdViewZoomingSpeed;
    public float thirdViewHeightMin, thirdViewHeightMax,
        thirdViewRadiusMin, thirdViewRadiusMax;
    private float thirdViewHeightStep, thirdViewHeightMid,
        thirdViewRadiusStep, thirdViewRadiusMid;
    private float neededThirdViewRadius, neededThirdViewHeight;

    public float mapViewZoomingSpeed;
    public float mapViewOffsetMin, mapViewOffsetMax;
    private float mapViewOffsetStep, mapViewOffsetMid;
    private float neededmapViewOffset;
    private CinemachineFramingTransposer mapViewTranspoder;

    private float scrollWheelInput = 0;
    public float scrollWheelMin, scrollWheelMax;
    private bool needToZoomCamera = false;

    ViewMode viewMode = ViewMode.thirdViewMode;

    private void Awake()
    {
        cameraInput = new CameraInput();
        cameraInput.Camera.Zoom.started += context => HandleScrollWheelInput(context);
        cameraInput.Camera.Zoom.performed += context => HandleScrollWheelInput(context);
        cameraInput.Camera.Zoom.canceled += context => HandleScrollWheelInput(context);

        float scrollStep = scrollWheelMax - scrollWheelMin;
        thirdViewHeightStep = (thirdViewHeightMax - thirdViewHeightMin) / scrollStep;
        thirdViewRadiusStep = (thirdViewRadiusMax - thirdViewRadiusMin) / scrollStep;
        thirdViewHeightMid = (thirdViewHeightMax + thirdViewHeightMin) / 2;
        thirdViewRadiusMid = (thirdViewRadiusMax + thirdViewRadiusMin) / 2;
        mapViewOffsetStep = (mapViewOffsetMax - mapViewOffsetMin) / scrollStep;
        mapViewOffsetMid = (mapViewOffsetMax + mapViewOffsetMin) / 2;

        mapViewTranspoder = mapViewCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
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

    void Update()
    {
        // Toggle First person view
        if (cameraInput.Camera.FirstViewToggle.WasPressedThisFrame())
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
        //Toggle Map view
        if (cameraInput.Camera.MapViewToggle.WasPressedThisFrame())
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
}
