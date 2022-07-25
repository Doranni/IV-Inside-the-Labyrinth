using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private Light directionalLingh;
    [SerializeField]
    private float directionalLightMaxIntensity, directionalLightMinIntensity,
        torchMaxIntensity, torchMinIntensity,
        torchEffectMaxRange, torchEffectMinRange;
    private TorchController[] torches;
    private static LightManager instance;

    void Awake()
    {
        instance = this;
        torches = FindObjectsOfType<TorchController>();
    }

    private void UpdateBrightness_Private(float value)
    {
        directionalLingh.intensity = Mathf.Clamp(value * directionalLightMaxIntensity, 
            directionalLightMinIntensity, directionalLightMaxIntensity);
        float torchLightIntensity = Mathf.Clamp(value * torchMaxIntensity, torchMinIntensity, torchMaxIntensity);
        float torchEffectRange = Mathf.Clamp(value * torchEffectMaxRange, torchEffectMinRange, torchEffectMaxRange);
        foreach (TorchController torch in torches)
        {
            torch.ChangeIntensity(torchLightIntensity, torchEffectRange);
        }
    }

    public static void UpdateBrightness(float value)
    {
        instance.UpdateBrightness_Private(value);
    }
}
