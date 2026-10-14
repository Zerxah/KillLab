using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attacksPerSecond = 1.0f;
    [SerializeField] private int damage = 1;

    [Header("Timing")]
    [SerializeField] private float damageDelay = 0.25f; 
    [SerializeField] private bool requireLineOfSight = true;
    [SerializeField] private LayerMask losMask = ~0;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    [SerializeField] private string attackTrigger = "Attack";

    private Transform player;
    private NavMeshAgent agent;
    private float nextAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        if (!player) return;

        Vector3 to = player.position - transform.position;
        to.y = 0f;
        if (to.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(to), Time.deltaTime * 10f);
        }

        float d = Vector3.Distance(transform.position, player.position);

        agent.stoppingDistance = attackRange * 0.95f;

        if (d <= attackRange && Time.time >= nextAttackTime)
        {
            if (requireLineOfSight && !HasLOS())
                return;

            nextAttackTime = Time.time + (1f / attacksPerSecond);

            if (anim) anim.SetTrigger(attackTrigger);
            StartCoroutine(DealDamageAfterDelay());
        }
    }

    bool HasLOS()
    {
        Vector3 origin = transform.position + Vector3.up * 1.4f;

        Collider pc = player.GetComponentInChildren<Collider>();
        Vector3 target = pc ? pc.bounds.center : player.position + Vector3.up * 1.0f;

        Vector3 dir = (target - origin).normalized;
        float dist = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, losMask, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.GetComponentInParent<PlayerHealth>() != null ||
                   hit.collider.CompareTag("Player");
        }
        return false;
    }

    IEnumerator DealDamageAfterDelay()
    {
        yield return new WaitForSeconds(damageDelay);

        if (!player) yield break;

        float d = Vector3.Distance(transform.position, player.position);
        if (d > attackRange) yield break;

        var dmg = player.GetComponentInParent<IDamageable>();
        if (dmg != null)
            dmg.TakeDamage(damage);
    }
}

