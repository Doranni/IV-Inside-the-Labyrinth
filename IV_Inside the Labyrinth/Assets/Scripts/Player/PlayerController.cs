using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private HealthController plHealth;
    private SanityController plSanity;
    private AttackController plAttack;
    private MovementController plMovement;
    private SanityLightRecoveryController plSanityRestore;

    private void Awake()
    {  
        plHealth = GetComponent<HealthController>();
        plSanity = GetComponent<SanityController>();
        plAttack = GetComponent<AttackController>();
        plMovement = GetComponentInParent<MovementController>();
        plSanityRestore = GetComponent<SanityLightRecoveryController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enter the Trap
        if (other.CompareTag(GameManager.tag_trap))
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
        // Enter Sanity Light
        if (other.CompareTag(GameManager.tag_sanityLight))
        {
            plSanityRestore.EnterSanityLight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Exit Sanity Light
        if (other.CompareTag(GameManager.tag_sanityLight))
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
