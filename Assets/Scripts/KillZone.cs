using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var respawn = other.GetComponent<PlayerRespawn>();
        if (respawn) respawn.Respawn();
    }
}
