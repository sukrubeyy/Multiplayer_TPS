using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public float minDistance;
    public float maxDistance;
    public float smooth = 10f;
    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;
    public Vector3 defaultPos;
    PlayerController playerScript;
    private void Awake()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        //x=2,y=3,z=5
        dollyDir = transform.localPosition.normalized;
        //x=4,y=9,z=25
        distance = transform.localPosition.magnitude;
    }

    void Update()
    {
        //Obje ile kamera arasına mesafe koyuyoruz ve bunun pozisyonunun bilgisini alıyoruz
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;
        //Kamera holder objesi ile yukarda desiredCameraPos arasında herhangi bir collider var ise bu şart içerisine girecektir.
        if(Physics.Linecast(transform.parent.position,desiredCameraPos,out hit))

        {   
              //if (hit.collider.tag != "taban")
               playerScript.fireCollision = true;
               distance = Mathf.Clamp(hit.distance * 0.09f, minDistance, maxDistance);
        }
        else
        {
            playerScript.fireCollision = false;
            distance = maxDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}
