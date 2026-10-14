using System.Collections;
using UnityEngine;

public class PlayerGunRaycast : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform cam;
    [SerializeField] private Transform firePoint;

    [Header("Gun")]
    [SerializeField] private float range = 60f;
    [SerializeField] private float fireRate = 6f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Ammo")]
    [SerializeField] private int magSize = 12;
    [SerializeField] private int ammoInMag = 12;
    [SerializeField] private int ammoReserve = 60;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 1.6f;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private string reloadTrigger = "Reload";

    [Header("FX")]
    [SerializeField] private ParticleSystem muzzleFlashFx;
    [SerializeField] private ParticleSystem hitFxPrefab;

    [Header("Wind Tracer (opcional)")]
    [SerializeField] private WindTracer windTracerPrefab;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip shotClip;
    [SerializeField] private AudioClip reloadClip;

    private float nextShotTime;
    private bool isReloading;

    private bool infiniteAmmo;
    private Coroutine infiniteAmmoRoutine;

    void Awake()
    {
        if (!cam && Camera.main) cam = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            TryReload();

        if (Input.GetMouseButtonDown(0))
            TryShoot();
    }

    void TryShoot()
    {
        if (isReloading) return;

        if (Time.time < nextShotTime) return;
        nextShotTime = Time.time + (1f / fireRate);

        if (!infiniteAmmo)
        {
            if (ammoInMag <= 0) return;
            ammoInMag--;
        }

        if (muzzleFlashFx) muzzleFlashFx.Play(true);
        if (sfxSource && shotClip) sfxSource.PlayOneShot(shotClip);

        Ray ray = new Ray(cam.position, cam.forward);
        Vector3 end = cam.position + cam.forward * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            end = hit.point;

            if (hitFxPrefab)
            {
                var fx = Instantiate(hitFxPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                fx.Play(true);
                Destroy(fx.gameObject, 2f);
            }

            var d = hit.collider.GetComponentInParent<IDamageable>();
            if (d != null)
                d.TakeDamage(damage);
        }

        if (windTracerPrefab)
        {
            Vector3 start = firePoint ? firePoint.position : cam.position;
            var wt = Instantiate(windTracerPrefab);
            wt.Init(start, end);
        }
    }

    void TryReload()
    {
        if (isReloading) return;
        if (ammoInMag >= magSize) return;
        if (ammoReserve <= 0) return;

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;

        if (weaponAnimator)
            weaponAnimator.SetTrigger(reloadTrigger);

        if (sfxSource && reloadClip)
            sfxSource.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        int needed = magSize - ammoInMag;
        int take = Mathf.Min(needed, ammoReserve);

        ammoInMag += take;
        ammoReserve -= take;

        isReloading = false;
    }

    public void ActivateInfiniteAmmo(float duration)
    {
        if (infiniteAmmoRoutine != null)
            StopCoroutine(infiniteAmmoRoutine);

        infiniteAmmoRoutine = StartCoroutine(InfiniteAmmoRoutine(duration));
    }

    private IEnumerator InfiniteAmmoRoutine(float duration)
    {
        infiniteAmmo = true;

        var vfx = GetComponentInParent<PlayerStatusVFX>();
        if (vfx) vfx.SetInfiniteAura(true);

        yield return new WaitForSeconds(duration);

        infiniteAmmo = false;

        if (vfx) vfx.SetInfiniteAura(false);

        infiniteAmmoRoutine = null;
    }

    public bool InfiniteAmmoActive => infiniteAmmo;

    public int AmmoInMag => ammoInMag;
    public int AmmoReserve => ammoReserve;
    public int MagSize => magSize;
    public bool IsReloading => isReloading;

    public void AddAmmo(int amount, bool fillMagFirst = true)
    {
        if (amount <= 0) return;

        if (fillMagFirst)
        {
            int need = magSize - ammoInMag;
            int toMag = Mathf.Min(need, amount);
            ammoInMag += toMag;
            amount -= toMag;
        }

        ammoReserve += amount;
    }
}





