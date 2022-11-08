using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private AudioClip damageClip;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Anim_DamageAudio()
    {
        audioSource.PlayOneShot(damageClip);
    }
}
