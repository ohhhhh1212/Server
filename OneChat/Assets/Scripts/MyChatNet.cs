using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyChatNet : MonoBehaviour
{
    public enum EPacket
    {
        Chat = 1001,
    }

    public static MyChatNet Inst = null;

    public ChatSender m_ChatSender = new ChatSender();
    public ChatReceiver m_ChatReceiver = null;
    //Coroutine m_Coroutine = null;

    Queue<ChatReceiver.PacketData> m_EventQueue = new Queue<ChatReceiver.PacketData>();

    public ChatScene m_ChatScene { get; set; } = null;
    public bool IsAction { get; set; } = true;

    private void Awake()
    {
        Inst = this;
    }

    public void StartReceiver(string ip, int port)
    {
        m_ChatReceiver = new ChatReceiver(ip, port);
        m_ChatReceiver.OnChatReceiveHandeler += GetPacket;
        m_ChatReceiver.Start();
        StartCoroutine(Co_Recieve());
    }

    void GetPacket(object sender, ChatReceiver.ReceiveData data)
    {
        m_EventQueue.Enqueue(data);
    }

    public void SendChat(string otherIp, int otherPort, string sName, string sMsg)
    {
        m_ChatSender.SendMsgAsync(otherIp, otherPort, sName, sMsg);
    }

    IEnumerator Co_Recieve()
    {
        while (IsAction)
        {
            yield return new WaitUntil(() => m_EventQueue.Count > 0);

            ChatReceiver.ReceiveData packet = (ChatReceiver.ReceiveData)m_EventQueue.Dequeue();

            m_ChatScene.m_ChatDlg.CreateText(packet.userName, packet.msg);
        }
    }

    private void OnDestroy()
    {
        if(m_ChatReceiver != null)
            m_ChatReceiver.CloseSocket();
    }
}
