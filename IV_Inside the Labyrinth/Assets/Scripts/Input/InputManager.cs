using System;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private GameInput gameInput;

    public event Action<InputAction.CallbackContext> OnMenu_performed,
        OnMoveForward_performed, OnMoveForward_started, OnMoveForward_canceled,
        OnMoveAside_started, OnMoveAside_canceled,
        OnRotateMouse_started, OnRotateMouse_performed, OnRotateMouse_canceled,
        OnRotateKeyboard_started, OnRotateKeyboard_performed, OnRotateKeyboard_canceled,
        OnAcceleration_started, OnAcceleration_canceled,
        OnMouseRightClick_started, OnMouseRightClick_canceled,
        OnJumpUp_performed,
        OnZoom_started, OnZoom_performed, OnZoom_canceled,
        OnFirstViewToggle_performed,
        OnMapViewToggle_performed,
        OnStartRotation_started, OnStartRotation_canceled;

    public override void Awake()
    {
        base.Awake();
        gameInput = new GameInput();

    }

    private void Start()
    {
        gameInput.Game.Menu.performed += Menu_performed;

        gameInput.Movement.MoveForward.performed += MoveForward_performed;
        gameInput.Movement.MoveForwardButton.started += MoveForward_started;
        gameInput.Movement.MoveForwardButton.canceled += MoveForward_canceled;
        gameInput.Movement.MoveAsideButton.started += MoveAside_started;
        gameInput.Movement.MoveAsideButton.canceled += MoveAside_canceled;

        gameInput.Movement.RotateMouse.started += RotateMouse_started;
        gameInput.Movement.RotateMouse.performed += RotateMouse_performed;
        gameInput.Movement.RotateMouse.canceled += RotateMouse_canceled;

        gameInput.Movement.RotateKeyboard.started += RotateKeyboard_started;
        gameInput.Movement.RotateKeyboard.performed += RotateKeyboard_performed;
        gameInput.Movement.RotateKeyboard.canceled += RotateKeyboard_canceled;

        gameInput.Movement.Acceleration.started += Acceleration_started;
        gameInput.Movement.Acceleration.canceled += Acceleration_canceled;

        gameInput.Movement.MouseRightClick.started += MouseRightClick_started;
        gameInput.Movement.MouseRightClick.canceled += MouseRightClick_canceled;

        gameInput.Movement.JumpUp.performed += JumpUp_performed;

        gameInput.Camera.Zoom.started += Zoom_started;
        gameInput.Camera.Zoom.performed += Zoom_performed;
        gameInput.Camera.Zoom.canceled += Zoom_canceled;

        gameInput.Camera.FirstViewToggle.performed += FirstViewToggle_performed;

        gameInput.Camera.MapViewToggle.performed += MapViewToggle_performed;

        gameInput.Camera.StartRotation.started += StartRotation_started;
        gameInput.Camera.StartRotation.canceled += StartRotation_canceled;
    }

    private void StartRotation_canceled(InputAction.CallbackContext obj)
    {
        OnStartRotation_canceled?.Invoke(obj);
    }

    private void StartRotation_started(InputAction.CallbackContext obj)
    {
        OnStartRotation_started?.Invoke(obj);
    }

    private void MapViewToggle_performed(InputAction.CallbackContext obj)
    {
        OnMapViewToggle_performed?.Invoke(obj);
    }

    private void FirstViewToggle_performed(InputAction.CallbackContext obj)
    {
        OnFirstViewToggle_performed?.Invoke(obj);
    }

    private void Zoom_canceled(InputAction.CallbackContext obj)
    {
        OnZoom_canceled?.Invoke(obj);
    }

    private void Zoom_performed(InputAction.CallbackContext obj)
    {
        OnZoom_performed?.Invoke(obj);
    }

    private void Zoom_started(InputAction.CallbackContext obj)
    {
        OnZoom_started?.Invoke(obj);
    }

    private void JumpUp_performed(InputAction.CallbackContext obj)
    {
        OnJumpUp_performed?.Invoke(obj);
    }

    private void MouseRightClick_canceled(InputAction.CallbackContext obj)
    {
        OnMouseRightClick_canceled?.Invoke(obj);
    }

    private void MouseRightClick_started(InputAction.CallbackContext obj)
    {
        OnMouseRightClick_started?.Invoke(obj);
    }

    private void Acceleration_canceled(InputAction.CallbackContext obj)
    {
        OnAcceleration_canceled?.Invoke(obj);
    }

    private void Acceleration_started(InputAction.CallbackContext obj)
    {
        OnAcceleration_started?.Invoke(obj);
    }

    private void RotateKeyboard_canceled(InputAction.CallbackContext obj)
    {
        OnRotateKeyboard_canceled?.Invoke(obj);
    }

    private void RotateKeyboard_performed(InputAction.CallbackContext obj)
    {
        OnRotateKeyboard_performed?.Invoke(obj);
    }

    private void RotateKeyboard_started(InputAction.CallbackContext obj)
    {
        OnRotateKeyboard_started?.Invoke(obj);
    }

    private void RotateMouse_canceled(InputAction.CallbackContext obj)
    {
        OnRotateMouse_canceled?.Invoke(obj);
    }

    private void RotateMouse_performed(InputAction.CallbackContext obj)
    {
        OnRotateMouse_performed?.Invoke(obj);
    }

    private void RotateMouse_started(InputAction.CallbackContext obj)
    {
        OnRotateMouse_started?.Invoke(obj);
    }

    private void MoveForward_canceled(InputAction.CallbackContext obj)
    {
        OnMoveForward_canceled?.Invoke(obj);
    }

    private void MoveForward_performed(InputAction.CallbackContext obj)
    {
        OnMoveForward_performed?.Invoke(obj);
    }

    private void MoveForward_started(InputAction.CallbackContext obj)
    {
        OnMoveForward_started?.Invoke(obj);
    }

    private void MoveAside_started(InputAction.CallbackContext obj)
    {
        OnMoveAside_started?.Invoke(obj);
    }

    private void MoveAside_canceled(InputAction.CallbackContext obj)
    {
        OnMoveAside_canceled?.Invoke(obj);
    }

    private void Menu_performed(InputAction.CallbackContext obj)
    {
        OnMenu_performed?.Invoke(obj);
    }

    private void OnEnable()
    {
        gameInput.Enable();
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }

    private void OnDestroy()
    {
        gameInput.Game.Menu.performed -= Menu_performed;

        gameInput.Movement.MoveForward.performed -= MoveForward_performed;
        gameInput.Movement.MoveForwardButton.started -= MoveForward_started;
        gameInput.Movement.MoveForwardButton.canceled -= MoveForward_canceled;
        gameInput.Movement.MoveAsideButton.started -= MoveAside_started;
        gameInput.Movement.MoveAsideButton.canceled -= MoveAside_canceled;

        gameInput.Movement.RotateMouse.started -= RotateMouse_started;
        gameInput.Movement.RotateMouse.performed -= RotateMouse_performed;
        gameInput.Movement.RotateMouse.canceled -= RotateMouse_canceled;

        gameInput.Movement.RotateKeyboard.started -= RotateKeyboard_started;
        gameInput.Movement.RotateKeyboard.performed -= RotateKeyboard_performed;
        gameInput.Movement.RotateKeyboard.canceled -= RotateKeyboard_canceled;

        gameInput.Movement.Acceleration.started -= Acceleration_started;
        gameInput.Movement.Acceleration.canceled -= Acceleration_canceled;

        gameInput.Movement.MouseRightClick.started -= MouseRightClick_started;
        gameInput.Movement.MouseRightClick.canceled -= MouseRightClick_canceled;

        gameInput.Movement.JumpUp.performed -= JumpUp_performed;

        gameInput.Camera.Zoom.started -= Zoom_started;
        gameInput.Camera.Zoom.performed -= Zoom_performed;
        gameInput.Camera.Zoom.canceled -= Zoom_canceled;

        gameInput.Camera.FirstViewToggle.performed -= FirstViewToggle_performed;

        gameInput.Camera.MapViewToggle.performed -= MapViewToggle_performed;

        gameInput.Camera.StartRotation.started -= StartRotation_started;
        gameInput.Camera.StartRotation.canceled -= StartRotation_canceled;
    }
}
