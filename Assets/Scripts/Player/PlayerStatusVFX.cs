using UnityEngine;

public class PlayerStatusVFX : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private ParticleSystem healBurstPrefab;    
    [SerializeField] private ParticleSystem infiniteAuraPrefab;  

    [Header("Attach")]
    [SerializeField] private Transform attachPoint;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 1f, 0f);

    private ParticleSystem infiniteAuraInstance;

    void Awake()
    {
        if (!attachPoint) attachPoint = transform;
    }

    public void PlayHealBurst()
    {
        if (!healBurstPrefab) return;

        var fx = Instantiate(healBurstPrefab, attachPoint);
        fx.transform.localPosition = localOffset;
        fx.transform.localRotation = Quaternion.identity;

        fx.Play();
        Destroy(fx.gameObject, 2f);
    }

    public void SetInfiniteAura(bool active)
    {
        if (active)
        {
            if (infiniteAuraInstance != null) return;
            if (!infiniteAuraPrefab) return;

            infiniteAuraInstance = Instantiate(infiniteAuraPrefab, attachPoint);
            infiniteAuraInstance.transform.localPosition = localOffset;
            infiniteAuraInstance.transform.localRotation = Quaternion.identity;

            infiniteAuraInstance.Play();
        }
        else
        {
            if (infiniteAuraInstance == null) return;

            // “Final” suave al terminar
            infiniteAuraInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            Destroy(infiniteAuraInstance.gameObject, 2f);
            infiniteAuraInstance = null;
        }
    }
}
