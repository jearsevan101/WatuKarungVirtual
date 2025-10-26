using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Assign Player")]
    public GameObject player;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        if (player == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Player or spawn points not assigned!");
            return;
        }

        // Pick random spawn point
        Transform chosenSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Copy full world transform
        player.transform.SetPositionAndRotation(chosenSpawn.position, chosenSpawn.rotation);
    }
}
