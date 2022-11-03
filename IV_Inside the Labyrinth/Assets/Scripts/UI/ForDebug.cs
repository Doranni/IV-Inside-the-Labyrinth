using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForDebug : MonoBehaviour
{
    [SerializeField] TMP_Text playerStateText, cameraStateText;
    [SerializeField] MovementController plMovement;
    [SerializeField] CameraController cameraController;

    private void Awake()
    {
        plMovement.StateMachine.OnStateChanged += Player_OnStateChanged;
        cameraController.StateMachine.OnStateChanged += Camera_OnStateChanged;
        cameraController.StateMachine.thirdViewState.OnStateChanged += Camera_OnStateChanged;
    }

    private void Start()
    {
        Player_OnStateChanged();
        Camera_OnStateChanged();
    }

    private void Camera_OnStateChanged()
    {
        cameraStateText.text = cameraController.StateMachine.CurrentState.ToString();
    }

    private void Player_OnStateChanged()
    {
        playerStateText.text = plMovement.StateMachine.CurrentState.ToString();
    }

    private void OnDestroy()
    {
        plMovement.StateMachine.OnStateChanged -= Player_OnStateChanged;
        cameraController.StateMachine.OnStateChanged -= Camera_OnStateChanged;
        cameraController.StateMachine.thirdViewState.OnStateChanged -= Camera_OnStateChanged;
    }
}
