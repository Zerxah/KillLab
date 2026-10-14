using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSlowVfx : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem slowSnow;

    [Header("Audio")]
    [SerializeField] private AudioClip freezeClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.8f;

    private AudioSource audioSource;

    private int slowStack; // Soporta múltiples enemigos

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D

        if (slowSnow)
            slowSnow.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void SetSlowActive(bool active)
    {
        if (active)
        {
            slowStack++;

            if (slowStack == 1)
            {
                // Primer slow → prender VFX y sonido
                if (slowSnow && !slowSnow.isPlaying)
                    slowSnow.Play(true);

                if (freezeClip)
                    audioSource.PlayOneShot(freezeClip, volume);
            }
        }
        else
        {
            slowStack = Mathf.Max(0, slowStack - 1);

            if (slowStack == 0)
            {
                if (slowSnow)
                    slowSnow.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}