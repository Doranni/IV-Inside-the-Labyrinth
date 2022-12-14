using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float health;

    public float Health => health;
    public float MaxHealth => maxHealth;

    private EffectsListController effectsListController;
    readonly Dictionary<int, Effect> effects = new Dictionary<int, Effect>();
    private readonly Dictionary<int, Coroutine> effectCoroutines = new Dictionary<int, Coroutine>();

    private HealingController healingController;

    public event Action<(float currentHealth, float maxHealth)> OnChangeHealth, OnGetDamage;
    public event Action OnDeath;
    public event Action<(float effectValue, float timeLeft)> OnEffectStarted, OnEffectPerformed;
    public event Action OnEffectEnded;

    private void Awake()
    {
        health = maxHealth;
        TryGetComponent(out effectsListController);
        TryGetComponent(out healingController);
    }

    private IEnumerator EffectRoutine(int id)
    {
        while (effects[id].TimeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            ChangeHealth(effects[id].effectValue);
            effects[id].ChangeTimeLeft(-1);
            if (effectsListController != null)
            {
                effectsListController.UpdateEffects();
            }
            OnEffectPerformed?.Invoke((effects[id].effectValue, effects[id].TimeLeft));
        }
        OnEffectEnded?.Invoke();
        if (effectsListController != null)
        {
            effectsListController.RemoveEffect(id);
        }
        effects.Remove(id);
        StopCoroutine(effectCoroutines[id]);
        effectCoroutines.Remove(id);
    }

    public bool AddEffect(Effect effect)
    {
        Effect newEffect = new Effect(effect);
        if (effect.type != Effect.EffectType.health)
        {
            return false;
        }
        OnEffectStarted?.Invoke((newEffect.effectValue, newEffect.timeTotal));
        int id = GameManager.NewId();
        if (effectsListController != null)
        {
            id = effectsListController.Add(newEffect, id);
        }
        effects.Add(id, newEffect);
        effectCoroutines.Add(id, StartCoroutine(EffectRoutine(id)));
        return true;
    }

    public void ChangeHealth(float value)
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);
        if (value < 0)
        {
            OnGetDamage?.Invoke((health, maxHealth));
            if (healingController != null)
            {
                healingController.RegisterDamage();
            }
        }
        OnChangeHealth?.Invoke((health, maxHealth));
        if (health == 0)
        {
            Death();
        }
    }

    public void Death()
    {
        OnDeath?.Invoke();
    }
}
