using UnityEngine;

public class PlayerDebuffController : MonoBehaviour
{
    [Header("Runtime (read only)")]
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private bool dashBlocked;

    private float slowUntil;
    private float dashBlockUntil;

    void Update()
    {
        if (Time.time > slowUntil) speedMultiplier = 1f;
        if (Time.time > dashBlockUntil) dashBlocked = false;
    }

    public void ApplySlow(float multiplier, float duration)
    {
        multiplier = Mathf.Clamp(multiplier, 0.1f, 1f);

        speedMultiplier = Mathf.Min(speedMultiplier, multiplier);

        slowUntil = Mathf.Max(slowUntil, Time.time + duration);
    }

    public void BlockDash(float duration)
    {
        dashBlocked = true;
        dashBlockUntil = Mathf.Max(dashBlockUntil, Time.time + duration);
    }

    public float SpeedMultiplier => speedMultiplier;
    public bool DashBlocked => dashBlocked;
}
