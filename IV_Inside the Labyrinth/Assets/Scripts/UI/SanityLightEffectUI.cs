using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SanityLightEffectUI : MonoBehaviour
{
    [SerializeField] private SanityLightRecoveryController plSanityRestore;

    private VisualElement SanityLightEffectScreen;
    const string SanityLightEffectScreen_Name = "SanityLightEffectScreen";

    UIDocument gameScreen;


    private void Awake()
    {
        gameScreen = GetComponent<UIDocument>();

        VisualElement rootElement = gameScreen.rootVisualElement;
        SanityLightEffectScreen = rootElement.Q(SanityLightEffectScreen_Name);
    }

    void Start()
    {
        plSanityRestore.OnSanityLightEnter += PlSanityRestore_OnSanityLightEnter;
        plSanityRestore.OnSanityLightExit += PlSanityRestore_OnSanityLightExit;
        PlSanityRestore_OnSanityLightExit();
    }

    private void PlSanityRestore_OnSanityLightExit()
    {
        SanityLightEffectScreen.style.display = DisplayStyle.None;
    }

    private void PlSanityRestore_OnSanityLightEnter()
    {
        SanityLightEffectScreen.style.display = DisplayStyle.Flex;
    }

    private void OnDestroy()
    {
        plSanityRestore.OnSanityLightEnter -= PlSanityRestore_OnSanityLightEnter;
        plSanityRestore.OnSanityLightExit -= PlSanityRestore_OnSanityLightExit;
    }
}
