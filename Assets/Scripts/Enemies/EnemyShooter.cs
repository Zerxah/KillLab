using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Ranges")]
    [SerializeField] private float aggroRange = 20f;
    [SerializeField] private float shootRange = 12f;

    [Tooltip("Distancia ideal para quedarse disparando")]
    [SerializeField] private float keepDistance = 10f;

    [Tooltip("Histeresis para evitar que cambie de estado todo el tiempo")]
    [SerializeField] private float distanceTolerance = 0.75f;

    [Header("Movement Tuning")]
    [SerializeField] private float repathInterval = 0.2f;
    [SerializeField] private float retreatStep = 3.0f;
    [SerializeField] private float retreatSampleRadius = 2.0f;

    [Header("Shoot")]
    [SerializeField] private float fireRate = 0.8f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float rayRange = 60f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Attack Timing")]
    [SerializeField] private float attackWindup = 0.15f;
    [SerializeField] private bool useDelayedDamage = true;
    [SerializeField] private float fakeBulletSpeed = 22f;

    [Header("FX")]
    [SerializeField] private ParticleSystem muzzleFx;
    [SerializeField] private WindTracer windTracerPrefab;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string attackTrigger = "Attack";

    [Header("Behaviour")]
    [SerializeField] private float rotateSpeed = 12f;
    [SerializeField] private bool requireLineOfSight = false;
    [SerializeField] private LayerMask losMask = ~0;

    private NavMeshAgent agent;
    private Transform player;

    private float nextShotTime;
    private bool isAttacking;

    private float repathTimer;

    private enum State { Idle, Chase, Retreat, Attack }
    private State state;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;

        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        agent.stoppingDistance = keepDistance;
        state = State.Idle;
    }

    void Update()
    {
        if (!player) return;

        float d = Vector3.Distance(transform.position, player.position);

        // Fuera de aggro: idle
        if (d > aggroRange)
        {
            SetState(State.Idle);
            return;
        }

        FacePlayer();

        // Si puede atacar, prioridad a Attack (no dependas de velocity)
        bool canShoot = d <= shootRange && (!requireLineOfSight || HasLOS());

        // Histeresis: salir del ataque cuando se aleja un poco
        if (state == State.Attack && d > shootRange + distanceTolerance)
            canShoot = false;

        if (canShoot)
        {
            SetState(State.Attack);
            TryShoot();
        }
        else
        {
            // Mantener distancia "ideal"
            if (d > keepDistance + distanceTolerance)
                SetState(State.Chase);
            else if (d < keepDistance - distanceTolerance)
                SetState(State.Retreat);
            else
                SetState(State.Attack); // en la zona ideal, se planta (aunque no dispare por LOS)
        }

        UpdateMovement(d);
        SetSpeed(agent.velocity.magnitude);
    }

    void SetState(State newState)
    {
        if (state == newState) return;
        state = newState;

        if (state == State.Idle)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        else if (state == State.Attack)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        else
        {
            agent.isStopped = false;
        }
    }

    void UpdateMovement(float d)
    {
        repathTimer -= Time.deltaTime;

        if (state == State.Chase)
        {
            agent.stoppingDistance = keepDistance;

            if (repathTimer <= 0f)
            {
                repathTimer = repathInterval;
                agent.SetDestination(player.position);
            }
        }
        else if (state == State.Retreat)
        {
            agent.stoppingDistance = 0f;

            if (repathTimer <= 0f)
            {
                repathTimer = repathInterval;

                Vector3 awayDir = (transform.position - player.position);
                awayDir.y = 0f;
                if (awayDir.sqrMagnitude < 0.001f) awayDir = transform.forward;
                awayDir.Normalize();

                Vector3 rawTarget = transform.position + awayDir * retreatStep;

                // IMPORTANTÍSIMO: samplear navmesh para no mandar un destino inválido
                if (NavMesh.SamplePosition(rawTarget, out NavMeshHit navHit, retreatSampleRadius, NavMesh.AllAreas))
                    agent.SetDestination(navHit.position);
                else
                    agent.ResetPath();
            }
        }
        // Attack / Idle: ya está frenado por isStopped + ResetPath()
    }

    void TryShoot()
    {
        if (isAttacking) return;
        if (Time.time < nextShotTime) return;

        nextShotTime = Time.time + (1f / fireRate);

        if (anim) anim.SetTrigger(attackTrigger);
        StartCoroutine(ShootSequence());
    }

    IEnumerator ShootSequence()
    {
        isAttacking = true;

        if (attackWindup > 0f)
            yield return new WaitForSeconds(attackWindup);

        DoShot();
        isAttacking = false;
    }

    void DoShot()
    {
        if (!player) return;

        if (muzzleFx) muzzleFx.Play(true);

        Vector3 origin = firePoint ? firePoint.position : (transform.position + Vector3.up * 1.4f);

        Collider pc = player.GetComponentInChildren<Collider>();
        Vector3 target = pc ? pc.bounds.center : player.position;

        Vector3 dir = (target - origin).normalized;

        bool hasHit = Physics.Raycast(origin, dir, out RaycastHit hit, rayRange, hitMask, QueryTriggerInteraction.Ignore);
        Vector3 end = hasHit ? hit.point : origin + dir * rayRange;

        if (windTracerPrefab)
        {
            var wt = Instantiate(windTracerPrefab);
            wt.Init(origin, end);
        }

        if (!hasHit) return;

        if (useDelayedDamage)
            StartCoroutine(DelayedDamage(origin, hit));
        else
            ApplyDamage(hit);
    }

    IEnumerator DelayedDamage(Vector3 origin, RaycastHit hit)
    {
        float dist = Vector3.Distance(origin, hit.point);
        float travelTime = dist / Mathf.Max(0.01f, fakeBulletSpeed);
        yield return new WaitForSeconds(travelTime);

        if (!hit.collider) yield break;
        ApplyDamage(hit);
    }

    void ApplyDamage(RaycastHit hit)
    {
        var dmg = hit.collider.GetComponentInParent<IDamageable>();
        if (dmg != null) dmg.TakeDamage(damage);
    }

    void FacePlayer()
    {
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude < 0.0001f) return;

        Quaternion rot = Quaternion.LookRotation(look);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
    }

    bool HasLOS()
    {
        Vector3 origin = transform.position + Vector3.up * 1.4f;

        Collider pc = player.GetComponentInChildren<Collider>();
        Vector3 target = pc ? pc.bounds.center : player.position + Vector3.up;

        Vector3 dir = (target - origin).normalized;
        float dist = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, losMask, QueryTriggerInteraction.Ignore))
            return hit.collider.CompareTag("Player") || hit.collider.GetComponentInParent<PlayerHealth>() != null;

        return false;
    }

    void SetSpeed(float v)
    {
        if (anim) anim.SetFloat(speedParam, v);
    }
}






