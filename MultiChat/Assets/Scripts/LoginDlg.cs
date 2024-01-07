using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginDlg : MonoBehaviour
{
    public InputField m_inputID = null;
    public InputField m_inputPW = null;
    public Button m_btnJoin = null;
    public Button m_btnLogin = null;
    public MainDlg MainDlg = null;

    void Init()
    {
        m_btnJoin.onClick.AddListener(OnClicked_Join);
        m_btnLogin.onClick.AddListener(OnClicked_Login);

        SocketMgr.Inst.onAck_Login += OnCallback_Login;
    }

    private void Start()
    {
        SocketMgr.Inst.InitSocketIO();
        Init();
    }

    void OnCallback_Login(object obj, int success)
    {
        if(success == 0) // 로그인 성공
        {
            MainDlg.SetMyId();

            m_inputID.text = string.Empty;
            m_inputPW.text = string.Empty;

            SocketMgr.Inst.isLogin = true;

            gameObject.SetActive(false);
        }
    }

    void OnClicked_Join()
    {
        SocketMgr.Inst.SendJoin(m_inputID.text, m_inputPW.text);
    }

    void OnClicked_Login()
    {
        SocketMgr.Inst.SendLogin(m_inputID.text, m_inputPW.text);
    }

}
