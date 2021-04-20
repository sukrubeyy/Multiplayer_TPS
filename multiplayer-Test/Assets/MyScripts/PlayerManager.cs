using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject control;
    Transform spawnPoint;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        //Eğer ben isem 
        if(PV.IsMine)
        {
            //CreatePlayerManager methodunu çağır
            CreatePlayerManager();
        }

    }
    void CreatePlayerManager()
    {
        //Photon serverlarında obje oluşması için PhotonNetwork.Instantie methodu
        // Path.Combine
        spawnPoint = SpawnManager.instancie.choseSpawn();
        control = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Players"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { PV.ViewID});
    }
    public void Die()
    {
        PhotonNetwork.Destroy(control);
        CreatePlayerManager();
    }
  
}
