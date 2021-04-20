using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScripts : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroyGameobject());
    }
    IEnumerator DestroyGameobject()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
