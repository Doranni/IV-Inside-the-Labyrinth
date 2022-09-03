using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip damageClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void AudioEffect_DamageAudio()
    {
        audioSource.PlayOneShot(damageClip);
    }
}
