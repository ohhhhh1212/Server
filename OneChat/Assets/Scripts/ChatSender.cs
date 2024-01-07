using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ChatSender
{
    delegate void SendDele(string otherIp, int otherPort, string sName, string sMsg);

    private int m_PacketId = 0; // ÆÐÅ¶ id
    public void SendMsgAsync(string otherIp, int otherPort, string sName, string sMsg)
    {
        SendDele dele = SendMsg;
        dele.BeginInvoke(otherIp, otherPort, sName, sMsg, null, null);
    }

    private void SendMsg(string otherIp, int otherPort, string sName, string sMsg)
    {
        try
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iePoint = new IPEndPoint(IPAddress.Parse(otherIp), otherPort);
            socket.Connect(iePoint);

            byte[] packet = MakeMsgPacket(sName, sMsg);
            socket.Send(packet);
            socket.Close();
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private byte[] MakeMsgPacket(string sName, string sMsg)
    {
        byte[] packet = new byte[1024];
        MemoryStream ms = new MemoryStream(packet);
        BinaryWriter bw = new BinaryWriter(ms);

        bw.Write(m_PacketId);
        bw.Write(sName);
        bw.Write(sMsg);

        bw.Close();
        ms.Close();
        return packet;
    }
}
