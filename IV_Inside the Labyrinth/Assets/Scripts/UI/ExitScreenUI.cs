using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ExitScreenUI : MonoBehaviour
{
    private VisualElement ExitScreen;
    private Button QuitButton, StayButton;

    const string ExitScreen_Name = "ExitScreen";
    const string QuitButton_Name = "QuitButton";
    const string StayButton_Name = "StayButton";

    UIDocument gameScreen;

    private void Awake()
    {
        gameScreen = GetComponent<UIDocument>();
        VisualElement rootElement = gameScreen.rootVisualElement;
        ExitScreen = rootElement.Q(ExitScreen_Name);
        QuitButton = rootElement.Q<Button>(QuitButton_Name);
        StayButton = rootElement.Q<Button>(StayButton_Name);
    }

    private void Start()
    {
        QuitButton.RegisterCallback<ClickEvent>((_) => GameManager.Instance.QuitGame());
        StayButton.RegisterCallback<ClickEvent>((_) => GameManager.Instance.StayInGame());
        GameManager.instance.StateMachine.OnStateChanged += UpdateVisibility;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (GameManager.instance.StateMachine.CurrentState.Equals(GameManager.instance.StateMachine.exitState))
        {
            ExitScreen.style.display = DisplayStyle.Flex;
        }
        else
        {
            ExitScreen.style.display = DisplayStyle.None;
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.StateMachine.OnStateChanged -= UpdateVisibility;
    }

}
