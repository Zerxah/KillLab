using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private Transform startCheckpoint;
    public static CheckpointManager Instance { get; private set; }

    public Vector3 LastCheckpointPos { get; private set; }
    public bool HasCheckpoint { get; private set; }

    public int CurrentRoomIndex { get; private set; } = -1;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (startCheckpoint)
            SetCheckpoint(startCheckpoint.position, -1);
    }

    public void SetCheckpoint(Vector3 pos, int roomIndex)
    {
        LastCheckpointPos = pos;
        CurrentRoomIndex = roomIndex;
        HasCheckpoint = true;

        Debug.Log($"CHECKPOINT -> Room {roomIndex} | Pos {pos}");
    }
}


