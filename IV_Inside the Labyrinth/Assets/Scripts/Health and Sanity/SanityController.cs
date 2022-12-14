using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityController : MonoBehaviour
{
    [SerializeField] private float maxSanity;
    private float sanity;

    public float Sanity => sanity;
    public float MaxSanity => maxSanity;

    private EffectsListController effectsListController;
    Dictionary<int, Effect> effects = new Dictionary<int, Effect>();
    private Dictionary<int, Coroutine> effectCoroutines = new Dictionary<int, Coroutine>();

    public event Action<(float currentSanity, float maxSanity)> OnChangeSanity, OnGetSanityDamage;
    public event Action OnLoseMind;
    public event Action<(float effectValue, float timeLeft)> OnEffectStarted, OnEffectPerformed;
    public event Action OnEffectEnded;

    private void Awake()
    {
        sanity = maxSanity;
        TryGetComponent(out effectsListController);
    }

    private IEnumerator EffectRoutine(int id)
    {
        while (effects[id].TimeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            ChangeSanity(effects[id].effectValue);
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
        if (effect.type != Effect.EffectType.sanity)
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

    public void ChangeSanity(float value)
    {
        sanity = Mathf.Clamp(sanity + value, 0, maxSanity);
        if (value < 0)
        {
            OnGetSanityDamage?.Invoke((sanity, maxSanity));
        }
        OnChangeSanity?.Invoke((sanity, maxSanity));
        if (sanity == 0)
        {
            LoseMind();
        }
    }

    public void LoseMind()
    {
        OnLoseMind?.Invoke();
    }
}
