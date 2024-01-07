using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MyChatNet2 : MonoBehaviour
{
    public enum EPacket
    {
        Chat = 1001,
    }

    public static MyChatNet2 Inst = null;

    ChatSender2 m_ChatSender = new ChatSender2();
    ChatReceiver2 m_ChatReceiver = null;
    Coroutine m_Coroutine = null;

    Queue<ChatReceiver2.ReceiveData> m_EventQueue = new Queue<ChatReceiver2.ReceiveData>();

    public ChatScene2 m_ChatScene { get; set; } = null;
    public bool IsAction { get; set; } = true;

    private void Awake()
    {
        Inst = this;
    }

    public void StartReceiver(string ip, int port)
    {
        m_ChatReceiver = new ChatReceiver2(ip, port);

        m_ChatReceiver.OnChatReceiveHandeler += GetPacket;
        m_ChatReceiver.Start();
        m_Coroutine = StartCoroutine(Co_Recieve());
    }

    public void CloseServer()
    {
        if (m_ChatReceiver != null)
        {
            m_ChatReceiver.OnChatReceiveHandeler -= GetPacket;
            m_ChatReceiver.CloseSocket();
        }

        IsAction = false;

        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);
    }

    void GetPacket(object sender, ChatReceiver2.ReceiveData data)
    {
        m_EventQueue.Enqueue(data);
    }

    IEnumerator Co_Recieve()
    {
        while (IsAction)
        {
            yield return new WaitUntil(() => m_EventQueue.Count > 0);

            ChatReceiver2.ReceiveData packet = m_EventQueue.Dequeue();

            OnReceived_Data(packet);
        }
    }

    //void OnReceived_Data(ChatReceiver2.ReceiveData kData)
    //{
    //    if(kData.packetId == (int)EPacket.Chat)
    //    {
    //        MemoryStream ms = new MemoryStream(kData.data);
    //        BinaryReader br = new BinaryReader(ms);

    //        string name = br.ReadString();
    //        string msg = br.ReadString();

    //        ms.Close();
    //        br.Close();

    //        m_ChatScene.m_ChatDlg.CreateText(name, msg);
    //    }
    //}

    void OnReceived_Data(ChatReceiver2.ReceiveData kData)
    {
        if (kData.packetId == (int)EPacket.Chat)
        {
            MemoryStream ms = new MemoryStream(kData.data);
            BinaryReader br = new BinaryReader(ms);

            short nameSize = br.ReadInt16();
            byte[] nameBuf = br.ReadBytes(nameSize);

            short msgSize = br.ReadInt16();
            byte[] msgBuf = br.ReadBytes(msgSize);

            ms.Close();
            br.Close();

            string name = Encoding.UTF8.GetString(nameBuf);
            string msg = Encoding.UTF8.GetString(msgBuf);

            m_ChatScene.m_ChatDlg.CreateText(name, msg);
        }
    }

    //public void SendMessage(string ip, int port, string name, string msg)
    //{
    //    byte[] packet = new byte[1024];

    //    short packetId = (short)EPacket.Chat;
    //    short size = (short)(ChatReceiver2.HeaderSize + name.Length * 3 + msg.Length * 3);

    //    MemoryStream ms = new MemoryStream(packet);
    //    BinaryWriter bw = new BinaryWriter(ms);

    //    bw.Write(packetId);
    //    bw.Write(size);

    //    bw.Write(name);
    //    bw.Write(msg);

    //    ms.Close();
    //    bw.Close();

    //    m_ChatSender.SendMsgAsync(ip, port, packet);
    //}

    public void SendMessage(string ip, int port, string name, string msg)
    {
        byte[] packet = new byte[1024];

        short packetId = (short)EPacket.Chat;

        byte[] nameBuf = Encoding.UTF8.GetBytes(name);
        short nameSize = (short)nameBuf.Length;

        byte[] msgBuf = Encoding.UTF8.GetBytes(msg);
        short msgSize = (short)msgBuf.Length;

        short size = (short)(ChatReceiver2.HeaderSize + nameSize + msgSize + sizeof(short) * 2);

        MemoryStream ms = new MemoryStream(packet);
        BinaryWriter bw = new BinaryWriter(ms);

        bw.Write(packetId);
        bw.Write(size);

        bw.Write(nameSize);
        bw.Write(nameBuf);

        bw.Write(msgSize);
        bw.Write(msgBuf);

        ms.Close();
        bw.Close();

        m_ChatSender.SendMsgAsync(ip, port, packet);
    }

    private void OnDestroy()
    {
        CloseServer();
    }
}
