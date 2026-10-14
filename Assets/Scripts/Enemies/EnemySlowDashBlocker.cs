using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemySlowDashBlocker : MonoBehaviour
{
    [Header("Debuff Zone")]
    [SerializeField] private float radius = 2.5f;

    [Header("Debuff Effect")]
    [Range(0.1f, 1f)]
    [SerializeField] private float slowMultiplier = 0.6f;
    [SerializeField] private float refreshDuration = 0.25f;

    [Header("Target")]
    [SerializeField] private string playerTag = "Player";

    [Header("VFX (Feedback) - Enemy")]
    [Tooltip("Aura principal (siempre visible) para mostrar el área.")]
    [SerializeField] private ParticleSystem auraLoop;

    [Tooltip("Efecto extra cuando el player está dentro (más intenso).")]
    [SerializeField] private ParticleSystem affectLoop;

    [Tooltip("Si querés, un puff/impacto al entrar.")]
    [SerializeField] private ParticleSystem enterBurst;

    [Tooltip("Si querés, un puff al salir.")]
    [SerializeField] private ParticleSystem exitBurst;

    [Header("VFX (Feedback) - Player")]
    [Tooltip("Activa/desactiva el VFX de slow en el player (copos de nieve).")]
    [SerializeField] private bool enablePlayerSlowVfx = true;

    [Header("VFX Tuning")]
    [SerializeField] private float affectStopDelay = 0.25f;

    private SphereCollider col;
    private int playersInside;
    private float affectStopAt;

    void Reset()
    {
        SetupCollider();
    }

    void Awake()
    {
        SetupCollider();

        // Aura siempre prendida para feedback constante
        if (auraLoop && !auraLoop.isPlaying) auraLoop.Play(true);

        // Affect apagada por defecto
        if (affectLoop) affectLoop.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void OnValidate()
    {
        SetupCollider();
    }

    void SetupCollider()
    {
        col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;
    }

    void Update()
    {
        // Apagado suave del efecto "afectado" cuando no queda nadie adentro
        if (playersInside <= 0 && affectLoop && affectLoop.isPlaying && Time.time >= affectStopAt)
        {
            affectLoop.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playersInside++;

        // Feedback al entrar (Enemy)
        if (enterBurst) enterBurst.Play(true);

        // Intensidad mientras está dentro (Enemy)
        if (affectLoop && !affectLoop.isPlaying) affectLoop.Play(true);

        // Feedback en el Player (copos / slow)
        if (enablePlayerSlowVfx)
        {
            var vfx = other.GetComponentInParent<PlayerSlowVfx>();
            if (vfx) vfx.SetSlowActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        var debuff = other.GetComponentInParent<PlayerDebuffController>();
        if (!debuff) return;

        debuff.ApplySlow(slowMultiplier, refreshDuration);
        debuff.BlockDash(refreshDuration);

        // Si está dentro, mantené vivo el affect (y evitá que se apague por timer)
        affectStopAt = Time.time + affectStopDelay;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playersInside = Mathf.Max(0, playersInside - 1);

        if (exitBurst) exitBurst.Play(true);

        // Programamos apagado suave (por si vuelve a entrar enseguida)
        affectStopAt = Time.time + affectStopDelay;

        // Apagar feedback en el Player
        if (enablePlayerSlowVfx && playersInside == 0)
        {
            var vfx = other.GetComponentInParent<PlayerSlowVfx>();
            if (vfx) vfx.SetSlowActive(false);
        }
    }

    // Por si el enemigo se desactiva/muere con el player adentro, apaga VFX enemy.
    void OnDisable()
    {
        playersInside = 0;

        if (affectLoop)
            affectLoop.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}

