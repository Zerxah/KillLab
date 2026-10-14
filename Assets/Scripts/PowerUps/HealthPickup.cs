using UnityEngine;
using UnityEngine.Audio;

public class HealthPickup : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private int healAmount = 25;

    [Header("FX")]
    [SerializeField] private ParticleSystem pickupFx;   

    [Header("Audio")]
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioMixerGroup outputGroup; 

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (!col)
        {
            col = gameObject.AddComponent<SphereCollider>();
            ((SphereCollider)col).radius = 0.6f;
        }
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponentInParent<PlayerHealth>();
        if (!health) return;

        health.Heal(healAmount);

        if (pickupFx)
        {
            var fx = Instantiate(pickupFx, transform.position, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, 2f);
        }

        OneShotAudio.Play(pickupClip, transform.position, outputGroup, 1f);

        Destroy(gameObject);
    }
}

