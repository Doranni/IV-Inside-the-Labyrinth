using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource playerSteps, damageEffect;
    private AudioSource backGroundMusic;

    private static SoundsManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        backGroundMusic = GetComponent<AudioSource>();

        UpdateBackgroundMusicVolume_Private(Preferences.backMusicVolume);
        UpdateStepsVolume_Private(Preferences.stepsVolume);
        UpdateDamageEffectVolume_Private(Preferences.damageEffectVolume);
    }

    private void UpdateBackgroundMusicVolume_Private(float volume)
    {
        backGroundMusic.volume = volume / 100;
    }

    public static void UpdateBackgroundMusicVolume(float volume)
    {
        instance.UpdateBackgroundMusicVolume_Private(volume);
    }

    private void UpdateStepsVolume_Private(float volume)
    {
        playerSteps.volume = volume / 100;
    }

    public static void UpdateStepsVolume(float volume)
    {
        instance.UpdateStepsVolume_Private(volume);
    }

    private void UpdateDamageEffectVolume_Private(float volume)
    {
        damageEffect.volume = volume / 100;
    }

    public static void UpdateDamageEffectVolume(float volume)
    {
        instance.UpdateDamageEffectVolume_Private(volume);
    }

}
