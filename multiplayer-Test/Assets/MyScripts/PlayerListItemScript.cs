using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class PlayerListItemScript : MonoBehaviourPunCallbacks
{
    Player player;
    [SerializeField] TMP_Text PlayerNameText;
    public void Setup(Player _player)
    {
        player = _player;
        PlayerNameText.text = _player.NickName;
        if(string.IsNullOrEmpty(PlayerNameText.text))
        {
            PlayerNameText.text = "Player" + Random.Range(0, 2000);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player==otherPlayer)
        {
            Destroy(gameObject);
        }
    }
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
