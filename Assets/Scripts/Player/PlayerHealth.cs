using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("HP")]
    [SerializeField] private int maxHP = 10;
    [SerializeField] private float invulnAfterHit = 0.25f;

    [Header("Death/Respawn")]
    [SerializeField] private float respawnDelay = 0.5f;

    int hp;
    bool invulnerable;
    bool dead;

    PlayerRespawn respawn;

    void Awake()
    {
        hp = maxHP;
        respawn = GetComponent<PlayerRespawn>();
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;
        if (invulnerable) return;

        hp -= amount;
        if (hp <= 0)
        {
            hp = 0;
            StartCoroutine(DieRoutine());
        }
        else
        {
            StartCoroutine(InvulnRoutine());
        }

        Debug.Log($"PLAYER HP: {hp}/{maxHP}");
    }

    IEnumerator InvulnRoutine()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnAfterHit);
        invulnerable = false;
    }

    IEnumerator DieRoutine()
    {
        dead = true;
        Debug.Log("PLAYER DIED");

        yield return new WaitForSeconds(respawnDelay);

        if (respawn)
            respawn.Respawn();

        hp = maxHP;
        dead = false;

        yield return new WaitForSeconds(invulnAfterHit);
    }

    public int CurrentHP => hp;
    public int MaxHP => maxHP;
    public bool IsDead => dead;

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        if (dead) return;

        int before = hp;
        hp = Mathf.Min(hp + amount, maxHP);

        if (hp > before)
        {
            var vfx = GetComponent<PlayerStatusVFX>();
            if (vfx) vfx.PlayHealBurst();
        }
    }
}


