using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 0.5f;

    public EnemyHealth Spawn()
    {
        if (!enemyPrefab)
        {
            Debug.LogWarning($"{name}: Enemy Prefab no asignado.");
            return null;
        }

        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        pos.y = transform.position.y;

        var go = Instantiate(enemyPrefab, pos, transform.rotation);

        var health = go.GetComponent<EnemyHealth>();
        if (!health)
            Debug.LogWarning($"{name}: El prefab spawneado no tiene EnemyHealth.");

        return health;
    }
}



