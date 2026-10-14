using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum RoomState { Inactive, Fighting, Cleared }

    [Header("Room Index")]
    [SerializeField] private int roomIndex;
    public int RoomIndex => roomIndex;

    [Header("Trigger de entrada (en este mismo GameObject)")]
    [SerializeField] private Collider enterTrigger;

    [Header("Puertas (DoorAnim)")]
    [SerializeField] private DoorAnim[] doors;

    [Header("Spawners")]
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private int enemiesToSpawn = 6;

    [Header("Checkpoint")]
    [SerializeField] private Transform checkpointPoint;

    [Header("Config")]
    [SerializeField] private bool startDoorsOpen = true;
    [SerializeField] private bool oneTimeOnly = true;

    [Header("Room Flow (opcional)")]
    [SerializeField] private RoomController nextRoom;
    [SerializeField] private bool enableNextRoomOnClear = true;

    private RoomState state = RoomState.Inactive;
    private readonly List<EnemyHealth> alive = new();

    public bool IsCleared => state == RoomState.Cleared;

    void Awake()
    {
        if (!enterTrigger) enterTrigger = GetComponent<Collider>();
        if (enterTrigger) enterTrigger.isTrigger = true;

        if (startDoorsOpen) OpenDoors();
        else CloseDoors();

        if (nextRoom && nextRoom.enterTrigger)
            nextRoom.enterTrigger.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (state != RoomState.Inactive) return;
        if (!other.CompareTag("Player")) return;

        StartRoom();
    }

    void StartRoom()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (checkpointPoint && CheckpointManager.Instance)
            CheckpointManager.Instance.SetCheckpoint(checkpointPoint.position, roomIndex);

        state = RoomState.Fighting;
        CloseDoors();
        SpawnWave();
    }

    void SpawnWave()
    {
        alive.Clear();

        if (spawners == null || spawners.Length == 0)
        {
            Debug.LogWarning($"{name}: No hay spawners asignados.");
            ClearRoom();
            return;
        }

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            EnemySpawner sp = spawners[i % spawners.Length];
            EnemyHealth e = sp.Spawn();
            if (!e) continue;

            e.OnDied += HandleEnemyDied;
            alive.Add(e);
        }

        CheckIfCleared();
    }

    void HandleEnemyDied(EnemyHealth e)
    {
        if (e) e.OnDied -= HandleEnemyDied;
        alive.Remove(e);
        CheckIfCleared();
    }

    void CheckIfCleared()
    {
        if (state != RoomState.Fighting) return;

        alive.RemoveAll(x => x == null);

        if (alive.Count == 0)
            ClearRoom();
    }

    void ClearRoom()
    {
        state = RoomState.Cleared;

        OpenDoors();

        if (checkpointPoint && CheckpointManager.Instance)
            CheckpointManager.Instance.SetCheckpoint(checkpointPoint.position, roomIndex);

        if (enableNextRoomOnClear && nextRoom && nextRoom.enterTrigger)
            nextRoom.enterTrigger.enabled = true;

        if (oneTimeOnly && enterTrigger)
            enterTrigger.enabled = false;
    }

    public void ResetRoom()
    {
        state = RoomState.Inactive;

        if (enterTrigger)
            enterTrigger.enabled = true;

        OpenDoors();

        foreach (var e in alive)
            if (e) Destroy(e.gameObject);

        alive.Clear();
    }

    void CloseDoors()
    {
        if (doors == null) return;
        for (int i = 0; i < doors.Length; i++)
            if (doors[i]) doors[i].Close();
    }

    void OpenDoors()
    {
        if (doors == null) return;
        for (int i = 0; i < doors.Length; i++)
            if (doors[i]) doors[i].Open();
    }
}



