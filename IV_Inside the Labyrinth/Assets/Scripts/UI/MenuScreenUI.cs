using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MenuScreenUI : MonoBehaviour
{
    private VisualElement menuScreen;
    private Button button_settings, button_exit;
    private Toggle toggle_isPaused;

    const string k_menuScreen = "MenuScreen";
    const string k_button_settings = "SettingsButton";
    const string k_button_exit = "ExitButton";
    const string k_toggle_isPaused = "Pause_Toggle";

    private void Awake()
    {
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
        menuScreen = rootElement.Q(k_menuScreen);
        button_settings = rootElement.Q<Button>(k_button_settings);
        button_exit = rootElement.Q<Button>(k_button_exit);
        toggle_isPaused = rootElement.Q<Toggle>(k_toggle_isPaused);
    }

    void Start()
    {
        button_settings.RegisterCallback<ClickEvent>((_) => GameManager.Instance.OpenSettingsScreen());
        button_exit.RegisterCallback<ClickEvent>((_) => GameManager.Instance.OpenExitScreen());
        toggle_isPaused.RegisterValueChangedCallback(SetPauseBehavior);
        GameManager.instance.StateMachine.OnStateChanged += UpdateVisibility;
        Preferences.OnPauseBehaviorChanged += UpdatePauseUI;
        UpdatePauseUI();
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (GameManager.instance.StateMachine.CurrentState.Equals(GameManager.instance.StateMachine.menuState))
        {
            menuScreen.style.display = DisplayStyle.Flex;
        }
        else
        {
            menuScreen.style.display = DisplayStyle.None;
        }
    }

    private void SetPauseBehavior(ChangeEvent<bool> evt)
    {
        Preferences.IsPausedWhileInMenu = evt.newValue;
    }

    private void UpdatePauseUI()
    {
        toggle_isPaused.value = Preferences.IsPausedWhileInMenu;
    }

    private void OnDestroy()
    {
        GameManager.instance.StateMachine.OnStateChanged -= UpdateVisibility;
        Preferences.OnPauseBehaviorChanged -= UpdatePauseUI;
    }
}
