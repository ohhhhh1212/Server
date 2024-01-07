using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextItem : MonoBehaviour
{
    public Text m_txtMsg = null;

    public void Init(string text)
    {
        m_txtMsg.text = text;
    }
}
