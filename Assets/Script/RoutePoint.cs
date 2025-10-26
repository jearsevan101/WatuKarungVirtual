using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePoint : MonoBehaviour
{
    public List<RoutePoint> neighbors;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var n in neighbors)
        {
            if (n != null)
                Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }
}
