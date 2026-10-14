using UnityEngine;

public class DamageDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 5;

    public void TakeDamage(int amount)
    {
        hp -= amount;
        Debug.Log($"{name} recibió {amount} daño. HP: {hp}");

        if (hp <= 0)
        {
            Debug.Log($"{name} destruido");
            Destroy(gameObject);
        }
    }
}
