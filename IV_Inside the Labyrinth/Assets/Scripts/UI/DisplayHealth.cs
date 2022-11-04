using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHealth : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthValueTmp, sanityValueTmp, effectsTmp;
    [SerializeField] private Image damageEffect_Image;
    [SerializeField] private float damageEffectImage_maxOpacity;
    [SerializeField] private Animator damageAnimator;
    [SerializeField] private GameObject sanityLightEffect;

    private HealthController plHealth;
    private SanityController plSanity;
    private EffectsListController plEffects;
    private SanityLightRecoveryController plSanityRestore;

    private int animHash_Damage_Trigger;

    private void Start()
    {
        plHealth = GetComponent<HealthController>();
        plSanity = GetComponent<SanityController>();
        plEffects = GetComponent<EffectsListController>();
        plSanityRestore = GetComponent<SanityLightRecoveryController>();

        plHealth.OnChangeHealth += PlHealth_OnChangeHealth;
        plHealth.OnGetDamage += PlHealth_OnGetDamage;
        plSanity.OnChangeSanity += PlSanity_OnChangeSanity;
        plEffects.OnEffectsChanged += UpdateEffects;
        plSanityRestore.OnSanityLightEnter += PlSanityRestore_OnSanityLightEnter;
        plSanityRestore.OnSanityLightExit += PlSanityRestore_OnSanityLightExit;

        PlHealth_OnChangeHealth((plHealth.Health, plHealth.MaxHealth));
        PlSanity_OnChangeSanity((plSanity.Sanity, plSanity.MaxSanity));
        UpdateEffects(plEffects.Effects);
        PlSanityRestore_OnSanityLightExit();

        animHash_Damage_Trigger = Animator.StringToHash("damage_trigger");
    }

    private void UpdateEffects(Dictionary<int, Effect> effects)
    {
        effectsTmp.text = string.Empty;
        string res = string.Empty;

        foreach (KeyValuePair<int, Effect> effect in effects)
        {
            res += effect.Value.ToString() + "\n";
        }
        effectsTmp.SetText(res);
    }

    private void PlHealth_OnChangeHealth((float currentHealth, float maxHealth) values)
    {
        healthValueTmp.text = Mathf.Round(values.currentHealth) + "/" + values.maxHealth;
        float damageColor_A = Mathf.Lerp(damageEffectImage_maxOpacity, 0, values.currentHealth / values.maxHealth);
        Color bloodColor = damageEffect_Image.color;
        bloodColor.a = damageColor_A;
        damageEffect_Image.color = bloodColor;
    }

    private void PlSanity_OnChangeSanity((float currentSanity, float maxSanity) values)
    {
        sanityValueTmp.text = Mathf.Round(values.currentSanity) + "/" + values.maxSanity;
    }

    private void PlHealth_OnGetDamage((float currentHealth, float maxHealth) values)
    {
        damageAnimator.SetTrigger(animHash_Damage_Trigger);
    }

    private void PlSanityRestore_OnSanityLightExit()
    {
        sanityLightEffect.SetActive(false);
    }

    private void PlSanityRestore_OnSanityLightEnter()
    {
        sanityLightEffect.SetActive(true);
    }

    private void OnDestroy()
    {
        plHealth.OnChangeHealth -= PlHealth_OnChangeHealth;
        plHealth.OnGetDamage -= PlHealth_OnGetDamage;
        plSanity.OnChangeSanity -= PlSanity_OnChangeSanity;
        plEffects.OnEffectsChanged -= UpdateEffects;
        plSanityRestore.OnSanityLightEnter -= PlSanityRestore_OnSanityLightEnter;
        plSanityRestore.OnSanityLightExit -= PlSanityRestore_OnSanityLightExit;
    }
}
