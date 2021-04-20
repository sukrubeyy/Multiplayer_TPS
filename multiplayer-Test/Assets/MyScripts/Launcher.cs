using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using System.IO;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    [SerializeField] MenuManagament menuControl;

    public TMP_InputField roomNameInput;
    public TMP_InputField nickName;
    public TMP_Text roomName;
    public TMP_Text errorMessage;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameObject;
    [SerializeField] Slider slider;
    [SerializeField] Text sliderText;
    [SerializeField] GameObject RoomManager;
    TouchScreenKeyboard keyboard;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Debug.Log("Connecting to master");
        PhotonNetwork.ConnectUsingSettings();
        Instantiate(RoomManager);

    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        //Loby'ye katılması için gerekli olan PhotonNetwork Scripti içerisindeki JoinLobby Methodunu Çağırıyoruz.
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        //Loby'e katıldı.
        Debug.Log("Joined Lobby");
        
        //Loby'e katıldığında , katılan oyuncuya random bir nickname ataması yapıyoruz.
        // PhotonNetwork.NickName = nickName.text;

    }

    //Create Room Menüsündeki Create Butonuna Tıklanınca yapacağı işlemler aşşağıda yer almaktadır.
    public void CreateRoom()
    {
     
        //UI arayüzünde TextMeshPro_InputField boşsa yani oda adı verilmemişse oda kuramasın diye böyle bir şart koyduk.
      if (string.IsNullOrEmpty(roomNameInput.text))
        {
            Debug.Log("Boş");
            return;
        }
      //Eğer oda adı vermiş ise aşşağıda yer alan PhotonNetwork.CreateRoom(); methoduna parametre olarak bu TMP_InputField'i gönderiyoruz.
      //Çünkü serverda oda adı kaydedilecek.
        PhotonNetwork.CreateRoom(roomNameInput.text);

    }

    //Odaya Giriş Yaptığında Çalışacak Kodlar Aşşağıda Yer Almaktadır.
    public override void OnJoinedRoom()
    {
        //Create Room veya Find Room açıksa bu menüleri kapatıp room odasını açıyoruz.
        menuControl.MenuActive(2);
        menuControl.MenuEnabled(1);
       if(PhotonNetwork.IsMasterClient)
        {
            startGameObject.SetActive(true);
        }
       else
        {
            startGameObject.SetActive(false);
        }
            
       
       
        //roomName text'ine PhotonNetwork serverlerinde kaydedilen serverimizin adını atıyoruz.

        roomName.text = PhotonNetwork.CurrentRoom.Name;
        //Player dizisi oluşturuyoruz ve bu diziye PhotonNetwork.PlayerList methodunu atıyoruz burada bize playerların sayısını veriyor.
        Player[] players = PhotonNetwork.PlayerList;
        //Döngü oluşturuyoruz. Bu döngü player sayısı kadar dönecek.

        //PlayerList güncel tutmak için
        foreach (Transform trns in playerListContent)
        {
            Destroy(trns.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            //PlayerListItem prefabı PlayerListContent transformu altında oluşturulacak.
            //PlayerListITemScript Scriptinde yer alan Setup() methoduna parametre olarak players[i] değerini gönderiyoruz.
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItemScript>().Setup(players[i]);
        }
       
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    //Oda kurulumunda herhangi bir hata olursa 
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
       //açık olan tüm odaları döngü ile kapatıyoruz
        for (int i = 0; i < menuControl.menus.Length; i++)
        {
            menuControl.MenuEnabled(i);
        }
        //ErrorMenu odasını açıyoruz.
        menuControl.MenuActive(3);
        //Error mesajı veriyoruz
        errorMessage.text = "Room Creation Failed " + message;
    }

    //Room Menu kısmında Leave Room kısmına verdiğimiz buton methodu. Odadan çıkış yapmasını sağlar.
    public void LeaveRoom()
    {
        //odadan çıkış yapması için gerekli olan photon sınıfı methodu
        PhotonNetwork.LeaveRoom();
        Debug.Log("Leave Room");
        //Menuleri aç kapa yapıyoruz
        menuControl.MenuActive(0);
        menuControl.MenuEnabled(2);
    }
    
    public void startGame()
    {

        StartCoroutine(loadSceneMenu(SceneManager.GetActiveScene().buildIndex+1));
    }
    public IEnumerator loadSceneMenu(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        menuControl.MenuActive(5);
        menuControl.MenuEnabled(2);
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress/.9f);
            slider.value = progress;
            sliderText.text = progress * 100f + "%";
            yield return null;
        }

    }

    //OnRoomListUpdate Methoduna " List<RoomInfo> veri tipi olarak roomList parametresini gönderiyoruz.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //foreach döngüsü ile RoomListContent kadar döndürüyoruz.
        foreach(Transform transfrom in roomListContent)
        {
            //roomListContent altında bulunan tüm odaları siliyoruz. 
            //Bunu yapma nedenimiz ise oda sayısını güncel tutmak.
            Destroy(transfrom.gameObject);
        }
        //roomList kadar dönsün döngü
        for (int i = 0; i < roomList.Count; i++)
        {
            //roomlistitem Prefabı roomListContent transformunda oluşsun.
            //roomListItemScript scriptinin Setup methoduna roomList parametresini gönderiyoruz.
            if(roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListItemPrefab,roomListContent).GetComponent<roomListItemScript>().Setup(roomList[i]);
        }
    }
    
    //roomListenItemScript içerisinden parametre gönderiyoruz.
    public void joinRoom(RoomInfo info)
    {
        //info parametresi roomListenItem'den gelen parametreye eşitleniyor ve aşşağıdaki method içerisine bilgiler gönderiliyor.
        PhotonNetwork.JoinRoom(info.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItemScript>().Setup(newPlayer);
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void Update()
    {
        if(keyboard!=null && !TouchScreenKeyboard.visible)
        {
            if(keyboard.done)
            {
                keyboard = null;
            }
        }
    }
    public void openKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("",TouchScreenKeyboardType.Default);
    }
}
