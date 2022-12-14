using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private SanityController plSanity;

    [Header("Torches")]
    [SerializeField] private float torchMaxIntensity;
    [SerializeField] private float torchMinIntensity, torchEffectMaxRange, torchEffectMinRange;
    private TorchController[] torches;

    [Header("Directional Lingh")]
    [SerializeField] private Light directionalLingh;
    [SerializeField] private Color directionalLightColor_fullSanity, directionalLightColor_withoutSanity;

    [Header("Skybox")]
    [SerializeField] private Color skyboxColor_fullSanity, skyboxColor_withoutSanity;
    [SerializeField] private float sunSize_fullSanity, sunSize_withoutSanity;

    private int id_skyTint, id_sunSize;

    void Start()
    {
        torches = FindObjectsOfType<TorchController>();
        id_skyTint = Shader.PropertyToID("_SkyTint");
        id_sunSize = Shader.PropertyToID("_SunSize");
        plSanity.OnChangeSanity += UpdateLight;
        UpdateLight((plSanity.Sanity, plSanity.MaxSanity));
    }

    private void UpdateLight((float currentSanity, float maxSanity) values)
    {
        float currentSanity_from0to1 = values.currentSanity / values.maxSanity;
        UpdateTorches(currentSanity_from0to1);
        UpdateSkybox(currentSanity_from0to1);
        UpdateDirectionalLight(currentSanity_from0to1);
    }

    private void UpdateTorches(float currentSanity_from0to1)
    {
        float torchLightIntensity = Mathf.Lerp(torchMinIntensity, torchMaxIntensity, currentSanity_from0to1);
        float torchEffectRange = Mathf.Lerp(torchEffectMinRange, torchEffectMaxRange, currentSanity_from0to1);
        foreach (TorchController torch in torches)
        {
            torch.ChangeIntensity(torchLightIntensity, torchEffectRange);
        }
    }

    private void UpdateSkybox(float currentSanity_from0to1)
    {
        RenderSettings.skybox.SetColor(id_skyTint, Color.Lerp(skyboxColor_withoutSanity, 
            skyboxColor_fullSanity, currentSanity_from0to1));
        RenderSettings.skybox.SetFloat(id_sunSize, Mathf.Lerp(sunSize_withoutSanity,
            sunSize_fullSanity, currentSanity_from0to1));
    }

    private void UpdateDirectionalLight(float currentSanity_from0to1)
    {
        directionalLingh.color = Color.Lerp(directionalLightColor_withoutSanity,
            directionalLightColor_fullSanity, currentSanity_from0to1);
    }

    private void OnDestroy()
    {
        plSanity.OnChangeSanity -= UpdateLight;
    }
}
