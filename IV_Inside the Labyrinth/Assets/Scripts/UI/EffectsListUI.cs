using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class EffectsListUI : MonoBehaviour
{
    [SerializeField] private EffectsListController plEffects;

    private VisualElement effectsListScreen;
    private Label effectsListLabel;

    const string effectsListScreen_Name = "EffectsListScreen";
    const string effectsListLabel_Name = "Effects_Label";

    UIDocument gameScreen;

    private void Awake()
    {
        gameScreen = GetComponent<UIDocument>();
        VisualElement rootElement = gameScreen.rootVisualElement;
        effectsListScreen = rootElement.Q(effectsListScreen_Name);
        effectsListLabel = rootElement.Q<Label>(effectsListLabel_Name);
    }

    // Start is called before the first frame update
    void Start()
    {
        plEffects.OnEffectsChanged += UpdateEffects;
        UpdateEffects(plEffects.Effects);
    }

    private void UpdateEffects(Dictionary<int, Effect> effects)
    {
        effectsListLabel.text = string.Empty;
        string res = string.Empty;

        foreach (KeyValuePair<int, Effect> effect in effects)
        {
            res += effect.Value.ToString() + "\n";
        }
        effectsListLabel.text = res;
    }

    private void OnDestroy()
    {
        plEffects.OnEffectsChanged -= UpdateEffects;
    }
}
