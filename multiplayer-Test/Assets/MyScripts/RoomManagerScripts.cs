using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManagerScripts : MonoBehaviourPunCallbacks
{
    public static RoomManagerScripts Instance;
     void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        // Debug.Log("Yönetici");
        DontDestroyOnLoad(gameObject);
        Instance = this;

    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded +=onSceneLoaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= onSceneLoaded;
    }
    void onSceneLoaded(Scene sceen ,LoadSceneMode loadSceneMode)
    {
       if(sceen.buildIndex==1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PlayerManager"),Vector3.zero,Quaternion.identity);
        }
    }
   
}
