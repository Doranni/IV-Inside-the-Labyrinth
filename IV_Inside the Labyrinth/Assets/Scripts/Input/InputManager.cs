using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    private GameInput gameInput;
    public static InputManager instance;

    public delegate void InputFunction(InputAction.CallbackContext obj);
    public event InputFunction OnMenu_performed,
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

    private void Awake()
    {
        instance = this;
        gameInput = new GameInput();

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
        if (OnStartRotation_canceled != null)
        {
            OnStartRotation_canceled(obj);
        }
    }

    private void StartRotation_started(InputAction.CallbackContext obj)
    {
        if (OnStartRotation_started != null)
        {
            OnStartRotation_started(obj);
        }
    }

    private void MapViewToggle_performed(InputAction.CallbackContext obj)
    {
        if (OnMapViewToggle_performed != null)
        {
            OnMapViewToggle_performed(obj);
        }
    }

    private void FirstViewToggle_performed(InputAction.CallbackContext obj)
    {
        if (OnFirstViewToggle_performed != null)
        {
            OnFirstViewToggle_performed(obj);
        }
    }

    private void Zoom_canceled(InputAction.CallbackContext obj)
    {
        if (OnZoom_canceled != null)
        {
            OnZoom_canceled(obj);
        }
    }

    private void Zoom_performed(InputAction.CallbackContext obj)
    {
        if (OnZoom_performed != null)
        {
            OnZoom_performed(obj);
        }
    }

    private void Zoom_started(InputAction.CallbackContext obj)
    {
        if (OnZoom_started != null)
        {
            OnZoom_started(obj);
        }
    }

    private void JumpUp_performed(InputAction.CallbackContext obj)
    {
        if (OnJumpUp_performed != null)
        {
            OnJumpUp_performed(obj);
        }
    }

    private void MouseRightClick_canceled(InputAction.CallbackContext obj)
    {
        if (OnMouseRightClick_canceled != null)
        {
            OnMouseRightClick_canceled(obj);
        }
    }

    private void MouseRightClick_started(InputAction.CallbackContext obj)
    {
        if (OnMouseRightClick_started != null)
        {
            OnMouseRightClick_started(obj);
        }
    }

    private void Acceleration_canceled(InputAction.CallbackContext obj)
    {
        if (OnAcceleration_canceled != null)
        {
            OnAcceleration_canceled(obj);
        }
    }

    private void Acceleration_started(InputAction.CallbackContext obj)
    {
        if (OnAcceleration_started != null)
        {
            OnAcceleration_started(obj);
        }
    }

    private void RotateKeyboard_canceled(InputAction.CallbackContext obj)
    {
        if (OnRotateKeyboard_canceled != null)
        {
            OnRotateKeyboard_canceled(obj);
        }
    }

    private void RotateKeyboard_performed(InputAction.CallbackContext obj)
    {
        if (OnRotateKeyboard_performed != null)
        {
            OnRotateKeyboard_performed(obj);
        }
    }

    private void RotateKeyboard_started(InputAction.CallbackContext obj)
    {
        if (OnRotateKeyboard_started != null)
        {
            OnRotateKeyboard_started(obj);
        }
    }

    private void RotateMouse_canceled(InputAction.CallbackContext obj)
    {
        if (OnRotateMouse_canceled != null)
        {
            OnRotateMouse_canceled(obj);
        }
    }

    private void RotateMouse_performed(InputAction.CallbackContext obj)
    {
        if (OnRotateMouse_performed != null)
        {
            OnRotateMouse_performed(obj);
        }
    }

    private void RotateMouse_started(InputAction.CallbackContext obj)
    {
        if (OnRotateMouse_started != null)
        {
            OnRotateMouse_started(obj);
        }
    }

    private void MoveForward_canceled(InputAction.CallbackContext obj)
    {
        if (OnMoveForward_canceled != null)
        {
            OnMoveForward_canceled(obj);
        }
    }

    private void MoveForward_performed(InputAction.CallbackContext obj)
    {
        if (OnMoveForward_performed != null)
        {
            OnMoveForward_performed(obj);
        }
    }

    private void MoveForward_started(InputAction.CallbackContext obj)
    {
        if (OnMoveForward_started != null)
        {
            OnMoveForward_started(obj);
        }
    }

    private void MoveAside_started(InputAction.CallbackContext obj)
    {
        if (OnMoveAside_started != null)
        {
            OnMoveAside_started(obj);
        }
    }

    private void MoveAside_canceled(InputAction.CallbackContext obj)
    {
        if (OnMoveAside_canceled != null)
        {
            OnMoveAside_canceled(obj);
        }
    }

    private void Menu_performed(InputAction.CallbackContext obj)
    {
        if (OnMenu_performed != null)
        {
            OnMenu_performed(obj);
        }
    }

    private void OnEnable()
    {
        gameInput.Enable();
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }
}
