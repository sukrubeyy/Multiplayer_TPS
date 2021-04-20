using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hastable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviourPunCallbacks , IDamageable 
{
    [Header("Movement")]
    float horizontal, vertical, speed = 5f, mouseX, mouseY;
    Vector3 position;
    Rigidbody rb;

    [Header("Camera")]
    [SerializeField] GameObject camDefaultPos, camRightClickPos;
    [SerializeField] float rotationSensitivy;

    [Header("Photon")]
    public PhotonView PV;

    [Header("GUN")]
    public Item[] items;
    int itemIndex;
    int previousItemIndex = -1;
    float fireRate = 15f, nextTimeToFire = 0f;
    public bool fireCollision = false;

    [Header("Animation")]
    Animator animator;
    public PlayerManager playerManager;
    bool jumper = true;
   

    [Header("İskeletler")]
    Transform _Spine;
    public Vector3 headPos;
    public Vector3 spineRotation;

    [Header("UI")]
    [SerializeField] Transform tabPlayerList;
    [SerializeField] GameObject playerListItemObject;
    const float maxHealt = 100f;
    float currentHealt = maxHealt;
    [SerializeField] TMP_Text healtText;
    public float jarjoR, toplamMermi;
    public GameObject gameSettings;
    public AudioSource effectSound;
    public Sprite[] musicIconSprite;
    public GameObject chatMenu;
    public TMP_InputField message_text;
    public Transform messageContent;
    public GameObject messageTextPrefabs;
    public bool keepFire = true;
    public bool dontMove = true;
    [SerializeField] Button chatButton;
    TouchScreenKeyboard keyboard;
    [SerializeField] GameObject[] gunsSprite;
    [SerializeField] Joystick joystick;
    bool rotateCheck = true;

    [Header("Camera Collision")]
    public float minDistance;
    public float maxDistance;
    public float smooth = 10f;
    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;
    public Vector3 defaultPos;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        animator = GetComponent<Animator>();
        jarjoR = 1;
    }
    void Start()
    {
        dollyDir = transform.GetChild(4).GetChild(0).localPosition.normalized;
        distance = transform.GetChild(4).GetChild(0).localPosition.magnitude;
        
        //_LeftHandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        _Spine = animator.GetBoneTransform(HumanBodyBones.Chest);
        gunSwitch(0);
        if (PV.IsMine)
        {
            
            
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<Canvas>().gameObject);
            Destroy(rb);
        }
       
     
    }

    [System.Obsolete]
    void Update()
    {
        //Write override methodu herkeste çalışsın. belki bug o şekilde çözülebilir.

        //network kontrolü
        if (!PV.IsMine)
        {
            return;
        }
        if (keyboard != null && !TouchScreenKeyboard.visible)
        {
            if (keyboard.done)
            {
                keyboard = null;
            }
        }

        if (dontMove)
        {

           
            
            if(Input.GetMouseButton(0) && rotateCheck)
            {
               
                CameraRotateWithMousePos();
            }
            
            
            if (transform.position.y <= -10f)
            {
                Die();
            }
            animatorController();
            healtText.text = "HP:" + currentHealt;
            cameraCollision();
        }
        if (jarjoR <= 0)
        {
            keepFire = false;
            animator.SetBool("Reload", true);
        }
        items[itemIndex].write();
    }
    [System.Obsolete]
    void FixedUpdate()
    {

        if (!PV.IsMine)
        {
            return;
        }
        if (dontMove)
            Movement();
    }

    public void LateUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }

       
        spineRotation.x = -mouseX;
        _Spine.rotation = _Spine.rotation * Quaternion.Euler(spineRotation);
    }
    /// <summary>
    /// Fire buton için tasarlanmış method.
    /// </summary>
    public void FireButton()
    {
        
        if (jarjoR > 0 && Time.time >= nextTimeToFire && keepFire)
        {
            items[itemIndex].Use();
            animator.SetBool("Fire", true);
            nextTimeToFire = Time.time + 1f / fireRate;
            effectSound.Play();
        }
    }
    /// <summary>
    /// UI arayüzünde yer alan butonlar için tasarlanmış false değer döndüren method.
    /// </summary>
    /// <param name="_boolParam"></param>
    public void RotateFalse(bool _boolParam)
    {
        rotateCheck = _boolParam;
    }
    /// <summary>
    /// UI arayüzünde yer alan butonlar için tasarlanmış true değer döndüren method.
    /// </summary>
    /// <param name="_boolParam"></param>
    public void RotateTrue(bool _boolParam)
    {
        rotateCheck = _boolParam;
    }
    /// <summary>
    /// Gun0 Objesi için oluşturulmuş method. Silah 0'ı seçmemize olanak sağlar.
    /// </summary>
    public void gun0Button()
    {
        gunSwitch(0);
        gunsSprite[0].GetComponent<Image>().color = new Color32(255,255,255,255);
        gunsSprite[1].GetComponent<Image>().color = new Color32(255, 255, 255, 100);

    }
    /// <summary>
    /// Gun1 Objesi için oluşturulmuş method. Silah 1'i seçmemize olanak sağlar.
    /// </summary>
    public void gun1Button()
    {
        gunSwitch(1);
        gunsSprite[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        gunsSprite[0].GetComponent<Image>().color = new Color32(255, 255, 255, 100);

    }
    /// <summary>
    /// Fire Animasyonuna Event olarak eklenmiştir ve fire animasyonunun bitmesini sağlar.
    /// </summary>
    public void FireAnimationFalse()
    {
        animator.SetBool("Fire", false);
    }
    /// <summary>
    /// Reload button için tasarlanmış method.
    /// </summary>
    public void ReloadButton()
    {
        if (jarjoR < toplamMermi)
        {
            keepFire = false;
            animator.SetBool("Reload", true);
        }
    }
    /// <summary>
    /// Lobby' menüsünde dönmek veya oyundan çıkmak için gerekli olan settings butonu için gerekli olan method.
    /// </summary>
    public void settingsMenu()
    {
            if (!chatMenu.active)
            {
                if (gameSettings.active)
                {
                    gameSettings.SetActive(false);
                    dontMove = true;
                }
                else
                {
                    gameSettings.SetActive(true);
                    dontMove = false;
                }
            }
    }
    /// <summary>
    /// Chat menünün aktif olması için gerekli olan method
    /// </summary>
    public void chatButtonMenu()
    {
        if (!chatMenu.active)
        {
            chatButton.gameObject.SetActive(false);
            chatMenu.SetActive(true);
            dontMove = false;
        }
    }
    /// <summary>
    /// Oyun içerisinde yer alan chat menu aktif olunca kullanıcının mesaj yazabilmesi için
    /// keyboard aktif edilmesi için gerekli olan method.
    /// </summary>
    public void chatKeyboardActive()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
    /// <summary>
    /// Run animasyonunun butondaki kontrolu için gereli olan method.
    /// </summary>
    public void RunButton()
    {
        if (animator.GetBool("RunActive"))
        {
            animator.SetBool("RunActive", false);
        }
        else
        {
            animator.SetBool("RunActive", true);
        }
    }
    /// <summary>
    /// Zıplama animasyonunun buton tarafından kontrol edilmesi için gerekli method.
    /// </summary>
    public void JumpButton()
    {
        if (jumper)
        {
            jumper = false;
            animator.SetBool("Jump", true);
        }
    }
    void Movement()
    {
        horizontal = joystick.Horizontal;
        vertical = joystick.Vertical;
        position = new Vector3(horizontal, 0, vertical);
        position = transform.TransformDirection(position);
        rb.position += position * Time.fixedDeltaTime * speed;

    }
    /// <summary>
    /// Zıplaması için gereken method. Bu method animasyon içerisinde event olarak çalışmaktadır.
    /// </summary>
    public void Jump()
    {
        if (!PV.IsMine)
            return;
        rb.AddForce(new Vector3(0, 200, 0));
        
    }
    /// <summary>
    /// Silah seçimi yapabilmemiz için gerekli olan method.
    /// </summary>
    /// <param name="_index">Hangi silahın aktif olmasını istiyorsak items parametresi olarak göndermeliyiz</param>
    void gunSwitch(int _index)
    {
        animator.SetBool("Reload", false);
        keepFire = true;

        if (_index == previousItemIndex)
        {
            return;
        }
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        previousItemIndex = itemIndex;

        //Sunucuya Silah seçimlerimizi göndermek için
        if (PV.IsMine)
        {
            Hastable has = new Hastable();
            has.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(has);
        }
    }
    /// <summary>
    /// Mouse pozisyonu ile kamera rotasyonunu değiştirebildiğimiz method.
    /// </summary>
    void CameraRotateWithMousePos()
    {
        if (!rotateCheck)
            return;

            mouseX += Input.GetAxisRaw("Mouse Y");
            mouseX = Mathf.Clamp(mouseX, -15f, 20f);
            mouseY = Input.GetAxisRaw("Mouse X");
        
          
            
            transform.Rotate(Vector3.up * mouseY * rotationSensitivy);
            transform.GetChild(4).localEulerAngles = new Vector3(-mouseX, 0);

    }
    /// <summary>
    /// Kameramızın objeler içerisinden geçmesini önlemek için CameraCollision kavramını kullandığımız method.
    /// </summary>
    void cameraCollision()
    {
        Vector3 desiredCameraPos = transform.GetChild(4).GetChild(0).parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;
        //Kamera holder objesi ile yukarda desiredCameraPos arasında herhangi bir collider var ise bu şart içerisine girecektir.
        if (Physics.Linecast(transform.GetChild(4).GetChild(0).parent.position, desiredCameraPos, out hit))

        {
            //if (hit.collider.tag != "taban")
            fireCollision = true;
            distance = Mathf.Clamp(hit.distance * 0.09f, minDistance, maxDistance);
        }
        else
        {
            fireCollision = false;
            distance = maxDistance;
        }
        transform.GetChild(4).GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(4).GetChild(0).localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
    /// <summary>
    /// Animator'ü kontrol edebilmemiz için gerekli olan method. Bu method içerisinde birçok parametre gönderilmektedir.
    /// </summary>
    void animatorController()
    {
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
     
        if(animator.GetBool("RunActive"))
        {
            speed = 7;
        }
        else
        {
            speed = 5;
        }
       
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    if(animator.GetBool("RunActive"))
        //    {
        //        animator.SetBool("RunActive", false);
        //    }
        //    if (animator.GetBool("crouching"))
        //    {
        //        animator.SetBool("crouching", false);
        //    }
        //    else
        //    {
        //        animator.SetBool("crouching", true);
        //    }
        //}
    }
    /// <summary>
    /// Zıplama animasyonuna event olarak eklenmiş bir method. Animasyonun bitmesini ve döngüden çıkabilmesi için
    /// parametreye false bool return etmektedir.
    /// </summary>
    public void JumpAnimationFalse()
    {
        animator.SetBool("Jump", false);
    }
    /// <summary>
    /// Bu method reload animasyonun içerisinde event olarak verilmiştir. İçerisinde abstract class içerisinde yer alan method çalışmaktadır.
    /// Bu sayede reload animasyonu bittiği zaman UI textlerde mermi bilgisi değişmektedir.
    /// </summary>
    public void BulletReloadMethod()
    {
        items[itemIndex].ReloadBulletMethod();
    }
    /// <summary>
    /// Oyun oynarken tab tuşuna basarark PhotonNetwork'ten almış olduğumuz player bilgilerini yazdırmamız için gereken method.
    /// </summary>
    void tabPlayerWrite()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform trans in tabPlayerList)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemObject, tabPlayerList).GetComponent<PlayerListItemScript>().Setup(players[i]);
        }
    }
    /// <summary>
    /// Reload animasyonunun içerisine event olarak verilmiştir. Animasyon bittiğinde empty animasyona geçmesi için animasyon false olmaktadır.
    /// </summary>
    public void ReloadFalseAnimation()
    {
        animator.SetBool("Reload", false);
        keepFire = true;
    }
    /// <summary>
    /// Karakterlerin ellerindeki silahları sunucuya gönderiyoruz bu sayede karşı karakterler silahlarını değiştirse bunların bilgisini alabileceğiz.
    /// </summary>
    /// <param name="targetPlayer">Oyuncu parametresi</param>
    /// <param name="changedProps">Silahın özelliklerinin yer aldığı Hastable Parametresi</param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hastable changedProps)
    {
        if(!PV.IsMine && targetPlayer==PV.Owner)
        {
            gunSwitch((int)changedProps["itemIndex"]);
        }
    }
    /// <summary>
    /// Kullanıcı odadan ayrıldığı zaman çalışan PhotonNetwork methodu.
    /// </summary>
    public override void OnLeftRoom()
    {
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
    /// <summary>
    /// Herhangi bir yere temas edip etmediğini anlıyoruz.
    /// </summary>
    /// <param name="col">Temas ettiği collision</param>
    public void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag!="Player")
        {
           
        }
        jumper = true;
        Debug.Log(col.gameObject.name);
        
    }
    /// <summary>
    /// ESC menüsünde Quit butonu için çalışan method.
    /// </summary>
    public void QuitThisGame()
    {
        Application.Quit();
    }
    /// <summary>
    /// ESC menüsünde Go Lobby butonu için çalışan method.
    /// </summary>
    /// <param name="_paramScene">Döneceği Sahne</param>
    public void GoBackLobbyScene(int _paramScene)
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(_paramScene);
    }
    /// <summary>
    /// Y tuşuna basıldığında Chat menusu aktif olmaktadır. Chat menusunde yer alan gönder butonuna basıldığında 
    /// bu method aktif hale gelmektedir. Ve PUNRPC'ye bilgi göndermektedir. Bu sayede tüm oyuncular mesajı görebilir.
    /// </summary>
    public void messageText()
    {

        string mesajText = message_text.text;
        string PlayerName = PhotonNetwork.NickName;
        PV.RPC("RPC_SendMessage", RpcTarget.All, mesajText, PlayerName);
        message_text.text = null;
        dontMove = true;
        chatMenu.SetActive(false);
        chatButton.gameObject.SetActive(true);
    }
    /// <summary>
    /// IDamageable interface içerisinden silah damage bilgisini alıp buraya ulaştırdıktan sonra PUNRPC TakeDamage methodu
    /// oluşturup bu methodun içerisine damage bilgisini göndermekteyiz. Çünkü Tüm oyuncularda bu bilgi çalışmalı.
    /// </summary>
    /// <param name="_damage"></param>
    public void TakeDamage(float _damage)
    {
        //Photon serverlarına bilgi gönderiyoruz.
        PV.RPC("RPC_TakeDamage",RpcTarget.All,_damage);
    }
    /// <summary>
    /// IDamageable içerisinde yer alan float TakeBullet methodu ile jarjor içerisindeki mermi bilgisini almaktayız.
    /// bu method float değer döndürmektedir.Bilgiyi ise abstract class içerisinden oluşturduğumuz gun itemlerinden almaktayız.
    /// </summary>
    /// <param name="_bullet">Jarjor Bilgisi</param>
    /// <returns></returns>
    float IDamageable.TakeBullet(float _bullet)
    {
      
        jarjoR = _bullet;
        return jarjoR;
    }
    /// <summary>
    /// IDamageable içerisinde yer alan float TakeBullet2 methodu ile jarjorun totalde alabildiği toplam 
    /// mermi sayısını bu script içerisine ulaştırmaktayız. Bu sayede reload animasyonunu kontrol edebiliyoruz.
    /// </summary>
    /// <param name="_ToplamBullet">Mermi kapasitesi</param>
    /// <returns></returns>
    float IDamageable.TakeBullet2(float _ToplamBullet)
    {
       toplamMermi = _ToplamBullet;
        return toplamMermi;
    }
    /// <summary>
    /// Eğer damage aldığımız zaman kameramızın titremesini istiyorsak bu methodu kullanabiliriz.
    /// </summary>
    /// <param name="time">Süre</param>
    /// <param name="force">Kuvvet</param>
    IEnumerator CameraShaker(float time, float force)
    {
        Vector3 originalPosition = transform.GetChild(4).GetChild(0).localPosition;

        float timeUp = 0f;
        while (timeUp < time)
        {
            float x = Random.Range(-1f,0f) * force;
            float y = Random.Range(.5f,1.5f) * force;
            transform.GetChild(4).GetChild(0).localPosition = new Vector3(x, y,originalPosition.z);
            timeUp += Time.deltaTime;
            yield return null;
        }
        transform.GetChild(4).GetChild(0).localPosition = originalPosition;

    }
    /// <summary>
    /// Sunucularda Çalışacak olan RPC_TAKEDAMAGE methodu. Fakat bunun içerisinde eğer kullanıcı biz isek methodu
    /// kırıyoruz. Çünkü kendi kendimize hasar veremeyiz.
    /// </summary>
    /// <param name="damage">Damage Info</param>
    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        //Bunu yapmamızın nedeni karşı tarafın bize atış yaptığında hangi silah ile vurduğunun bilgisini almamız için.
        //Çünkü diyelim ki karşı taraf bizi vurdu çalışacak sistem şudur.
        //SingleShoutGunScript içerisindeki raycast ile IDamageable scriptine bir damage parametresi gönderiyoruz.
        //Bu parametre silahın hasar kaydını Playercontroller içerisindeki TakeDamage parametresine gönderiyor.
        //Daha sonra biz bu parametreyi de RPC_TakeDamage methoduna gönderiyoruz.
        //Eğer ben değilsem ateş eden , benim canımı azalt.
        //Eğer ben isem döngüyü kır.

        if (!PV.IsMine)
            return;
        if(currentHealt>=0)
        {
            currentHealt -= damage;

          //  StartCoroutine(CameraShaker(.15f, .03f));

            if (currentHealt<=0)
            {
                currentHealt = 0;
                //animator.SetBool("Death", true);
                Die();
            }
        }
    }

    /// <summary>
    ///Karakterin Healt<=0 ise ölmesi için PlayerManager objesine ulaşıp oluşturmuz olduğumuz objeyi yok edip
    /// tekrar oluşturuyoruz.Eğer PlayerManager'ı yok edersek kullanıcı serverdan kopar.Bu sebepten dolayı
    /// PlayerObjesi ve PlayerManager objesi olmak üzere ikiye ayırdık.
    /// </summary>
    public void Die()
    {
        playerManager.Die();
    }

    /// <summary>
    /// Mesajları Sunucularda gösterebilmek için PunRpc içerisinde oluşturduğumuz method.
    /// Bu method sayesinde tüm kullanıcılar birbirlerine mesaj atabiliyor. Fakat kullanıcı ben isem bu method 
    /// çalışmaktadır.
    /// </summary>
    /// <param name="_Messagetext">Message Info</param>
    /// <param name="PlayerName">Player Name Info</param>
    [PunRPC]
    void RPC_SendMessage(string _Messagetext , string PlayerName)
    {
        ShowMessage(_Messagetext,PlayerName);
    }
    /// <summary>
    /// RPC_SendMessage içerisinde yer alan methoddur. Mesajın nerede nasıl oluşacağını belirttiğimiz bir methoddur.
    /// </summary>
    /// <param name="_Text">Message Text</param>
    /// <param name="PlayerName">Player Name</param>
    public void ShowMessage(string _Text, string PlayerName)
    {
        GameObject scene2Canvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject message = Instantiate(messageTextPrefabs);
        message.transform.SetParent(scene2Canvas.transform.GetChild(0).GetChild(0).transform);
        // GameObject message = PhotonNetwork.Instantiate(Path.Combine("Prefabs","TMP"),messageContent.transform.position,messageContent.transform.rotation,0);
        message.GetComponent<messageText_Script>().WriteText(_Text, PlayerName);
    }
}
