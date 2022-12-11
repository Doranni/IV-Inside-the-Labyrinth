using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ExitScreenUI : MonoBehaviour
{
    private VisualElement exitScreen;
    private Button button_quit, button_stay;

    const string k_exitScreen = "ExitScreen";
    const string k_button_quit = "QuitButton";
    const string k_button_stay = "StayButton";

    private void Awake()
    {
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
        exitScreen = rootElement.Q(k_exitScreen);
        button_quit = rootElement.Q<Button>(k_button_quit);
        button_stay = rootElement.Q<Button>(k_button_stay);
    }

    private void Start()
    {
        button_quit.RegisterCallback<ClickEvent>((_) => GameManager.Instance.QuitGame());
        button_stay.RegisterCallback<ClickEvent>((_) => GameManager.Instance.StayInGame());
        GameManager.instance.StateMachine.OnStateChanged += UpdateVisibility;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (GameManager.instance.StateMachine.CurrentState.Equals(GameManager.instance.StateMachine.exitState))
        {
            exitScreen.style.display = DisplayStyle.Flex;
        }
        else
        {
            exitScreen.style.display = DisplayStyle.None;
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.StateMachine.OnStateChanged -= UpdateVisibility;
    }

}
