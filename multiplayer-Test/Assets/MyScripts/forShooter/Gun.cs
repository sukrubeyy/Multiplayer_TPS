using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public ParticleSystem muzzleFlash;
    public abstract override void Use();
    public abstract override void write();
    public abstract override void ReloadBulletMethod();
    public GameObject bulletImpact;

    

}
