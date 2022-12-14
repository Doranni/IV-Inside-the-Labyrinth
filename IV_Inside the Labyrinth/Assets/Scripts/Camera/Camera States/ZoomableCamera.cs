using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ZoomableCamera 
{
    protected readonly CameraController cameraController;
    protected readonly CinemachineVirtualCamera camera;
    protected readonly CinemachineFramingTransposer framingTransposer;

    protected float scrollWheelInput;
    protected readonly float scrollWheelRange;

    protected readonly float zoomingSpeed, distanceMin, distanceMax, distanceRange, distanceStep, distanceMid;

    protected float neededDistance;
    protected bool needToZoomCamera = false;

    public ZoomableCamera(CameraController cameraController, CinemachineVirtualCamera camera,
        float scrollWheelRange, float zoomingSpeed, float distanceMin, float distanceMax)
    {
        this.cameraController = cameraController;
        this.camera = camera;
        framingTransposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        this.zoomingSpeed = zoomingSpeed;
        this.distanceMin = distanceMin;
        this.distanceMax = distanceMax;

        scrollWheelInput = 0;
        this.scrollWheelRange = scrollWheelRange;
        float scrollStep = scrollWheelRange * 2;

        distanceRange = (distanceMax - distanceMin);
        distanceStep = distanceRange / scrollStep;
        distanceMid = (distanceMax + distanceMin) / 2;
        neededDistance = distanceMid;
    }

    protected virtual void HandleScrollWheelInput(InputAction.CallbackContext context) 
    {
        float input = scrollWheelInput + context.ReadValue<float>();
        scrollWheelInput = Mathf.Clamp(input, -scrollWheelRange, scrollWheelRange);
        neededDistance = distanceMid + distanceStep * scrollWheelInput;
        needToZoomCamera = true;
    }

    protected virtual void ResetDistance() 
    {
        scrollWheelInput = 0;
        framingTransposer.m_CameraDistance = distanceMid;
    }

    protected virtual bool IsZoomValid()
    {
        return framingTransposer.m_CameraDistance == neededDistance;
    }

    protected virtual void PerformZoom()
    {
        if (needToZoomCamera)
        {
            framingTransposer.m_CameraDistance = Mathf.MoveTowards(framingTransposer.m_CameraDistance,
                neededDistance, Time.deltaTime * zoomingSpeed);
            needToZoomCamera = !IsZoomValid();
        }
    }
}
