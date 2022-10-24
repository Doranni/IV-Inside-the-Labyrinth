using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource playerSteps, damageEffect;
    private AudioSource backGroundMusic;

    private void Start()
    {
        backGroundMusic = GetComponent<AudioSource>();

        Preferences.OnBackgroundMusicVolumeChanged += Preferences_OnBackgroundMusicVolumeChanged;
        Preferences.OnStepsVolumeChanged += Preferences_OnStepsVolumeChanged;
        Preferences.OnDamageEffectVolumeChanged += Preferences_OnDamageEffectVolumeChanged;

        Preferences_OnBackgroundMusicVolumeChanged();
        Preferences_OnStepsVolumeChanged();
        Preferences_OnDamageEffectVolumeChanged();
    }

    private void Preferences_OnBackgroundMusicVolumeChanged()
    {
        backGroundMusic.volume = Preferences.backMusicVolume / 100;
    }

    private void Preferences_OnStepsVolumeChanged()
    {
        playerSteps.volume = Preferences.stepsVolume / 100;
    }

    private void Preferences_OnDamageEffectVolumeChanged()
    {
        damageEffect.volume = Preferences.damageEffectVolume / 100;
    }

    private void OnDestroy()
    {
        Preferences.OnBackgroundMusicVolumeChanged -= Preferences_OnBackgroundMusicVolumeChanged;
        Preferences.OnStepsVolumeChanged -= Preferences_OnStepsVolumeChanged;
        Preferences.OnDamageEffectVolumeChanged -= Preferences_OnDamageEffectVolumeChanged;
    }
}
