using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsListController : MonoBehaviour
{
    public Dictionary<int, Effect> effects = new Dictionary<int, Effect>();

    public delegate void ListEffectsDelegate(Dictionary<int, Effect> effects);
    public event ListEffectsDelegate OnEffectsChanged;

    public int Add(Effect effect, int id)
    {
        effects.Add(id, effect);
        UpdateEffects();
        return id;
    }

    public void RemoveEffect(int id)
    {
        effects.Remove(id);
        UpdateEffects();
    }

    public void UpdateEffects()
    {
        if (OnEffectsChanged != null)
        {
            OnEffectsChanged(effects);
        }
    }
}

[System.Serializable]
public class Effect
{
    public enum EffectType
    {
        health,
        sanity,
        movement
    }

    public EffectType type;
    public bool isPositive;
    public float effectValue, timeTotal;
    private float timeLeft;
    public float TimeLeft => timeLeft;

    public Effect(EffectType type, bool isPositive, float effectValue, float time)
    {
        this.type = type;
        timeTotal = time;
        timeLeft = time;
        this.effectValue = effectValue;
        this.isPositive = isPositive;
    }
    public Effect (Effect effect)
    {
        type = effect.type;
        timeTotal = effect.timeTotal;
        timeLeft = timeTotal;
        effectValue = effect.effectValue;
        isPositive = effect.isPositive;
    }

    public void ChangeTimeLeft(float value)
    {
        timeLeft = Mathf.Clamp(timeLeft + value, 0, timeTotal);
    }

    public override string ToString()
    {
        string res = string.Empty;
        switch (type)
        {
            case EffectType.health:
                {
                    res = $"<color=#FF7D72>Health</color> effect: ";
                    break;
                }
            case EffectType.sanity:
                {
                    res = $"<color=#FF7D72>Sanity</color> effect: ";
                    break;
                }
            case EffectType.movement:
                {
                    res = $"<color=#FF7D72>Movement</color> effect: <color=#FF7D72>* </color>";
                    break;
                }
        }
        res += $"<color=#FF7D72>{effectValue} </color> for <color=#FF7D72>{Mathf.Round(timeLeft)} s. </color>";
        return res;
    }
}
