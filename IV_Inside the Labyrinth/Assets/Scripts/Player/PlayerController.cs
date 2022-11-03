using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Image damageEffect_Image;
    [SerializeField] private float damageEffectImage_maxOpacity;
    [SerializeField] private Animator damageAnimator;
    [SerializeField] private GameObject sanityLightEffect;
    [SerializeField] private LayerMask sanityLightMask;

    private int animHash_Damage_Trigger, animHash_HealthLeft_Float;

    private HealthController plHealth;
    private SanityController plSanity;
    private AttackController plAttack;
    private MovementController plMovement;
    private EffectsListController plEffects;
    private SanityRestoreController plSanityRestore;

    private void Start()
    {  
        plHealth = GetComponent<HealthController>();
        plSanity = GetComponent<SanityController>();
        plAttack = GetComponent<AttackController>();
        plMovement = GetComponentInParent<MovementController>();
        plEffects = GetComponent<EffectsListController>();
        plSanityRestore = GetComponent<SanityRestoreController>();

        plHealth.OnChangeHealth += PlHealth_OnChangeHealth;
        plHealth.OnGetDamage += PlHealth_OnGetDamage;
        plSanity.OnChangeSanity += PlSanity_OnChangeSanity;
        plHealth.OnEffectStarted += PlHealth_OnEffectStarted;
        plHealth.OnEffectPerformed += PlHealth_OnEffectPerformed;
        plHealth.OnEffectEnded += PlHealth_OnEffectEnded;
        plSanity.OnEffectStarted += PlSanity_OnEffectStarted;
        plSanity.OnEffectPerformed += PlSanity_OnEffectPerformed;
        plSanity.OnEffectEnded += PlSanity_OnEffectEnded;
        plEffects.OnEffectsChanged += GameManager.UpdateEffects;
        plSanityRestore.OnSanityLightEnter += PlSanityRestore_OnSanityLightEnter;
        plSanityRestore.OnSanityLightExit += PlSanityRestore_OnSanityLightExit;

        PlHealth_OnChangeHealth(plHealth.Health, plHealth.MaxHealth);
        PlSanity_OnChangeSanity(plSanity.Sanity, plSanity.MaxSanity);
        PlSanityRestore_OnSanityLightExit();

        animHash_Damage_Trigger = Animator.StringToHash("damage_trigger");
    }

    private void PlHealth_OnGetDamage(float currentHealth, float maxHealth)
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

    private void PlSanity_OnEffectEnded()
    {
        
    }

    private void PlSanity_OnEffectPerformed(float effectValue, float time)
    {
        
    }

    private void PlSanity_OnEffectStarted(float effectValue, float time)
    {
        
    }

    private void PlHealth_OnEffectEnded()
    {
        
    }

    private void PlHealth_OnEffectPerformed(float effectValue, float time)
    {
        
    }

    private void PlHealth_OnEffectStarted(float effectValue, float time)
    {
        
    }

    private void PlSanity_OnChangeSanity(float currentSanity, float maxSanity)
    {
        GameManager.UpdateSanity(currentSanity, maxSanity);
        LightManager.UpdateLight(currentSanity / maxSanity);
    }

    private void PlHealth_OnChangeHealth(float currentHealth, float maxHealth)
    {
        GameManager.UpdateHealth(currentHealth, maxHealth);
        float damageColor_A = Mathf.Lerp(damageEffectImage_maxOpacity, 0, currentHealth / maxHealth);
        Color bloodColor = damageEffect_Image.color;
        bloodColor.a = damageColor_A;
        damageEffect_Image.color = bloodColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameManager.trapTag))
        {
            TrapController trap = other.gameObject.GetComponent<TrapController>();
            if (trap.IsCharged)
            {
                trap.Activate(plAttack.PassiveDamage);
                plHealth.ChangeHealth(-trap.HealthDamage_Immediate);
                plSanity.ChangeSanity(-trap.SanityDamage_Immediate);
                foreach(Effect effect in trap.Effects)
                {
                    AddEffect(effect);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameManager.sanityLight))
        {
            plSanityRestore.EnterSanityLight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameManager.sanityLight))
        {
            plSanityRestore.ExitSanityLight();
        }
    }

    private void AddEffect(Effect effect)
    {
        switch (effect.type)
        {
            case Effect.EffectType.health:
                {
                    plHealth.AddEffect(effect);
                    break;
                }
            case Effect.EffectType.sanity:
                {
                    plSanity.AddEffect(effect);
                    break;
                }
            case Effect.EffectType.movement:
                {
                    plMovement.AddEffect(effect);
                    break;
                }
        }
    }
}
