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
    private int lastId = 0;

    public delegate void Delegate();
    public delegate void ChangeSanityDelegate(float value, float maxValue);
    public delegate void EffectDelegate(float effectValue, float time);

    public event ChangeSanityDelegate OnChangeSanity;
    public event Delegate OnLoseMind;
    public event EffectDelegate OnEffectStarted, OnEffectPerformed;
    public event Delegate OnEffectEnded;

    private void Awake()
    {
        sanity = maxSanity;
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
            ChangeSanity(effects[id].effectValue);
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
        if (effect.type != Effect.EffectType.sanity)
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

    public void ChangeSanity(float value)
    {
        sanity = Mathf.Clamp(sanity + value, 0, maxSanity);
        if (OnChangeSanity != null)
        {
            OnChangeSanity(sanity, maxSanity);
        }
        if (sanity == 0)
        {
            LoseMind();
        }
    }

    public void LoseMind()
    {
        if (OnLoseMind != null)
        {
            OnLoseMind();
        }
    }

    int NewId()
    {
        return lastId++;
    }
}
