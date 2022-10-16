using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [Header("Torches")]
    [SerializeField]
    private float torchMaxIntensity, torchMinIntensity,
        torchEffectMaxRange, torchEffectMinRange;
    private TorchController[] torches;

    [Header("Directional Lingh")]
    [SerializeField] private Light directionalLingh;
    [SerializeField] private Color directionalLightColor_fullSanity, directionalLightColor_withoutSanity;

    [Header("Skybox")]
    [SerializeField] private Color skyboxColor_fullSanity, skyboxColor_withoutSanity;
    [SerializeField] private float sunSize_fullSanity, sunSize_withoutSanity;

    private int skyTint_id, sunSize_id;

    private static LightManager instance;

    void Awake()
    {
        instance = this;
        torches = FindObjectsOfType<TorchController>();
        skyTint_id = Shader.PropertyToID("_SkyTint");
        sunSize_id = Shader.PropertyToID("_SunSize");
    }

    private void UpdateTorches(float currentSanity_from0to1)
    {
        float torchLightIntensity = Mathf.Clamp(currentSanity_from0to1 * torchMaxIntensity, torchMinIntensity, torchMaxIntensity);
        float torchEffectRange = Mathf.Clamp(currentSanity_from0to1 * torchEffectMaxRange, torchEffectMinRange, torchEffectMaxRange);
        foreach (TorchController torch in torches)
        {
            torch.ChangeIntensity(torchLightIntensity, torchEffectRange);
        }
    }

    public static void UpdateLight(float currentSanity_from0to1)
    {
        instance.UpdateTorches(currentSanity_from0to1);
        instance.UpdateSkybox(currentSanity_from0to1);
        instance.UpdateDirectionalLight(currentSanity_from0to1);
    }

    private void UpdateSkybox(float currentSanity_from0to1)
    {
        RenderSettings.skybox.SetColor(skyTint_id, Color.Lerp(skyboxColor_withoutSanity, 
            skyboxColor_fullSanity, currentSanity_from0to1));
        RenderSettings.skybox.SetFloat(sunSize_id, Mathf.Lerp(sunSize_withoutSanity,
            sunSize_fullSanity, currentSanity_from0to1));
    }

    private void UpdateDirectionalLight(float currentSanity_from0to1)
    {
        directionalLingh.color = Color.Lerp(directionalLightColor_withoutSanity,
            directionalLightColor_fullSanity, currentSanity_from0to1);
    }
}
