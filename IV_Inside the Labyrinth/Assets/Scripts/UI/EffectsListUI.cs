using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class EffectsListUI : MonoBehaviour
{
    [SerializeField] private EffectsListController plEffects;

    private VisualElement effectsListScreen;
    private Label label_effectsList;

    const string k_effectsListScreen = "EffectsListScreen";
    const string k_label_effectsList = "Effects_Label";

    private void Awake()
    {
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
        effectsListScreen = rootElement.Q(k_effectsListScreen);
        label_effectsList = rootElement.Q<Label>(k_label_effectsList);
    }

    // Start is called before the first frame update
    void Start()
    {
        plEffects.OnEffectsChanged += UpdateEffects;
        UpdateEffects(plEffects.Effects);
    }

    private void UpdateEffects(Dictionary<int, Effect> effects)
    {
        label_effectsList.text = string.Empty;
        string res = string.Empty;

        foreach (KeyValuePair<int, Effect> effect in effects)
        {
            res += effect.Value.ToString() + "\n";
        }
        label_effectsList.text = res;
    }

    private void OnDestroy()
    {
        plEffects.OnEffectsChanged -= UpdateEffects;
    }
}
