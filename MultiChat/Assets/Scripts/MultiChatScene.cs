using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChatScene : MonoBehaviour
{
    public MainDlg MainDlg = null;
    public LoginDlg LoginDlg = null;

    private void Awake()
    {
        MyDataNet.Inst.m_ChatScene = this;
    }

}
