using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ExitScreenUI : MonoBehaviour
{
    private VisualElement exitScreen;
    private Button quitButton, stayButton;

    const string exitScreen_Name = "ExitScreen";
    const string quitButton_Name = "QuitButton";
    const string stayButton_Name = "StayButton";

    UIDocument gameScreen;

    private void Awake()
    {
        gameScreen = GetComponent<UIDocument>();
        VisualElement rootElement = gameScreen.rootVisualElement;
        exitScreen = rootElement.Q(exitScreen_Name);
        quitButton = rootElement.Q<Button>(quitButton_Name);
        stayButton = rootElement.Q<Button>(stayButton_Name);
    }

    private void Start()
    {
        quitButton.RegisterCallback<ClickEvent>((_) => GameManager.Instance.QuitGame());
        stayButton.RegisterCallback<ClickEvent>((_) => GameManager.Instance.StayInGame());
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
