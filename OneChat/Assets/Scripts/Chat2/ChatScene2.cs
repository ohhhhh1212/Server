using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatScene2 : MonoBehaviour
{
    public ChatDlg2 m_ChatDlg = null;

    private void Start()
    {
        MyChatNet2.Inst.m_ChatScene = this;
    }

}
