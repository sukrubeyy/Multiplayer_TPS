using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject obje;
    private void Awake()
    {
        obje.SetActive(false);
    }
}
