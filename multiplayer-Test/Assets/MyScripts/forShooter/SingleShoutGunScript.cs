using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class SingleShoutGunScript : Gun
{
    [SerializeField] Camera cam;
    [SerializeField] Animator playerAnimator;
    RaycastHit hit;
    PhotonView PV;
    [SerializeField] TMP_Text bulletCount;
    float jarjorMermisi;
    float toplamMermi;
    public GameObject PlayerObject;
    public IDamageable InterfaceScript;
    float writeRange = 15f, nextoTimeWrite = 0f;
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        InterfaceScript = (IDamageable)PlayerObject.GetComponent("IDamageable");
        jarjorMermisi = ((gunInfo)itemInfo).jarjor;
        toplamMermi = ((gunInfo)itemInfo).toplamBullet;
    }
    /// <summary>
    /// Abstarct class içerisinde yer alan Use methodu içerisinde raycast yer almaktadır ve ateş edebilmemiz için 
    /// gerekli olan kodlar çalışmaktadır.
    /// </summary>
    public override void Use()
    {
        Shoot();
    }
    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out hit))
        {
            muzzleFlash.Play();
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((gunInfo)itemInfo).damage);
            Debug.DrawLine(cam.transform.position, hit.point, Color.green);
         
            if (jarjorMermisi > 0)
            {
                jarjorMermisi--;
            }
       
            PV.RPC("RPC_shout", RpcTarget.All, hit.point, hit.normal);
            Debug.DrawLine(cam.transform.position,hit.point,Color.red);
        }
    }
    public override void write()
    {
        InterfaceScript.TakeBullet(jarjorMermisi);

        InterfaceScript.TakeBullet2(((gunInfo)itemInfo).jarjor);
        if(Time.time>=nextoTimeWrite)
        {
            nextoTimeWrite = Time.time + 1f / writeRange;
            bulletCount.text = jarjorMermisi + "/" + toplamMermi;
        }
    }

    public override void ReloadBulletMethod()
    {
       
        if (jarjorMermisi <= 0)
        {
            jarjorMermisi = ((gunInfo)itemInfo).jarjor;

        }

        if (jarjorMermisi < ((gunInfo)itemInfo).jarjor && toplamMermi > 0)
        {
            jarjorMermisi = ((gunInfo)itemInfo).jarjor;
         
        }
    }
    [PunRPC]
    void RPC_shout(Vector3 hitPosition,Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition,0.3f);
        if(colliders.Length!=0)
        {
            GameObject bulletObj = Instantiate(bulletImpact, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up));
            Destroy(bulletObj, 5f);
            bulletObj.transform.SetParent(colliders[0].transform);
        }

   
    }

}
