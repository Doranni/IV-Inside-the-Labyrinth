using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DebugUI : MonoBehaviour
{
    [SerializeField] MovementController plMovement;
    [SerializeField] CameraController cameraController;

    private VisualElement debugScreen;
    private Label label_playerState, label_cameraState;
    private Toggle toggle_isPlGrounded;

    const string k_debugScreen = "DebugScreen";
    const string k_label_playerState = "PlState_Value_Label";
    const string k_label_cameraState = "CamState_Value_Label";
    const string k_toggle_isPlGrounded = "isPlGrounded_Toggle";

    private void Awake()
    {
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
        debugScreen = rootElement.Q(k_debugScreen);
        label_playerState = rootElement.Q<Label>(k_label_playerState);
        label_cameraState = rootElement.Q<Label>(k_label_cameraState);
        toggle_isPlGrounded = rootElement.Q<Toggle>(k_toggle_isPlGrounded);
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
        label_cameraState.text = cameraController.StateMachine.CurrentState.ToString();
    }

    private void Player_OnStateChanged()
    {
        label_playerState.text = plMovement.StateMachine.CurrentState.ToString();
        if (plMovement.StateMachine.CurrentState is GroundedState)
        {
            toggle_isPlGrounded.value = true;
        }
        else
        {
            toggle_isPlGrounded.value = false;
        }
    }

    private void OnDestroy()
    {
        plMovement.StateMachine.OnStateChanged -= Player_OnStateChanged;
        cameraController.StateMachine.OnStateChanged -= Camera_OnStateChanged;
        cameraController.StateMachine.thirdViewState.OnStateChanged -= Camera_OnStateChanged;
    }
}
