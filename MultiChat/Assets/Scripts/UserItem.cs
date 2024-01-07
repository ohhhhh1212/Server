using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserItem : MonoBehaviour
{
    public Text m_txtName = null;
    public string m_id = "";

    public void Init(string id)
    {
        m_txtName.text = id;
        m_id = id; 
    }
    
}