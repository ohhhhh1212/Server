using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatDlg : MonoBehaviour
{
    [SerializeField] ScrollRect m_Scroll = null;
    [SerializeField] InputField m_inputText = null;
    [SerializeField] Button m_btnSend = null;
    [SerializeField] GameObject m_preText = null;
    [Header("MySetting")]
    [SerializeField] InputField m_myIp = null;
    [SerializeField] InputField m_myPort = null;
    [SerializeField] InputField m_myName = null;
    [SerializeField] Button m_btnMySetting = null;
    [Header("OtherSetting")]
    [SerializeField] InputField m_otIp = null;
    [SerializeField] InputField m_otPort = null;
    [SerializeField] Button m_btnOtSetting = null;

    string otIp = "";
    int otPort = 0;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        m_btnSend.onClick.AddListener(OnClicked_Send);
        m_btnMySetting.onClick.AddListener(OnClicked_MySetting);
        m_btnOtSetting.onClick.AddListener(OnClicked_OtherSetting);
        m_inputText.onSubmit.AddListener(OnSubmit_Text);
    }
    
    void SendText(string kmsg)
    {
        if (otIp == "" || otPort == 0)
        {
            Debug.Log("상대방의 정보를 입력해주세요");
            return;
        }

        string name = m_myName.text;
        string msg = kmsg;

        if (IsStringEmpty(name, msg))
            return;

        CreateText(name, msg);

        MyChatNet.Inst.SendChat(otIp, otPort, name, msg);

        m_inputText.text = "";

        m_inputText.ActivateInputField();
    }

    void OnClicked_Send()
    {
        string msg = m_inputText.text;

        SendText(msg);
    }

    void OnClicked_MySetting()
    {
        string ip = m_myIp.text;
        string port = m_myPort.text;

        if (IsStringEmpty(ip, port))
            return;

        MyChatNet.Inst.StartReceiver(m_myIp.text, int.Parse(m_myPort.text));
    }

    void OnClicked_OtherSetting()
    {
        if (IsStringEmpty(m_otIp.text, m_otPort.text))
            return;

        otIp = m_otIp.text;
        otPort = int.Parse(m_otPort.text);
    }
    
    void OnSubmit_Text(string kmsg)
    {
        SendText(kmsg);
    }

    bool IsStringEmpty(string s1, string s2)
    {
        if(s1 == "" || s2 == "")
        {
            Debug.Log("값이 제대로 입력 되었는지 확인해주세요.");
            return true;
        }

        return false;
    }

    public void CreateText(string name, string msg)
    {
        GameObject go = Instantiate(m_preText, m_Scroll.content);
        Text txt = go.GetComponent<Text>();
        txt.text = $"{name} : {msg}";
    }

    private void OnDestroy()
    {
        for (int i = 0; i < m_Scroll.content.childCount; i++)
        {
            Destroy(m_Scroll.content.GetChild(i).gameObject);
        }
    }
}
