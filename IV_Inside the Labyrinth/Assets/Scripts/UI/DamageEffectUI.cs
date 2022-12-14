using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DamageEffectUI : MonoBehaviour
{
    [SerializeField] private HealthController plHealth;
    [SerializeField] private float damageEffectImage_maxOpacity, darkeningTimeStep;
    [SerializeField] private AudioClip damageClip;

    private AudioSource audioSource;

    private VisualElement damageEffectScreen;
    private IMGUIContainer darkeningEffect;

    const string k_damageEffectScreen = "DamageEffectScreen";
    const string k_darkeningEffect = "Darkening";

    const string k_uiAnim_darkening = "damage_effect-triggered";

    private bool isDamageRoutineRunning = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
        damageEffectScreen = rootElement.Q(k_damageEffectScreen);
        darkeningEffect = rootElement.Q<IMGUIContainer>(k_darkeningEffect);
    }

    private void Start()
    {
        plHealth.OnChangeHealth += PlHealth_OnChangeHealth;
        plHealth.OnGetDamage += PlHealth_OnGetDamage;

        PlHealth_OnChangeHealth((plHealth.Health, plHealth.MaxHealth));
    }

    private void PlHealth_OnChangeHealth((float currentHealth, float maxHealth) values)
    {
        float damageColor_A = Mathf.Lerp(damageEffectImage_maxOpacity, 0, values.currentHealth / values.maxHealth);
        Color bloodColor = damageEffectScreen.style.unityBackgroundImageTintColor.value;
        bloodColor.a = damageColor_A;
        damageEffectScreen.style.unityBackgroundImageTintColor = bloodColor;
    }

    private void PlHealth_OnGetDamage((float currentHealth, float maxHealth) values)
    {
        if (!isDamageRoutineRunning)
        {
            StartCoroutine(DamageAnimationRoutine());
        }
    }

    private IEnumerator DamageAnimationRoutine()
    {
        isDamageRoutineRunning = true;
        darkeningEffect.AddToClassList(k_uiAnim_darkening);
        audioSource.PlayOneShot(damageClip);
        yield return new WaitForSeconds(darkeningTimeStep);
        darkeningEffect.RemoveFromClassList(k_uiAnim_darkening);
        yield return new WaitForSeconds(darkeningTimeStep);
        isDamageRoutineRunning = false;
    }

    private void OnDestroy()
    {
        plHealth.OnChangeHealth -= PlHealth_OnChangeHealth;
        plHealth.OnGetDamage -= PlHealth_OnGetDamage;
    }
}
