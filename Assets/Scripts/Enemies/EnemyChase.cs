using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private float aggroRange = 15f;
    [SerializeField] private float stopDistance = 1.6f;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    [SerializeField] private string speedParam = "Speed";

    private NavMeshAgent agent;
    private Transform player;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;

        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
            SetAnimSpeed(0f);
            return;
        }

        float d = Vector3.Distance(transform.position, player.position);

        if (d <= aggroRange)
            agent.SetDestination(player.position);
        else
            agent.ResetPath();

        SetAnimSpeed(agent.velocity.magnitude);
    }

    void SetAnimSpeed(float v)
    {
        if (anim) anim.SetFloat(speedParam, v);
    }
}


