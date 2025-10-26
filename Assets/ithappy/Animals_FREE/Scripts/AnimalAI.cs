using UnityEngine;
using Controller; // butuh karena CreatureMover ada di namespace Controller

[RequireComponent(typeof(CreatureMover))]
public class AnimalAI : MonoBehaviour
{
    private CreatureMover mover;

    private enum State { Idle, Wander, Run, Patrol }
    [SerializeField] private State current = State.Idle;

    // Input ke CreatureMover
    private Vector2 moveAxis;
    private Vector3 lookTarget;
    private bool isRun;

    // Timer
    private float stateTimer;
    private float steerTimer;

    [Header("Idle")]
    public Vector2 idleDurationRange = new Vector2(2f, 5f);

    [Header("Wander (jalan santai)")]
    public Vector2 wanderDurationRange = new Vector2(4f, 8f);
    public Vector2 wanderRepathRange = new Vector2(1.5f, 3f); // seberapa sering ganti arah kecil
    public Vector2 wanderStepRange = new Vector2(3f, 8f);     // jarak target lokal

    [Header("Run (lari pendek)")]
    public float runDuration = 2f;
    public Vector2 runRepathRange = new Vector2(0.6f, 1.2f);
    public Vector2 runStepRange = new Vector2(5f, 10f);

    [Header("Patrol Waypoints (biar ga off-map)")]
    public Transform[] waypoints;
    public float waypointReachThreshold = 1.5f;
    private int wpIndex = 0;

    [Header("Avoidance (opsional sederhana)")]
    public LayerMask obstacleLayers;
    public float probeDistance = 2f;
    public float probeRadius = 0.3f;

    private Vector3 steerTarget; // titik tujuan lokal (untuk Wander/Run)

    void Awake()
    {
        mover = GetComponent<CreatureMover>();
        SetState(State.Idle);
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;
        steerTimer -= Time.deltaTime;

        switch (current)
        {
            case State.Idle:
                moveAxis = Vector2.zero;
                isRun = false;

                // tetap menatap ke depan
                lookTarget = transform.position + transform.forward * 3f;

                if (stateTimer <= 0f)
                {
                    // Kalau ada waypoint, 50% peluang masuk Patrol
                    float roll = Random.value;
                    if (waypoints != null && waypoints.Length > 0 && roll < 0.5f)
                        SetState(State.Patrol);
                    else if (roll < 0.85f)
                        SetState(State.Wander);
                    else
                        SetState(State.Run);
                }
                break;

            case State.Wander:
                // Rencanakan target lokal berkala
                if (steerTimer <= 0f || Reached(steerTarget, 1.2f))
                {
                    PlanLocalTarget(wanderStepRange);
                    steerTimer = Random.Range(wanderRepathRange.x, wanderRepathRange.y);
                }

                // Cek rintangan di depan; jika ada, rencanakan ulang
                if (HitObstacleAhead()) PlanLocalTarget(wanderStepRange);

                // Bergerak maju menuju steerTarget
                lookTarget = steerTarget;
                moveAxis = new Vector2(0f, 1f); // maju mengikuti arah ke lookTarget
                isRun = false;

                if (stateTimer <= 0f) SetState(State.Idle);
                break;

            case State.Run:
                if (steerTimer <= 0f || Reached(steerTarget, 1.5f))
                {
                    PlanLocalTarget(runStepRange);
                    steerTimer = Random.Range(runRepathRange.x, runRepathRange.y);
                }

                if (HitObstacleAhead()) PlanLocalTarget(runStepRange);

                lookTarget = steerTarget;
                moveAxis = new Vector2(0f, 1f);
                isRun = true;

                if (stateTimer <= 0f) SetState(State.Idle);
                break;

            case State.Patrol:
                if (waypoints == null || waypoints.Length == 0)
                {
                    SetState(State.Wander);
                    break;
                }

                Transform wp = waypoints[wpIndex];
                Vector3 wpPos = wp.position;
                wpPos.y = transform.position.y; // jaga tetap di tanah

                // Gerak menuju waypoint
                lookTarget = wpPos;
                moveAxis = new Vector2(0f, 1f);
                isRun = false;

                if (Vector3.Distance(transform.position, wpPos) <= waypointReachThreshold)
                {
                    wpIndex = (wpIndex + 1) % waypoints.Length;
                    SetState(State.Idle); // istirahat sebentar sebelum ke waypoint berikutnya
                }
                break;
        }

        // Kirim input ke CreatureMover (jump = false)
        mover.SetInput(in moveAxis, in lookTarget, in isRun, false);
    }

    private void SetState(State s)
    {
        current = s;
        switch (s)
        {
            case State.Idle:
                stateTimer = Random.Range(idleDurationRange.x, idleDurationRange.y);
                break;
            case State.Wander:
                stateTimer = Random.Range(wanderDurationRange.x, wanderDurationRange.y);
                PlanLocalTarget(wanderStepRange);
                steerTimer = Random.Range(wanderRepathRange.x, wanderRepathRange.y);
                break;
            case State.Run:
                stateTimer = runDuration;
                PlanLocalTarget(runStepRange);
                steerTimer = Random.Range(runRepathRange.x, runRepathRange.y);
                break;
            case State.Patrol:
                stateTimer = Mathf.Infinity; // sampai waypoint tercapai
                break;
        }
    }

    private void PlanLocalTarget(Vector2 stepRange)
    {
        // pilih arah acak di bidang XZ
        Vector2 dir2 = Random.insideUnitCircle.normalized;
        float dist = Random.Range(stepRange.x, stepRange.y);
        Vector3 dir = new Vector3(dir2.x, 0f, dir2.y);
        steerTarget = transform.position + dir * dist;
        steerTarget.y = transform.position.y;
    }

    private bool Reached(Vector3 pos, float radius)
    {
        Vector3 flatA = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatB = new Vector3(pos.x, 0f, pos.z);
        return Vector3.Distance(flatA, flatB) <= radius;
    }

    private bool HitObstacleAhead()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 fwd = (lookTarget - transform.position);
        fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.01f) fwd = transform.forward;
        fwd.Normalize();

        return Physics.SphereCast(origin, probeRadius, fwd, out _, probeDistance, obstacleLayers, QueryTriggerInteraction.Ignore);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Gizmo steerTarget
        Gizmos.DrawWireSphere(steerTarget, 0.3f);

        // Gizmo waypoints
        if (waypoints != null && waypoints.Length > 0)
        {
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < waypoints.Length; i++)
            {
                var a = waypoints[i];
                var b = waypoints[(i + 1) % waypoints.Length];
                if (a && b)
                {
                    UnityEditor.Handles.DrawLine(a.position, b.position);
                    UnityEditor.Handles.SphereHandleCap(0, a.position, Quaternion.identity, 0.3f, EventType.Repaint);
                }
            }
        }
    }
#endif
}
