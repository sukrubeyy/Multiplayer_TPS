using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instancie;
    SpawnPoint[] spawnPoints;
    private void Awake()
    {
        instancie = this;
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }
    public Transform  choseSpawn()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
    }
}
