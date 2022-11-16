using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DebugUI : MonoBehaviour
{
    [SerializeField] MovementController plMovement;
    [SerializeField] CameraController cameraController;

    private VisualElement debugScreen;
    private Label playerStateLabel, cameraStateLabel;
    private Toggle isPlGroundedToggle;

    const string debugScreen_Name = "DebugScreen";
    const string plStateLabel_Name = "PlState_Value_Label";
    const string camStateLabel_Name = "CamState_Value_Label";
    const string isPlGroundedToggle_Name = "isPlGrounded_Toggle";

    UIDocument gameScreen;

    private void Awake()
    {
        gameScreen = GetComponent<UIDocument>();
        VisualElement rootElement = gameScreen.rootVisualElement;
        debugScreen = rootElement.Q(debugScreen_Name);
        playerStateLabel = rootElement.Q<Label>(plStateLabel_Name);
        cameraStateLabel = rootElement.Q<Label>(camStateLabel_Name);
        isPlGroundedToggle = rootElement.Q<Toggle>(isPlGroundedToggle_Name);
    }

    private void Start()
    {
        plMovement.StateMachine.OnStateChanged += Player_OnStateChanged;
        cameraController.StateMachine.OnStateChanged += Camera_OnStateChanged;
        cameraController.StateMachine.thirdViewState.OnStateChanged += Camera_OnStateChanged;
        Player_OnStateChanged();
        Camera_OnStateChanged();
    }

    private void Camera_OnStateChanged()
    {
        cameraStateLabel.text = cameraController.StateMachine.CurrentState.ToString();
    }

    private void Player_OnStateChanged()
    {
        playerStateLabel.text = plMovement.StateMachine.CurrentState.ToString();
        if (plMovement.StateMachine.CurrentState is GroundedState)
        {
            isPlGroundedToggle.value = true;
        }
        else
        {
            isPlGroundedToggle.value = false;
        }
    }

    private void OnDestroy()
    {
        plMovement.StateMachine.OnStateChanged -= Player_OnStateChanged;
        cameraController.StateMachine.OnStateChanged -= Camera_OnStateChanged;
        cameraController.StateMachine.thirdViewState.OnStateChanged -= Camera_OnStateChanged;
    }
}
