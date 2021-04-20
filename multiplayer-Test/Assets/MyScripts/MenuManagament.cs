using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class MenuManagament : MonoBehaviourPunCallbacks
{
    public GameObject[] menus;
    public static MenuManagament Instance;
    public TMP_InputField nick;

    private void Awake()
    {
        Instance = this;
    }
    public void MenuActive(int menuIndex)
    {
        menus[menuIndex].SetActive(true);
    }
    public void MenuEnabled(int menuIndex)
    {
        menus[menuIndex].SetActive(false);
    }

    public void createRoomButtonFirst()
    {
        PhotonNetwork.NickName = nick.text;
        if (string.IsNullOrEmpty(nick.text))
        {
            Debug.Log("Random Nick Seçildi");
            PhotonNetwork.NickName = "Player" + Random.Range(0, 2000);
        }
        
        MenuActive(1);
        MenuEnabled(0);
    }
    public void findRoom1()
    {
        PhotonNetwork.NickName = nick.text;
        if (string.IsNullOrEmpty(nick.text))
        {
            Debug.Log("Random Nick Seçildi");
            PhotonNetwork.NickName = "Player" + Random.Range(0, 2000);
        }
        
        MenuActive(4);
        MenuEnabled(0);
    }
    public void errorMenu()
    {
        MenuActive(0);
        MenuEnabled(3);
    }
    public void BackButtonForFindRoom()
    {
        MenuActive(0);
        MenuEnabled(4);
    }
    public void BackButtonForCreateRoom()
    {
        MenuActive(0);
        MenuEnabled(1);
    }
}
