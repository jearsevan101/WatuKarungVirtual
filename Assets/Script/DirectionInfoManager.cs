using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionInfoManager : MonoBehaviour
{
    [SerializeField] private Transform parentPoint;
    [SerializeField] private Transform playerTransform; // location of XROrigin
    [SerializeField] private GameObject prefabDirection;
    [SerializeField] private Transform parentPrefab;
    [SerializeField] private float distanceEachPrefab = 1f;
    [SerializeField] private Transform coconutPosition;
    [SerializeField] private Transform sandPositionDisable;
    [SerializeField] private Transform sandPositionNormal;
    [SerializeField] private Transform clampPositionDisable;
    [SerializeField] private Transform clampPositionNormal;

    private Transform currentSandPosition;
    private Transform currentClampPosition;

    private void Start()
    {
        EventManager.OnActiveMinigame += SetRouteToDirection;
        EventManager.OnModeNormal += HandleModeGame;
    }
    private void HandleModeGame(bool isGameNormal)
    {
        if (isGameNormal)
        {
            currentSandPosition = sandPositionNormal;
            currentClampPosition = clampPositionNormal;
        }
        else
        {
            currentSandPosition = sandPositionDisable;
            currentClampPosition = clampPositionDisable;
        }
    }
    public void SetRouteToDirection(currentActiveMinigame direction)
    {
        switch (direction)
        {
            case currentActiveMinigame.coconut:
                CreateRoute(coconutPosition.position);
                break;
            case currentActiveMinigame.sand:
                CreateRoute(currentSandPosition.position);
                break;
            case currentActiveMinigame.clamp:
                CreateRoute(currentClampPosition.position);
                break;
            default:
                break;
        }
    }

    public void CreateRoute(Vector3 destination)
    {
        // Get all route points
        RoutePoint[] allPoints = parentPoint.GetComponentsInChildren<RoutePoint>();

        if (allPoints.Length == 0)
        {
            Debug.LogWarning("No RoutePoints found!");
            return;
        }

        // Find start and end nodes
        RoutePoint startNode = GetNearestPoint(playerTransform.position, allPoints);
        RoutePoint endNode = GetNearestPoint(destination, allPoints);

        // Run pathfinding (Dijkstra)
        List<RoutePoint> path = FindShortestPath(startNode, endNode);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No valid path found!");
            return;
        }

        // Clear previous arrows
        foreach (Transform child in parentPrefab)
            Destroy(child.gameObject);

        // Spawn arrows along the path
        Vector3 lastPos = playerTransform.position;
        foreach (var point in path)
        {
            InstantiateDirectionAlongLine(lastPos, point.transform.position);
            lastPos = point.transform.position;
        }
        // Final segment to destination
        InstantiateDirectionAlongLine(lastPos, destination);
    }

    private RoutePoint GetNearestPoint(Vector3 reference, RoutePoint[] points)
    {
        RoutePoint nearest = null;
        float minDist = Mathf.Infinity;
        foreach (var p in points)
        {
            float dist = Vector3.Distance(reference, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }
        return nearest;
    }

    private List<RoutePoint> FindShortestPath(RoutePoint start, RoutePoint goal)
    {
        // Dijkstra-like search
        var prev = new Dictionary<RoutePoint, RoutePoint>();
        var distances = new Dictionary<RoutePoint, float>();
        var queue = new List<RoutePoint>();

        foreach (var p in parentPoint.GetComponentsInChildren<RoutePoint>())
        {
            distances[p] = Mathf.Infinity;
            queue.Add(p);
        }
        distances[start] = 0;

        while (queue.Count > 0)
        {
            queue.Sort((a, b) => distances[a].CompareTo(distances[b]));
            RoutePoint current = queue[0];
            queue.RemoveAt(0);

            if (current == goal)
                break;

            foreach (var neighbor in current.neighbors)
            {
                float alt = distances[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    prev[neighbor] = current;
                }
            }
        }

        // Reconstruct path
        List<RoutePoint> path = new List<RoutePoint>();
        RoutePoint u = goal;
        if (!prev.ContainsKey(u)) return path; // no route
        while (u != null)
        {
            path.Insert(0, u);
            prev.TryGetValue(u, out u);
        }
        return path;
    }

    private void InstantiateDirectionAlongLine(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;

        int count = Mathf.FloorToInt(distance / distanceEachPrefab);
        for (int i = 1; i <= count; i++)
        {
            Vector3 pos = start + direction * distanceEachPrefab * i;
            GameObject obj = Instantiate(prefabDirection, pos, Quaternion.LookRotation(direction), parentPrefab);
        }
    }
}
