using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HealthAndSanityUI : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private HealthController plHealth;
    private SanityController plSanity;

    private VisualElement healthAndSanityScreen;
    private Label label_health, label_sanity;

    const string k_healthAndSanityScreen = "HealthAndSanityScreen";
    const string k_label_health = "PlHealth_Value_Label";
    const string k_label_sanity = "PlSanity_Value_Label";

    private void Awake()
    {
        plHealth = player.GetComponent<HealthController>();
        plSanity = player.GetComponent<SanityController>();

        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
        healthAndSanityScreen = rootElement.Q(k_healthAndSanityScreen);
        label_health = rootElement.Q<Label>(k_label_health);
        label_sanity = rootElement.Q<Label>(k_label_sanity);
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
        label_health.text = Mathf.Round(values.currentHealth) + "/" + values.maxHealth;
    }

    private void UpdateSanity((float currentSanity, float maxSanity) values)
    {
        label_sanity.text = Mathf.Round(values.currentSanity) + "/" + values.maxSanity;
    }

    private void OnDestroy()
    {
        plHealth.OnChangeHealth -= UpdateHealth;
        plSanity.OnChangeSanity -= UpdateSanity;
        GameManager.instance.StateMachine.OnStateChanged -= UpdateVisibility;
    }
}
