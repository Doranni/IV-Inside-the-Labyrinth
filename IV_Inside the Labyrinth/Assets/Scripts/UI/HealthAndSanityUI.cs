using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HealthAndSanityUI : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private HealthController plHealth;
    private SanityController plSanity;

    private VisualElement healthAndSanityScreen;
    private Label healthLabel, sanityLabel;

    const string healthAndSanityScreen_Name = "HealthAndSanityScreen";
    const string healthLabel_Name = "PlHealth_Value_Label";
    const string sanityLabel_Name = "PlSanity_Value_Label";

    UIDocument gameScreen;

    private void Awake()
    {
        plHealth = player.GetComponent<HealthController>();
        plSanity = player.GetComponent<SanityController>();
        gameScreen = GetComponent<UIDocument>();

        VisualElement rootElement = gameScreen.rootVisualElement;
        healthAndSanityScreen = rootElement.Q(healthAndSanityScreen_Name);
        healthLabel = rootElement.Q<Label>(healthLabel_Name);
        sanityLabel = rootElement.Q<Label>(sanityLabel_Name);
    }

    private void UpdateVisibility()
    {
        if (GameManager.instance.StateMachine.CurrentState.Equals(GameManager.instance.StateMachine.activeState))
        {
            healthAndSanityScreen.style.display = DisplayStyle.Flex;
        }
        else
        {
            healthAndSanityScreen.style.display = DisplayStyle.None;
        }
    }

    private void Start()
    {
        plHealth.OnChangeHealth += UpdateHealth;
        plSanity.OnChangeSanity += UpdateSanity;
        GameManager.instance.StateMachine.OnStateChanged += UpdateVisibility;

        UpdateHealth((plHealth.Health, plHealth.MaxHealth));
        UpdateSanity((plSanity.Sanity, plSanity.MaxSanity));
        UpdateVisibility();
    }

    private void UpdateHealth((float currentHealth, float maxHealth) values)
    {
        healthLabel.text = Mathf.Round(values.currentHealth) + "/" + values.maxHealth;
    }

    private void UpdateSanity((float currentSanity, float maxSanity) values)
    {
        sanityLabel.text = Mathf.Round(values.currentSanity) + "/" + values.maxSanity;
    }

    private void OnDestroy()
    {
        plHealth.OnChangeHealth -= UpdateHealth;
        plSanity.OnChangeSanity -= UpdateSanity;
        GameManager.instance.StateMachine.OnStateChanged -= UpdateVisibility;
    }
}
