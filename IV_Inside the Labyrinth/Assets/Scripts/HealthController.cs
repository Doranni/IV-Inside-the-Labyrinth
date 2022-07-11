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
    Dictionary<int, Effect> effects = new Dictionary<int, Effect>();
    private Dictionary<int, Coroutine> effectCoroutines = new Dictionary<int, Coroutine>();
    private int lastId = 0;

    public delegate void Delegate();
    public delegate void ChangeHealthDelegate(float value, float maxValue);
    public delegate void EffectDelegate(float effectValue, float time);

    public event ChangeHealthDelegate OnChangeHealth;
    public event Delegate OnDeath;
    public event EffectDelegate OnEffectStarted, OnEffectPerformed;
    public event Delegate OnEffectEnded;

    private void Awake()
    {
        health = maxHealth;
    }

    public void AddEffectListController(EffectsListController controller)
    {
        effectsListController = controller;
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
            if (OnEffectPerformed != null)
            {
                OnEffectPerformed(effects[id].effectValue, effects[id].TimeLeft);
            }
        }
        if (OnEffectEnded != null)
        {
            OnEffectEnded();
        }
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
        if (OnEffectStarted != null)
        {
            OnEffectStarted(newEffect.effectValue, newEffect.timeTotal);
        }
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
        if (OnChangeHealth != null)
        {
            OnChangeHealth(health, maxHealth);
        }
        if (health == 0)
        {
            Death();
        }
    }

    public void Death()
    {
        if (OnDeath != null)
        {
            OnDeath();
        }
    }

    int NewId()
    {
        return lastId++;
    }
}
