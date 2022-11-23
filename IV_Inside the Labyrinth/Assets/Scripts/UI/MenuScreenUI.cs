using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MenuScreenUI : MonoBehaviour
{
    private VisualElement MenuScreen;
    private Button SettingsButton, ExitButton;
    private Toggle isPausedToggle;

    const string MenuScreen_Name = "MenuScreen";
    const string SettingsButton_Name = "SettingsButton";
    const string ExitButton_Name = "ExitButton";
    const string isPausedToggle_Name = "Pause_Toggle";

    UIDocument gameScreen;

    private void Awake()
    {
        gameScreen = GetComponent<UIDocument>();
        VisualElement rootElement = gameScreen.rootVisualElement;
        MenuScreen = rootElement.Q(MenuScreen_Name);
        SettingsButton = rootElement.Q<Button>(SettingsButton_Name);
        ExitButton = rootElement.Q<Button>(ExitButton_Name);
        isPausedToggle = rootElement.Q<Toggle>(isPausedToggle_Name);
    }

    void Start()
    {
        SettingsButton.RegisterCallback<ClickEvent>((_) => GameManager.Instance.OpenSettingsScreen());
        ExitButton.RegisterCallback<ClickEvent>((_) => GameManager.Instance.OpenExitScreen());
        isPausedToggle.RegisterValueChangedCallback(SetPauseBehavior);
        GameManager.instance.StateMachine.OnStateChanged += UpdateVisibility;
        Preferences.OnPauseBehaviorChanged += UpdatePauseUI;
        UpdatePauseUI();
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (GameManager.instance.StateMachine.CurrentState.Equals(GameManager.instance.StateMachine.menuState))
        {
            MenuScreen.style.display = DisplayStyle.Flex;
        }
        else
        {
            MenuScreen.style.display = DisplayStyle.None;
        }
    }

    private void SetPauseBehavior(ChangeEvent<bool> evt)
    {
        Preferences.IsPausedWhileInMenu = evt.newValue;
    }

    private void UpdatePauseUI()
    {
        isPausedToggle.value = Preferences.IsPausedWhileInMenu;
    }

    private void OnDestroy()
    {
        GameManager.instance.StateMachine.OnStateChanged -= UpdateVisibility;
        Preferences.OnPauseBehaviorChanged -= UpdatePauseUI;
    }
}
