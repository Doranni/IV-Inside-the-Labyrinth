using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private HealthController plHealth;
    private SanityController plSanity;
    private AttackController plAttack;
    private AnimationAndMovementController plMovement;
    private EffectsListController plEffects;

    private void Start()
    {  
        plHealth = GetComponent<HealthController>();
        plSanity = GetComponent<SanityController>();
        plAttack = GetComponent<AttackController>();
        plMovement = GetComponent<AnimationAndMovementController>();
        plEffects = GetComponent<EffectsListController>();
        
        plHealth.OnChangeHealth += PlHealth_OnChangeHealth;
        plSanity.OnChangeSanity += PlSanity_OnChangeSanity;
        plHealth.OnEffectStarted += PlHealth_OnEffectStarted;
        plHealth.OnEffectPerformed += PlHealth_OnEffectPerformed;
        plHealth.OnEffectEnded += PlHealth_OnEffectEnded;
        plSanity.OnEffectStarted += PlSanity_OnEffectStarted;
        plSanity.OnEffectPerformed += PlSanity_OnEffectPerformed;
        plSanity.OnEffectEnded += PlSanity_OnEffectEnded;

        plHealth.AddEffectListController(plEffects);
        plSanity.AddEffectListController(plEffects);
        plEffects.OnEffectsChanged += GameManager.UpdateEffects;

        GameManager.UpdateHealth(plHealth.Health, plHealth.MaxHealth);
        GameManager.UpdateSanity(plSanity.Sanity, plSanity.MaxSanity);
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

    private void PlSanity_OnChangeSanity(float value, float maxValue)
    {
        GameManager.UpdateSanity(value, maxValue);
    }

    private void PlHealth_OnChangeHealth(float value, float maxValue)
    {
        GameManager.UpdateHealth(value, maxValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameManager.trapTag))
        {
            TrapController trap = other.gameObject.GetComponent<TrapController>();
            if (trap.IsCharged)
            {
                Debug.Log("Player OnTriggerEnter with active trap");
                trap.Activate(plAttack.PassiveDamage);
                plHealth.ChangeHealth(-trap.HealthDamage_Immediate);
                plSanity.ChangeSanity(-trap.SanityDamage_Immediate);
                foreach(Effect effect in trap.Effects)
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
        }
    }
}
