using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatScene : MonoBehaviour
{
    public ChatDlg m_ChatDlg = null;

    private void Start()
    {
        MyChatNet.Inst.m_ChatScene = this;
    }

}
