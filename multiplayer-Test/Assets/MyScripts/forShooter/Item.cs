using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public itemInfo itemInfo;
    public GameObject itemGameObject;


    public abstract void Use();
    public abstract void write();
    public abstract void ReloadBulletMethod();
}

