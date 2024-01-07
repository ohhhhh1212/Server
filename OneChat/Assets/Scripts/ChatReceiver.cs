using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class ChatReceiver
{
    [Serializable]
    public class PacketData
    {
        public int packetId { get; set; } // Packet Type

        public PacketData() { }
        public PacketData(int nPacketId)
        {
            this.packetId = nPacketId;
        }
    }

    [Serializable]
    public class ReceiveData : PacketData
    {
        public IPEndPoint remoteEndPoint { get; private set; }
        public string msg { get; private set; }
        public string ipStr { get => remoteEndPoint.Address.ToString(); }
        public int port { get => remoteEndPoint.Port; }
        public string userName { get; private set; }

        public ReceiveData(int nPacketId, IPEndPoint remote, string sUserName, string sMsg) : base(nPacketId)
        {
            remoteEndPoint = remote;
            userName = sUserName;
            msg = sMsg;
        }
    }

    public event EventHandler<ReceiveData> OnChatReceiveHandeler = null;

    public string ipStr { get; private set; }
    public int port { get; private set; }

    Socket m_Socket = null;

    public bool isActive { get; set; } = true;

    public ChatReceiver(string sIp, int nPort)
    {
        ipStr = sIp;
        port = nPort;
    }

    public bool Start()
    {
        try
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iepoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
            m_Socket.Bind(iepoint);
            m_Socket.Listen(5);

            AcceptLoopAsync();
            return true;
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    public void CloseSocket()
    {
        m_Socket?.Close();
    }

    delegate void AcceptDele();
    private void AcceptLoopAsync()
    {
        AcceptDele dele = AssetLoop;
        dele.BeginInvoke(null, null);
    }

    private void AssetLoop()
    {
        Socket doSocket = null;

        while (isActive)
        {
            doSocket = m_Socket.Accept();
            DoItAsync(doSocket);
        }
    }

    delegate void DoItDele(Socket doSocket);
    private void DoItAsync(Socket doSocket)
    {
        //DoItDele dele = DoIt;
        //dele.BeginInvoke(dosocket, null, null);
        DoIt(doSocket);
    }

    private async void DoIt(Socket doSocket)
    {
        IPEndPoint remote = doSocket.RemoteEndPoint as IPEndPoint;
        byte[] packet = new byte[1024];

        // doSocket.Receive(packet);
        await doSocket.ReceiveAsync(packet, SocketFlags.None);
        doSocket.Close();

        MemoryStream ms = new MemoryStream(packet);
        BinaryReader br = new BinaryReader(ms);
        int nPacketId = br.ReadInt32();
        string sName = br.ReadString();
        string sMsg = br.ReadString();
        br.Close();
        ms.Close();

        // Thread 때문에 UI가 동작하지 않는다.
        if(OnChatReceiveHandeler != null)
        {
            OnChatReceiveHandeler(this, new ReceiveData(0, remote, sName, sMsg));
        }
    }
}