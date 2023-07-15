using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetUserInfo : MonoBehaviour
{
    public TMP_Text textComponent;
    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = UserInfo.email + " " + UserInfo.name + " " + UserInfo.id;
    }
}
