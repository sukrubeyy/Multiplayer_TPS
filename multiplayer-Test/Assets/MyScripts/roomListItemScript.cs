using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
public class roomListItemScript : MonoBehaviour
{
    public RoomInfo info;
    [SerializeField] TMP_Text text;

    public void Setup(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name;
    }
    public void OnClick()
    {
        Launcher.instance.joinRoom(info);
        //Aşşağıdaki kodlar UI dizaynı için. Find Room Kapatır ve Room odasını açar.
        MenuManagament.Instance.MenuActive(2);
        MenuManagament.Instance.MenuEnabled(4);
    }

  
}
