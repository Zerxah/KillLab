using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Transform closedPose;
    [SerializeField] private Transform openPose;

    [Header("Motion")]
    [SerializeField] private float speed = 6f;

    private Vector3 targetPos;
    private Quaternion targetRot;

    void Awake()
    {
        if (closedPose)
        {
            transform.position = closedPose.position;
            transform.rotation = closedPose.rotation;
        }

        targetPos = transform.position;
        targetRot = transform.rotation;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * speed);
    }

    public void Close()
    {
        if (!closedPose) return;
        targetPos = closedPose.position;
        targetRot = closedPose.rotation;
    }

    public void Open()
    {
        if (!openPose) return;
        targetPos = openPose.position;
        targetRot = openPose.rotation;
    }
}

