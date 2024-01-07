using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ChatSender2
{
    delegate void SendDele(string ip, int port, byte[] packet);

    public void SendMsgAsync(string ip, int port, byte[] packet)
    {
        SendDele dele = SendMsg;
        dele.BeginInvoke(ip, port, packet, null, null);
    }

    private void SendMsg(string ip, int port, byte[] packet)
    {
        try
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iePoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.Connect(iePoint);

            socket.Send(packet);
            socket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
