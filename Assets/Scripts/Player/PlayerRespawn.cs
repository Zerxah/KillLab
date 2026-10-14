using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float respawnYOffset = 1f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Respawn()
    {
        if (!CheckpointManager.Instance || !CheckpointManager.Instance.HasCheckpoint)
        {
            Debug.LogWarning("No hay checkpoint seteado.");
            return;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 pos = CheckpointManager.Instance.LastCheckpointPos;
        transform.position = pos + Vector3.up * respawnYOffset;

        int idx = CheckpointManager.Instance.CurrentRoomIndex;
        RoomController[] rooms = FindObjectsOfType<RoomController>();

        foreach (var room in rooms)
        {
            if (room.RoomIndex > idx)
            {
                room.ResetRoom();
            }
            else if (room.RoomIndex == idx && !room.IsCleared)
            {
                room.ResetRoom();
            }
        }

        Debug.Log($"RESPAWN -> Room {idx}");
    }
}


