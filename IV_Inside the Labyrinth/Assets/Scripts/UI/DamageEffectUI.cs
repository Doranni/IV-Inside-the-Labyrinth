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
    UIDocument gameScreen;

    private VisualElement damageEffectScreen;
    private IMGUIContainer darkeningEffect;

    const string damageEffectScreen_Name = "DamageEffectScreen";
    const string darkeningEffect_Name = "Darkening";

    const string darkeningAnimation_Name = "damage_effect-triggered";

    private bool isDamageRoutineRunning = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameScreen = GetComponent<UIDocument>();
        VisualElement rootElement = gameScreen.rootVisualElement;
        damageEffectScreen = rootElement.Q(damageEffectScreen_Name);
        darkeningEffect = rootElement.Q<IMGUIContainer>(darkeningEffect_Name);
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
        darkeningEffect.AddToClassList(darkeningAnimation_Name);
        audioSource.PlayOneShot(damageClip);
        yield return new WaitForSeconds(darkeningTimeStep);
        darkeningEffect.RemoveFromClassList(darkeningAnimation_Name);
        yield return new WaitForSeconds(darkeningTimeStep);
        isDamageRoutineRunning = false;
    }

    private void OnDestroy()
    {
        plHealth.OnChangeHealth -= PlHealth_OnChangeHealth;
        plHealth.OnGetDamage -= PlHealth_OnGetDamage;
    }
}
