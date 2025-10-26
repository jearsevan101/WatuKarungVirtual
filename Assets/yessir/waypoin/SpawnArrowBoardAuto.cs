using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArrowBoardAuto : MonoBehaviour
{
    [System.Serializable]
    public class SpawnSet
    {
        public Transform spawnPoint;    // posisi respawn
        public GameObject arrowMarker;  // panah penanda
        public GameObject boardObject;  // papan objek
    }

    public SpawnSet[] spawns;       // daftar semua spawn
    public GameObject player;       // drag player ke sini di Inspector

    private int lastIndex = -1;

    void Update()
    {
        // Cek spawn mana yang paling dekat dengan player
        int nearestIndex = GetNearestSpawnIndex();

        if (nearestIndex != lastIndex)
        {
            ActivateSpawn(nearestIndex);
            lastIndex = nearestIndex;
        }
    }

    // Cari spawn terdekat
    int GetNearestSpawnIndex()
    {
        int nearest = 0;
        float minDist = Vector3.Distance(player.transform.position, spawns[0].spawnPoint.position);

        for (int i = 1; i < spawns.Length; i++)
        {
            float dist = Vector3.Distance(player.transform.position, spawns[i].spawnPoint.position);
            if (dist < minDist)
            {
                nearest = i;
                minDist = dist;
            }
        }

        return nearest;
    }

    // Aktifkan arrow & papan sesuai spawn
    void ActivateSpawn(int index)
    {
        for (int i = 0; i < spawns.Length; i++)
        {
            bool active = (i == index);

            if (spawns[i].arrowMarker != null) spawns[i].arrowMarker.SetActive(active);
            if (spawns[i].boardObject != null) spawns[i].boardObject.SetActive(active);
        }
    }
}