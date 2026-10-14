using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public event Action<EnemyHealth> OnDied;

    [SerializeField] private int maxHP = 5;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    [SerializeField] private string deadBool = "Dead";

    [Header("Death")]
    [SerializeField] private float destroyDelay = 2.5f;

    private int hp;
    private bool dead;

    void Awake()
    {
        hp = maxHP;
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        hp -= amount;
        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    void Die()
    {
        dead = true;

        OnDied?.Invoke(this);

        if (anim) anim.SetBool(deadBool, true);

        var agent = GetComponent<NavMeshAgent>();
        if (agent) agent.enabled = false;

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Destroy(gameObject, destroyDelay);
    }
}





