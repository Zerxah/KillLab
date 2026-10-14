using UnityEngine;
using UnityEngine.Audio;

public class InfiniteAmmoPickup : MonoBehaviour
{
    [Header("PowerUp")]
    [SerializeField] private float duration = 5f;

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
        var gun = other.GetComponentInParent<PlayerGunRaycast>();
        if (!gun) return;


        gun.ActivateInfiniteAmmo(duration);


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

