using UnityEngine;

public class WindTracer : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] private float speed = 140f;    
    [SerializeField] private float tailDelay = 0.015f; 
    [SerializeField] private float lifeAfter = 0.03f; 

    Vector3 start;
    Vector3 end;

    float headT;
    float tailT;
    bool running;

    void Awake()
    {
        if (!lr) lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    public void Init(Vector3 from, Vector3 to)
    {
        start = from;
        end = to;

        headT = 0f;
        tailT = 0f;
        running = true;

        lr.SetPosition(0, start);
        lr.SetPosition(1, start);
    }

    void Update()
    {
        if (!running) return;

        float dist = Vector3.Distance(start, end);
        if (dist < 0.001f)
        {
            Finish();
            return;
        }

        float dt = (speed / dist) * Time.deltaTime;

        headT = Mathf.Min(1f, headT + dt);

        float tailTarget = Mathf.Clamp01(headT - tailDelay * (speed / Mathf.Max(1f, dist)));
        tailT = Mathf.MoveTowards(tailT, tailTarget, dt * 1.2f);

        Vector3 headPos = Vector3.Lerp(start, end, headT);
        Vector3 tailPos = Vector3.Lerp(start, end, tailT);

        lr.SetPosition(0, tailPos);
        lr.SetPosition(1, headPos);

        if (headT >= 1f)
            Finish();
    }

    void Finish()
    {
        running = false;
        Destroy(gameObject, lifeAfter);
    }
}



