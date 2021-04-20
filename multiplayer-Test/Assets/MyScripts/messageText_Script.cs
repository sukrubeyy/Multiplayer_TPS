using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class messageText_Script : MonoBehaviour
{
  // public Text messageText;
    public TMP_Text tmptext;

    public void  WriteText(string _infoText,string playerName)
    {
     //   messageText.text = _infoText;
        tmptext.text = playerName+":"+_infoText;
        Invoke("Destroy", 10f);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
