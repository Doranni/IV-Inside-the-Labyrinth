using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public CameraStateMachine StateMachine { get; private set; }

    [SerializeField] private AnimationAndMovementController plMovement;

    [SerializeField] private CinemachineVirtualCamera thirdViewCameraPOV, thirdViewCameraFollow,
        firstViewCamera, mapViewCamera;

    [SerializeField] private float scrollWheelRange;

    [SerializeField] private float thirdViewZoomingSpeed;
    [SerializeField] private float thirdViewDistanceMin, thirdViewDistanceMax;

    [SerializeField] private float mapViewZoomingSpeed;
    [SerializeField] private float mapViewDistanceMin, mapViewDistanceMax;

    [SerializeField] private float thirdViewFollowOffset_minDistance, thirdViewFollowOffset_maxDistance,
        thirdViewFollowTilt_minDistance, thirdViewFollowTilt_maxDistance;

    private void Awake()

    {
        StateMachine = new CameraStateMachine(cameraController: this, player: plMovement,
            thirdViewCameraPOV: thirdViewCameraPOV, 
            thirdViewCameraFollow: thirdViewCameraFollow,
            scrollWheelRange: scrollWheelRange,
            thirdViewZoomingSpeed: thirdViewZoomingSpeed,
            thirdViewDistanceMin: thirdViewDistanceMin, thirdViewDistanceMax: thirdViewDistanceMax,
            thirdViewFollowOffset_maxDistance: thirdViewFollowOffset_maxDistance, 
            thirdViewFollowOffset_minDistance: thirdViewFollowOffset_minDistance,
            thirdViewFollowTilt_maxDistance: thirdViewFollowTilt_maxDistance, 
            thirdViewFollowTilt_minDistance: thirdViewFollowTilt_minDistance,
            firstViewCamera: firstViewCamera,
            mapViewCamera: mapViewCamera,  mapViewZoomingSpeed: mapViewZoomingSpeed,
            mapViewDistanceMin: mapViewDistanceMin,  mapViewDistanceMax: mapViewDistanceMax);
        StateMachine.Initialize(StateMachine.thirdViewState);

        // TODO: to fix first view camera
        //InputManager.instance.OnFirstViewToggle_performed += FirstViewToggle_performed;
        InputManager.instance.OnMapViewToggle_performed += MapViewToggle_performed;
    }

    private void MapViewToggle_performed(InputAction.CallbackContext obj)
    {
        StateMachine.MapViewToggle_performed();
    }

    private void FirstViewToggle_performed(InputAction.CallbackContext obj)
    {
        StateMachine.FirstViewToggle_performed();
    }

    private void LateUpdate()
    {
        StateMachine.LateUpdate();
    }

    private void OnDestroy()
    {
        //InputManager.instance.OnFirstViewToggle_performed -= FirstViewToggle_performed;
        InputManager.instance.OnMapViewToggle_performed -= MapViewToggle_performed;

        StateMachine.Destroy();
    }
}
