using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCPatrol : MonoBehaviour
{
    /*[Header("Navigation")]
    public Transform[] waypoints;
    public bool loop = true;              // true: muter terus; false: bolak-balik
    public float waitAtWaypoint = 2f;     // lama diam di tiap titik

    private NavMeshAgent agent;
    private Animator anim;
    private int index;
    private bool waiting;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        if (anim) anim.applyRootMotion = false; // biar jalan pakai agent, bukan root motion
    }

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"{name}: Belum ada waypoints. Drag & drop dulu di Inspector.");
            enabled = false;
            return;
        }

        index = 0;
        GoTo(index);
    }

    void Update()
    {
        // Update parameter animasi berdasar kecepatan agent
        if (anim) anim.SetFloat("Speed", agent.velocity.magnitude);

        // Saat sampai tujuan, tunggu sebentar lalu ke titik berikutnya
        if (!waiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndGo());
        }
    }

    IEnumerator WaitAndGo()
    {
        waiting = true;
        agent.isStopped = true;
        // diam 50–150% dari waitAtWaypoint biar keliatan natural
        float t = Random.Range(waitAtWaypoint * 0.5f, waitAtWaypoint * 1.5f);
        yield return new WaitForSeconds(t);

        index = NextIndex(index);
        agent.isStopped = false;
        GoTo(index);
        waiting = false;
    }

    int NextIndex(int i)
    {
        if (loop) return (i + 1) % waypoints.Length;  // muter
        // ping-pong (bolak-balik)
        if (i >= waypoints.Length - 1) System.Array.Reverse(waypoints);
        return Mathf.Clamp(i + 1, 0, waypoints.Length - 1);
    }

    void GoTo(int i)
    {
        if (waypoints[i] == null) return;
        agent.SetDestination(waypoints[i].position);
    }

    // Biar keliatan garis antar waypoint saat dipilih di editor
    void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] && waypoints[i + 1])
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
        if (loop && waypoints[0] && waypoints[^1])
            Gizmos.DrawLine(waypoints[^1].position, waypoints[0].position);
    }*/
}
