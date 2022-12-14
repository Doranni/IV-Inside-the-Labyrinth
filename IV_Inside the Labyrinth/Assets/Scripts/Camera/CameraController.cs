using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public CameraStateMachine StateMachine { get; private set; }

    [SerializeField] private MovementController plMovement;

    [SerializeField] private CinemachineVirtualCamera thirdViewCameraPOV, thirdViewCameraFollow,
        firstViewCamera, mapViewCamera;

    [SerializeField] private float scrollWheelRange = 1200;

    [SerializeField] private float thirdViewZoomingSpeed = 10;
    [SerializeField] private float thirdViewDistanceMin = 1.5f, thirdViewDistanceMax = 12;

    [SerializeField] private float mapViewZoomingSpeed = 40;
    [SerializeField] private float mapViewDistanceMin = 10, mapViewDistanceMax = 70;

    [SerializeField] private float thirdViewFollowOffset_minDistance = 0, thirdViewFollowOffset_maxDistance = 2,
        thirdViewFollowTilt_minDistance = 2, thirdViewFollowTilt_maxDistance = 1;

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
    }

    private void Start()
    {
        StateMachine.Start();
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
